using System;
using AO;

public class BrainrotCatalogEntry
{
	public string Name;
	public long BaseGoldGeneration;
	public long BasePrice;
	public int BasePartsToEvolve; // per-species base parts required for first evolution (0 = use rarity default)
	public Texture Sprite;
	public BrainrotValueRarity Rarity; // Common, Rare, Epic, Legendary, Mythic
	public int StableId; // Manually assigned, fixed across content updates
	public AudioAsset Sound;
	public bool IsEvolution = false;
	public string StoreProductId = string.Empty;
	public bool IsNotStealable = false;
	public string EventId = string.Empty;
	public Action<Enemy> OnSpawn;
}

public static class BrainrotCatalog
{
	public static readonly Dictionary<string, BrainrotCatalogEntry> Entries = new Dictionary<string, BrainrotCatalogEntry> { };
	private static int NextStableId;
	// Species evolution map: from catalogId -> to catalogId
	public static readonly Dictionary<string, string> EvolutionMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

	// Pre-sorted cache by rarity for efficient UI enumeration (Common → Mythic)
	public static List<KeyValuePair<string, BrainrotCatalogEntry>> EntriesByRarity { get; private set; } =
		new List<KeyValuePair<string, BrainrotCatalogEntry>>();

	public static Dictionary<string, BrainrotCatalogEntry> EntriesByRarityAllowedToSpawnInConveyor { get; private set; } =
		new Dictionary<string, BrainrotCatalogEntry>();

	// Cached candidates per rarity (base species allowed to spawn)
	public static Dictionary<BrainrotValueRarity, List<string>> AllowedKeysByRarity { get; private set; } =
		new Dictionary<BrainrotValueRarity, List<string>>();

	// used in conveyor spawner
	public static List<KeyValuePair<string, BrainrotCatalogEntry>> EntriesByRarityAllowedToSpawnInConveyorShuffled { get; private set; } =
		new List<KeyValuePair<string, BrainrotCatalogEntry>>();

	// Weighted rarity table for spawns (higher weight = more common)
	private static readonly System.Collections.Generic.Dictionary<BrainrotValueRarity, int> RaritySpawnWeights =
		new System.Collections.Generic.Dictionary<BrainrotValueRarity, int>
		{
			{ BrainrotValueRarity.Common, 50 },
			{ BrainrotValueRarity.Rare, 35 },
			{ BrainrotValueRarity.Epic, 10 },
			{ BrainrotValueRarity.Legendary, 4 },
			{ BrainrotValueRarity.Mythic, 1 },
			{ BrainrotValueRarity.Ethereal, 0 },
			{ BrainrotValueRarity.Primal, 0 },
		};

	// Additional per-entry weighting layered on top of rarity. Default weight = 1
	public static readonly Dictionary<string, int> EntrySpawnWeights = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
	public static void SetEntrySpawnWeight(string catalogId, int weight)
	{
		if (string.IsNullOrWhiteSpace(catalogId)) return;
		EntrySpawnWeights[catalogId] = System.Math.Max(0, weight);
	}
	public static int GetEntrySpawnWeight(string catalogId)
	{
		if (string.IsNullOrWhiteSpace(catalogId)) return 1;
		if (EntrySpawnWeights.TryGetValue(catalogId, out var w)) return System.Math.Max(0, w);
		return 1;
	}

	// Multi-target version: set desired conditional percents for several IDs within a rarity.
	// Automatically distributes the remaining probability mass uniformly across all other entries
	// in that rarity. Percent values are in [0,1].
	public static void SetConditionalPercentsWithinRarity(
		BrainrotValueRarity rarity,
		System.Collections.Generic.Dictionary<string, float> idToPercent)
	{
		if (idToPercent == null || idToPercent.Count == 0) return;
		// Collect eligible ids in this rarity
		var ids = new System.Collections.Generic.List<string>();
		foreach (var kv in EntriesByRarityAllowedToSpawnInConveyor)
		{
			if (kv.Value.Rarity == rarity) ids.Add(kv.Key);
		}
		if (ids.Count == 0) return;

		// Filter targets to those present in this rarity
		var targets = new System.Collections.Generic.Dictionary<string, float>(System.StringComparer.OrdinalIgnoreCase);
		foreach (var kv in idToPercent)
		{
			if (ids.Contains(kv.Key))
			{
				float p = kv.Value;
				if (p < 0f) p = 0f; if (p > 1f) p = 1f;
				targets[kv.Key] = p;
			}
		}
		int targetsCount = targets.Count;
		int othersCount = ids.Count - targetsCount;
		if (targetsCount == 0 && othersCount == 0) return;

		float sumP = 0f;
		foreach (var p in targets.Values) sumP += p;
		if (sumP < 0f) sumP = 0f; if (sumP > 1f) sumP = 1f;
		float othersShare = 1f - sumP;

		// Choose a scale to convert percents to integer weights
		// Aim for at least 1 for any non-zero share; keep scale moderate
		int scale = 10000;
		// Assign target weights
		foreach (var kv in targets)
		{
			int w = (int)System.Math.Round(kv.Value * scale);
			if (kv.Value > 0f && w == 0) w = 1;
			SetEntrySpawnWeight(kv.Key, w);
		}
		// Assign others uniformly
		int wOther = 0;
		if (othersCount > 0)
		{
			float perOther = othersShare / othersCount;
			if (perOther < 0f) perOther = 0f;
			wOther = (int)System.Math.Round(perOther * scale);
			if (perOther > 0f && wOther == 0) wOther = 1;
		}
		foreach (var id in ids)
		{
			if (targets.ContainsKey(id)) continue;
			SetEntrySpawnWeight(id, wOther);
		}
	}
	// // Configure per-entry weights so that, within the given rarity, the targetId has ~percent chance
	// // after the rarity has already been selected. Uses integer weights; result is approximate.
	// public static void SetConditionalPercentWithinRarity(string targetId, BrainrotValueRarity rarity, float percent)
	// {
	// 	if (string.IsNullOrWhiteSpace(targetId)) return;
	// 	if (percent <= 0f) { SetEntrySpawnWeight(targetId, 0); return; }
	// 	if (percent >= 1f) { SetEntrySpawnWeight(targetId, int.MaxValue / 4); return; }
	// 	// Collect eligible ids in this rarity
	// 	var ids = new System.Collections.Generic.List<string>();
	// 	foreach (var kv in EntriesByRarityAllowedToSpawnInConveyor)
	// 	{
	// 		if (kv.Value.Rarity == rarity) ids.Add(kv.Key);
	// 	}
	// 	if (ids.Count == 0) return;
	// 	if (!ids.Contains(targetId)) return;
	// 	int others = ids.Count - 1;
	// 	if (others <= 0) { SetEntrySpawnWeight(targetId, 1); return; }
	// 	// Choose base target weight = 1; compute uniform other weight to approximate desired percent
	// 	// percent ≈ w_t / (w_t + others * w_o) => w_o ≈ w_t * (1 - p) / (p * others)
	// 	int w_t = 1;
	// 	double w_o_f = (w_t * (1.0 - percent)) / (percent * others);
	// 	int w_o = System.Math.Max(1, (int)System.Math.Round(w_o_f));
	// 	SetEntrySpawnWeight(targetId, w_t);
	// 	foreach (var id in ids)
	// 	{
	// 		if (id.Equals(targetId, System.StringComparison.OrdinalIgnoreCase)) continue;
	// 		SetEntrySpawnWeight(id, w_o);
	// 	}
	// }



	// Stable ID maps for persistence
	public static Dictionary<int, string> StableIdToKey { get; private set; } = new Dictionary<int, string>();
	public static Dictionary<string, int> KeyToStableId { get; private set; } = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

	public static Dictionary<string, List<(string, float)>> LuckyBlockChances = new Dictionary<string, List<(string, float)>>
	{
		{ "LuckyBlock_Legendary", new List<(string, float)> {
			("chimpanzini_bananini", 0.35f),
			("ChefCrabracadabra", 0.32f),
			("BlueberriniOctopussini", 0.22f),
			("PipiPotato", 0.08f),
			("SigmaBoy", 0.03f),
		} },


		{ "LuckyBlock_Mythic", new List<(string, float)> {
			("FrigoCamelo", 0.54f),
			("zibra_zurbra_zibralini", 0.32f),
			("CavalloVirtuoso", 0.08f),
			("Lerulerulerule", 0.05f),
			("DragonCannelloni", 0.01f),
		} },

		{ "LuckyBlock_Ethereal", new List<(string, float)> {
			("FrigoCameloTetetete", 0.75f),
			("trigrrullini_watermellini_cocofanto", 0.20f),
			("CavalloVirtuosoCrabracadabra", 0.035f),
			("GanganzelliTrulalaSigma", 0.01f),
			("DragonCannelloniTracotucotulu", 0.005f),
		} },
	};

	// Roll a brainrot id from a configured lucky block list (weighted by provided chances).
	// - blockId: key in LuckyBlockChances
	// - applyServerLuck: if true, each entry chance is multiplied by GameManager.Instance.ServerLuck (clamped)
	public static string GetRandomFromLuckyBlock(string blockId, bool applyServerLuck = false)
	{
		if (string.IsNullOrWhiteSpace(blockId)) return string.Empty;
		if (!LuckyBlockChances.TryGetValue(blockId, out var list) || list == null || list.Count == 0) return string.Empty;

		float luck = 1f;
		if (applyServerLuck && GameManager.Instance != null)
		{
			luck = System.MathF.Max(0f, GameManager.Instance.ServerLuck);
			if (luck == 0f) luck = 0f; // allow disabling if desired
		}

		// Build weights (non-negative); skip unknown ids
		double total = 0.0;
		var weights = new System.Collections.Generic.List<(string id, double w)>(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			var (id, chance) = list[i];
			if (string.IsNullOrWhiteSpace(id)) continue;
			if (!Entries.ContainsKey(id)) continue; // guard against typos
			double w = System.Math.Max(0.0, (double)chance);
			if (applyServerLuck && luck != 1f)
			{
				w *= luck;
			}
			if (w <= 0.0) continue;
			weights.Add((id, w));
			total += w;
		}
		if (weights.Count == 0 || total <= 0.0)
		{
			// fallback: first valid id
			for (int i = 0; i < list.Count; i++)
			{
				if (!string.IsNullOrWhiteSpace(list[i].Item1) && Entries.ContainsKey(list[i].Item1)) return list[i].Item1;
			}
			return string.Empty;
		}

		// Weighted roll
		var seed = RNG.Seed((ulong)System.DateTime.UtcNow.Ticks);
		double roll = RNG.RangeFloat(ref seed, 0f, (float)total);
		double acc = 0.0;
		for (int i = 0; i < weights.Count; i++)
		{
			acc += weights[i].w;
			if (roll <= acc) return weights[i].id;
		}
		return weights[weights.Count - 1].id;
	}

	static BrainrotCatalog()
	{
		RegisterAllNewMemes();
		RebuildSortedIndex();
		SetConditionalPercentsWithinRarity(BrainrotValueRarity.Mythic, new Dictionary<string, float> {
			{ "DragonCannelloni", 0.01f },
			{ "LuckyBlock_Mythic", 0.01f },
		});
		SetConditionalPercentsWithinRarity(BrainrotValueRarity.Legendary, new Dictionary<string, float> {
			{ "LuckyBlock_Legendary", 0.05f },
		});
		SetConditionalPercentsWithinRarity(BrainrotValueRarity.Ethereal, new Dictionary<string, float> {
			{ "LuckyBlock_Ethereal", 0.005f },
		});
	}
	private static void RegisterAllNewMemes()
	{
		// cappuccino
		RegisterBrainrot(
			"cappuccino_assassino", "Cappuccino Assassino", BrainrotValueRarity.Epic, 75, References.CappuccinoAssassino, References.MergeSound_CappuccinoAssassino,
			"cappuccino_ballerina", "Cappuccino Ballerina", References.CappuccinoBallerina, References.MergeSoundCappuccinoBallerina,
			"cappuccino_ballerina_cacto", "Cappuccino Ballerina Cacto", References.CappuccinoBallerinaCacto, References.MergeSoundCappuccinoBallerinaCacto);

		// boneca
		RegisterBrainrot(
			"boneca_ambalabu", "Boneca Ambalabu", BrainrotValueRarity.Rare, 40, References.Boneca, References.MergeSoundBoneca,
			"boneca_burbaloni", "Boneca Burbaloni", References.BonecaBurbaloni, References.MergeSoundBonecaBurbaloni,
			"boneca_burbaloni_tata_sahur", "Boneca Burbaloni Tata Sahur", References.BonecaBurbaloniTataSahur, References.MergeSoundBonecaBurbaloniTataSahur);

		// chimpanzini
		RegisterBrainrot(
			"chimpanzini_bananini", "Chimpanzini Bananini", BrainrotValueRarity.Legendary, 300, References.ChimpanziniBananini, References.MergeSound_ChimpanziniBananini,
			"chimpanzini_bananini_watermellini", "Chimpanzini Watermellini", References.ChimpanziniBananiniWatermellini, References.MergeSoundChimpanziniBananiniWatermellini,
			"chimpanzini_bananini_cocosino", "Chimpanzini Watermellini Cocosino", References.ChimpanziniBananiniCocosino, References.MergeSoundChimpanziniBananiniCocosino);

		// ballerina
		RegisterBrainrot(
			"ballerina_cappuccina", "Ballerina Cappuccina", BrainrotValueRarity.Legendary, 500, References.BallerinaCappucina, References.MergeSound_Ballerina,
			"ballerina_BrrBrr", "Ballerina BrrBrr", References.BallerinaBrrBrr, References.MergeSoundBallerinaBrrBrr,
			"ballerina_BrrBrr_Lirili", "Ballerina BrrBrr Lirili", References.BallerinaBrrBrrLirili, References.MergeSoundBallerinaBrrBrrLirili);

		// burbaloni
		RegisterBrainrot(
			"burbaloni_lulilolli", "Burbaloni Lulilolli", BrainrotValueRarity.Legendary, 200, References.BurbaloniLulilolli, References.MergeSound_BurbaloniLuliloli,
			"burbaloni_lulilolli_bananini", "Burbaloni Lulilolli Bananini", References.BurbaloniLulilolliBananini, References.MergeSoundBurbaloniLulilolliBananini,
			"burbaloni_lulilolli_bananini_fruttodrillo", "Burbaloni Lulilolli Bananini Frutto Drillo", References.BurbaloniLulilolliBananiniFruttoDrillo, References.MergeSoundBurbaloniLulilolliBananiniFruttoDrillo);

		// cocofanto
		RegisterBrainrot(
			"cocofanto_elefanto", "Cocofanto Elefanto", BrainrotValueRarity.Rare, 60, References.CocofantoElefanto, References.MergeSound_CocofantoElephanto,
			"cocofanto_elefanto_cocosino", "Cocofanto Cocosino", References.CocofantoCocosino, References.MergeSoundCocofantoElefantoCocosino,
			"cocofanto_elefanto_cocosino_cacto", "Cocofanto Cocosino Cacto", References.CocofantoCocosinoCacto, References.MergeSoundCocofantoElefantoCocosinoCacto);

		// cocosino rhino
		RegisterBrainrot(
			"cocosino_rhino", "Cocosino Rhino", BrainrotValueRarity.Rare, 80, References.CocosinoRhino, References.MergeSoundCocosinoRhino,
			"cocosino_rhino_lirili", "Cocosino Rhino Lirili", References.CocosinoRhinoLirili, References.MergeSoundCocosinoRhinoLirili,
			"cocosino_rhino_lirili_fruttodrillo", "Cocosino Rhino Lirili Frutto Drillo", References.CocosinoRhinoLiriliFruttoDrillo, References.MergeSoundCocosinoRhinoLiriliFruttoDrillo);

		// glorbo
		RegisterBrainrot(
			"glorbo_fruttodrillo", "Glorbo Fruttodrillo", BrainrotValueRarity.Legendary, 750, References.GlorboFruttodrillo, References.MergeSoundGlorbo,
			"glorbo_fruttodrillo_lulilolli", "Glorbo Fruttodrillo Lulilolli", References.GlorboFruttodrilloLulilolli, References.MergeSoundGlorboFruttodrilloLulilolli,
			"glorbo_fruttodrillo_lulilolli_cappucina", "Glorbo Fruttodrillo Lulilolli Cappucina", References.GlorboFruttodrilloLulilolliCappucina, References.MergeSoundGlorboFruttodrilloLulilolliCappucina);

		// il cacto
		RegisterBrainrot(
			"il_cacto_hipopotamo", "Il Cacto Hipopotamo", BrainrotValueRarity.Rare, 50, References.IlCactoHipopotamo, References.MergeSoundIlCacto,
			"il_cacto_hipopotamo_bananini", "Il Cacto Hipopotamo Bananini", References.IlCactoHipopotamoBananini, References.MergeSoundIlCactoHipopotamoBananini,
			"il_cacto_hipopotamo_bananini_tungtung", "Il Cacto Hipopotamo Bananini Tungtung", References.IlCactoHipopotamoBananiniTungtung, References.MergeSoundIlCactoHipopotamoBananiniTungtung);

		// lirili
		RegisterBrainrot(
			"lirili_larila", "Lirili Larila", BrainrotValueRarity.Common, 3, References.LiriliLarila, References.MergeSoundLirilli,
			"lirili_larila_ambalabu", "Lirili Larila Ambalabu", References.LiriliLarilaAmbalabu, References.MergeSoundLiriliLarilaAmbalabu,
			"lirili_larila_ambalabu_tata_sahur", "Lirili Larila Ambalabu Tata Sahur", References.LiriliLarilaAmbalabuTataSahur, References.MergeSoundLiriliLarilaAmbalabuTataSahur);

		// ta_ta_sahur
		RegisterBrainrot(
			"ta_ta_sahur", "Ta Ta Ta Ta Ta Sahur", BrainrotValueRarity.Rare, 55, References.TataSahur, References.MergeSoundTatatata,
			"ta_ta_sahur_larila", "Ta Ta Sahur Larila", References.TataSahurLarila, References.MergeSoundTaTaSahurLarila,
			"ta_ta_sahur_larila_zibra_zubra", "Ta Ta Sahur Larila Zibra Zubra", References.TataSahurLarilaZibraZubra, References.MergeSoundTaTaSahurLarilaZibraZubra);

		// trigrrullini
		RegisterBrainrot(
			"trigrrullini_watermellini", "Trigrrullini Watermellini", BrainrotValueRarity.Mythic, 6500, References.TrigrrulliniWatermellini, References.MergeSoundTigrallini,
			"trigrrullini_watermellini_cocofanto", "Tigrrullini Watermellini Cocofanto", References.TrigrrulliniWatermelliniCocofanto, References.MergeSoundTrigrrulliniWatermelliniCocofanto,
			"trigrrullini_watermellini_cocofanto_cappucino", "Trigrrullini Watermellini Cocofanto Cappucino", References.TrigrrulliniWatermelliniCocofantoCappucino, References.MergeSoundTrigrrulliniWatermelliniCocofantoCappucino);

		// tung_tung
		RegisterBrainrot(
			"tung_tung", "Tung Tung", BrainrotValueRarity.Rare, 25, References.TungTung, References.MergeSoundTungTung,
			"tung_tung_bananini", "Tung Tung Bananini", References.TungTungBananini, References.MergeSoundTungTungBananini,
			"tung_tung_bananini_watermeloni", "Tung Tung Bananini Watermeloni", References.TungTungBananiniWatermeloni, References.MergeSoundTungTungBananiniWatermeloni);

		// zibra
		RegisterBrainrot(
			"zibra_zurbra_zibralini", "Zibra Zurbra Zibralini", BrainrotValueRarity.Mythic, 6000, References.ZibraZurbraZibralini, References.MergeSoundZibra,
			"zibra_zurbra_zibralini_patapim", "Zibra Patapim", References.ZibraZurbraZibraliniPatapim, References.MergeSoundZibraZurbraZibraliniPatapim,
			"zibra_zurbra_zibralini_patapim_assassino", "Zibra Patapim Assassino", References.ZibraZurbraZibraliniPatapimAssassino, References.MergeSoundZibraZurbraZibraliniPatapimAssassino);

		// brrbrr
		RegisterBrainrot(
			"brrbrr_patapim", "BrrBrr Patapim", BrainrotValueRarity.Epic, 100, References.BrrBrrPatapim, References.MergeSoundBrrBrrPatapim,
			"brrbrr_tungung", "BrrBrr TungTung", References.BrrBrrTungTung, References.MergeSoundBrrBrrTungTung,
			"brrbrr_tungung_zibra_zubra", "BrrBrr TungTung Zibra Zubra", References.BrrBrrTungTungZibraZubra, References.MergeSoundBrrBrrTungTungZibraZubra);

		// karkerkar
		RegisterBrainrot(
			"karkerkar_kurkur", "Karkerkar Kurkur", BrainrotValueRarity.Mythic, 7000, References.KarkerkarKurkur, References.MergeSoundKarkerkarKurkur,
			"karkerkar_kurkur_ambalabu", "Karkerkar Kurkur Ambalabu", References.KarkerkarKurkurAmbalabu, References.MergeSoundKarkerkarKurkurAmbalabu,
			"karkerkar_kurkur_ambalabu_tungtung", "Karkerkar Kurkur Ambalabu Tungtung", References.KarkerkarKurkurAmbalabuTungtung, References.MergeSoundKarkerkarKurkurAmbalabuTungtung);

		// Avocadini Antilopini
		RegisterBrainrot(
			"AvocadiniAntilopini", "Avocadini Antilopini", BrainrotValueRarity.Epic, 115, References.AvocadiniAntilopini, References.MergeSound_Avocadini_Antilopini,
			"AvocadiniAntilopiniTrulala", "Avocadini Antilopini Trulala", References.AvocadiniAntilopiniTrulala, References.MergeSound_Avocadini_Antilopini_Trulala,
			"AvocadiniAntilopiniTrulalaToasterino", "Avocadini Antilopini Trulala Toasterino", References.AvocadiniAntilopiniTrulalaToasterino, References.MergeSound_Avocadini_Antilopini_Trulala_Toasterino);

		// Avocadini Guffo
		RegisterBrainrot(
			"AvocadiniGuffo", "Avocadini Guffo", BrainrotValueRarity.Epic, 225, References.AvocadiniGuffo, References.MergeSound_Avocadini_Guffo,
			"AvocadiniGuffoBrriBri", "Avocadini Guffo BrriBri", References.AvocadiniGuffoBrriBri, References.MergeSound_Avocadini_Guffo_BrriBri,
			"AvocadiniGuffoBrriBriTititi", "Avocadini Guffo BrriBri Tititi", References.AvocadiniGuffoBrriBriTititi, References.MergeSound_Avocadini_Guffo_BrriBri_Tititi);

		// Avocadorilla
		RegisterBrainrot(
			"Avocadorilla", "Avocadorilla", BrainrotValueRarity.Legendary, 1200, References.Avocadorilla, References.MergeSound_Avocadorilla,
			"AvocadorillaPipi", "Avocadorilla Pipi", References.AvocadorillaPipi, References.MergeSound_Avocadorilla_Pipi,
			"AvocadorillaPipiTricTrac", "Avocadorilla Pipi Tric Trac", References.AvocadorillaPipiTricTrac, References.MergeSound_Avocadorilla_Pipi_Tric_Trac);

		// Bambini Crostini
		RegisterBrainrot(
			"BambiniCrostini", "Bambini Crostini", BrainrotValueRarity.Epic, 135, References.BambiniCrostini, References.MergeSound_Bambini_Crostini,
			"BambiniCrostiniSalamini", "Bambini Crostini Salamini", References.BambiniCrostiniSalamini, References.MergeSound_Bambini_Crostini_Salamini,
			"BambiniCrostiniSalaminiTititi", "Bambini Crostini Salamini Tititi", References.BambiniCrostiniSalaminiTititi, References.MergeSound_Bambini_Crostini_Salamini_Tititi);

		// Bananita Dolphinita
		RegisterBrainrot(
			"BananitaDolphinita", "Bananita Dolphinita", BrainrotValueRarity.Epic, 150, References.BananitaDolphinita, References.MergeSound_Bananita_Dolphinita,
			"BananitaDolphinitaCamelo", "Bananita Dolphinita Camelo", References.BananitaDolphinitaCamelo, References.MergeSound_Bananita_Dolphinita_Camelo,
			"BananitaDolphinitaCameloTrulicina", "Bananita Dolphinita Camelo Trulicina", References.BananitaDolphinitaCameloTrulicina, References.MergeSound_Bananita_Dolphinita_Camelo_Trulicina);

		// Blueberrini Octopussini
		RegisterBrainrot(
			"BlueberriniOctopussini", "Blueberrini Octopussini", BrainrotValueRarity.Legendary, 1000, References.BlueberriniOctopussini, References.MergeSound_Blueberrini_Octopussini,
			"BlueberriniOctopussiniKurkur", "Blueberrini Octopussini Kurkur", References.BlueberriniOctopussiniKurkur, References.MergeSound_Blueberrini_Octopussini_Kurkur,
			"BlueberriniOctopussiniKurkurOrangutini", "Blueberrini Octopussini Kurkur Orangutini", References.BlueberriniOctopussiniKurkurOrangutini, References.MergeSound_Blueberrini_Octopussini_Kurkur_Orangutini);

		// Bobrito Bandito
		RegisterBrainrot(
			"BobritoBandito", "Bobrito Bandito", BrainrotValueRarity.Rare, 35, References.BobritoBandito, References.MergeSound_Bobrito_Bandito,
			"BobritoBanditoTobTobi", "Bobrito Bandito Tob Tobi", References.BobritoBanditoTobTobi, References.MergeSound_Bobrito_Bandito_Tob_Tobi,
			"BobritoBanditoTobTobiCrabracada", "Bobrito Bandito Tob Tobi Crabracada", References.BobritoBanditoTobTobiCrabracada, References.MergeSound_Bobrito_Bandito_Tob_Tobi_Crabracada);

		// Brri Brri Bicus
		RegisterBrainrot(
			"BrriBrriBicus", "Brri Brri Bicus", BrainrotValueRarity.Epic, 175, References.BrriBrriBicus, References.MergeSound_Brri_Brri_Bicus,
			"BrriBrriBicusTigrullini", "Brri Brri Bicus Tigrullini", References.BrriBrriBicusTigrullini, References.MergeSound_Brri_Brri_Bicus_Tigrullini,
			"BrriBrriBicusTigrulliniTungTung", "Brri Brri Bicus Tigrullini TungTung", References.BrriBrriBicusTigrulliniTungTung, References.MergeSound_Brri_Brri_Bicus_Tigrullini_TungTung);

		// Cavallo Virtuoso
		RegisterBrainrot(
			"CavalloVirtuoso", "Cavallo Virtuoso", BrainrotValueRarity.Mythic, 7500, References.CavalloVirtuoso, References.MergeSound_Cavallo_Virtuoso,
			"CavalloVirtuosoCrabracadabra", "Cavallo Virtuoso Crabracadabra", References.CavalloVirtuosoCrabracadabra, References.MergeSound_Cavallo_Virtuoso_Crabracadabra,
			"CavalloVirtuosoCrabracadabraTetete", "Cavallo Virtuoso Crabracadabra Tetete", References.CavalloVirtuosoCrabracadabraTetete, References.MergeSound_Cavallo_Virtuoso_Crabracadabra_Tetete);

		// Chef Crabracadabra
		RegisterBrainrot(
			"ChefCrabracadabra", "Chef Crabracadabra", BrainrotValueRarity.Legendary, 600, References.ChefCrabracadabra, References.MergeSound_Chef_Crabracadabra,
			"ChefCrabracadabraLirili", "Chef Crabracadabra Lirili", References.ChefCrabracadabraLirili, References.MergeSound_Chef_Crabracadabra_Lirili,
			"ChefCrabracadabraLiriliGangster", "Chef Crabracadabra Lirili Gangster", References.ChefCrabracadabraLiriliGangster, References.MergeSound_Chef_Crabracadabra_Lirili_Gangster);

		// Cocossini Mama
		RegisterBrainrot(
			"CocossiniMama", "Cocossini Mama", BrainrotValueRarity.Legendary, 1200, References.CocossiniMama, References.MergeSound_Cocossini_Mama,
			"CocossiniMamaStrawberelli", "Cocossini Mama Strawberelli", References.CocossiniMamaStrawberelli, References.MergeSound_Cocossini_Mama_Strawberelli,
			"CocossiniMamaStrawberelliAssassino", "Cocossini Mama Strawberelli Assassino", References.CocossiniMamaStrawberelliAssassino, References.MergeSound_Cocossini_Mama_Strawberelli_Assassino);

		// Dragon Cannelloni
		RegisterBrainrot(
			"DragonCannelloni", "Dragon Cannelloni", BrainrotValueRarity.Mythic, 12500, References.DragonCannelloni, References.MergeSound_Dragon_Cannelloni,
			"DragonCannelloniTracotucotulu", "Dragon Cannelloni Tracotucotulu", References.DragonCannelloniTracotucotulu, References.MergeSound_Dragon_Cannelloni_Tracotucotulu,
			"DragonCannelloniTracotucotuluPotato", "Dragon Cannelloni Tracotucotulu Potato", References.DragonCannelloniTracotucotuluPotato, References.MergeSound_Dragon_Cannelloni_Tracotucotulu_Potato,
			onSpawn: (b) =>
			{
				// if (b.State == Brainrot.BrainrotState.InConveyor)
				{
					SFX.Play(References.MergeSound_Dragon_Cannelloni, new SFX.PlaySoundDesc { Positional = false, Volume = 0.7f });
				}
			});

		// Fluriflura
		RegisterBrainrot(
			"Fluriflura", "Fluriflura", BrainrotValueRarity.Common, 7, References.Fluriflura, References.MergeSound_Fluriflura,
			"FlurifluraTim", "Fluriflura Tim", References.FlurifluraTim, References.MergeSound_Fluriflura_Tim,
			"FlurifluraTimDindin", "Fluriflura Tim Dindin", References.FlurifluraTimDindin, References.MergeSound_Fluriflura_Tim_Dindin);

		// Frigo Camelo
		RegisterBrainrot(
			"FrigoCamelo", "Frigo Camelo", BrainrotValueRarity.Mythic, 1400, References.FrigoCamelo, References.MergeSound_Frigo_Camelo,
			"FrigoCameloTetetete", "Frigo Camelo Tetetete", References.FrigoCameloTetetete, References.MergeSound_Frigo_Camelo_Tetetete,
			"FrigoCameloTeteteteRhino", "Frigo Camelo Tetetete Rhino", References.FrigoCameloTeteteteRhino, References.MergeSound_Frigo_Camelo_Tetetete_Rhino,
			onSpawn: (b) =>
			{
			});

		// Ganganzelli Trulala
		RegisterBrainrot(
			"GanganzelliTrulala", "Ganganzelli Trulala", BrainrotValueRarity.Mythic, 9000, References.GanganzelliTrulala, References.MergeSound_Ganganzelli_Trulala,
			"GanganzelliTrulalaSigma", "Ganganzelli Trulala Sigma", References.GanganzelliTrulalaSigma, References.MergeSound_Ganganzelli_Trulala_Sigma,
			"GanganzelliTrulalaSigmaTricTrac", "Ganganzelli Trulala Sigma Tric Trac", References.GanganzelliTrulalaSigmaTricTrac, References.MergeSound_Ganganzelli_Trulala_Sigma_Tric_Trac);

		// Gangster Footera
		RegisterBrainrot(
			"GangsterFootera", "Gangster Footera", BrainrotValueRarity.Rare, 30, References.GangsterFootera, References.MergeSound_Gangster_Footera,
			"GangsterFooteraTungTung", "Gangster Footera TungTung", References.GangsterFooteraTungTung, References.MergeSound_Gangster_Footera_TungTung,
			"GangsterFooteraTungTungKurkur", "Gangster Footera TungTung Kurkur", References.GangsterFooteraTungTungKurkur, References.MergeSound_Gangster_Footera_TungTung_Kurkur);

		// Gorillo
		RegisterBrainrot(
			"GorilloWatermellondrillo", "Gorillo Watermellondrillo", BrainrotValueRarity.Mythic, 8000, References.GorilloWatermellondrillo, References.MergeSound_Gorillo_Watermellondrillo,
			"GorilloWatermellondrilloLemonchello", "Gorillo Watermellondrillo Lemonchello", References.GorilloLemonchello, References.MergeSound_Gorillo_Watermellondrillo_Lemonchello,
			"GorilloWatermellondrilloLemonchelloBlueberrini", "Gorillo Watermellondrillo Lemonchello Blueberrini", References.GorilloLemonchelloBlueberrini, References.MergeSound_Gorillo_Watermellondrillo_Lemonchello_Blueberrini);

		// Lerulerulerule
		RegisterBrainrot(
			"Lerulerulerule", "Lerulerulerule", BrainrotValueRarity.Mythic, 8700, References.Lerulerulerule, References.MergeSound_Lerulerulerule,
			"LeruleruleruleBrrBrr", "Lerulerulerule BrrBrr", References.LeruleruleruleBrrBrr, References.MergeSound_Lerulerulerule_BrrBrr,
			"LeruleruleruleBrrBrrTroppi", "Lerulerulerule BrrBrr Troppi", References.LeruleruleruleBrrBrrTroppi, References.MergeSound_Lerulerulerule_BrrBrr_Troppi);

		// Lionel Cactuseli
		RegisterBrainrot(
			"LionelCactuseli", "Lionel Cactuseli", BrainrotValueRarity.Legendary, 650, References.LionelCactuseli, References.MergeSound_Lionel_Cactuseli,
			"LionelCactuseliPipiWatermelon", "Lionel Cactuseli PipiWatermelon", References.LionelCactuseliPipiWatermelon, References.MergeSound_Lionel_Cactuseli_PipiWatermelon,
			"LionelCactuseliPipiWatermelonDiFero", "Lionel Cactuseli PipiWatermelon DiFero", References.LionelCactuseliPipiWatermelonDiFero, References.MergeSound_Lionel_Cactuseli_PipiWatermelon_DiFero);

		// Orangutini Ananasini
		RegisterBrainrot(
			"OrangutiniAnanasini", "Orangutini Ananasini", BrainrotValueRarity.Mythic, 1700, References.OrangutiniAnanasini, References.MergeSound_Orangutini_Ananasini,
			"OrangutiniAnanasiniCocosino", "Orangutini Ananasini Cocosino", References.OrangutiniAnanasiniCocosino, References.MergeSound_Orangutini_Ananasini_Cocosino,
			"OrangutiniAnanasiniCocosinoFlamingelli", "Orangutini Ananasini Cocosino Flamingelli", References.OrangutiniAnanasiniCocosinoFlamingelli, References.MergeSound_Orangutini_Ananasini_Cocosino_Flamingelli);

		// Pandaccini Bananini
		RegisterBrainrot(
			"PandacciniBananini", "Pandaccini Bananini", BrainrotValueRarity.Legendary, 1200, References.PandacciniBananini, References.MergeSound_Pandaccini_Bananini,
			"PandacciniBananiniPotato", "Pandaccini Bananini Potato", References.PandacciniBananiniPotato, References.MergeSound_Pandaccini_Bananini_Potato,
			"PandacciniBananiniPotatoQuivioli", "Pandaccini Bananini Potato Quivioli", References.PandacciniBananiniPotatoQuivioli, References.MergeSound_Pandaccini_Bananini_Potato_Quivioli);

		// Penguino Cocosino
		RegisterBrainrot(
			"PenguinoCocosino", "Penguino Cocosino", BrainrotValueRarity.Epic, 300, References.PenguinoCocosino, References.MergeSound_Penguino_Cocosino,
			"PenguinoCocosinoSigma", "Penguino Cocosino Sigma", References.PenguinoCocosinoSigma, References.MergeSound_Penguino_Cocosino_Sigma,
			"PenguinoCocosinoSigmaSalamino", "Penguino Cocosino Sigma Salamino", References.PenguinoCocosinoSigmaSalamino, References.MergeSound_Penguino_Cocosino_Sigma_Salamino);

		// Perochello Lemonchello
		RegisterBrainrot(
			"PerochelloLemonchello", "Perochello Lemonchello", BrainrotValueRarity.Epic, 160, References.PerochelloLemonchello, References.MergeSound_PerochHello_Lemonchello,
			"PerochelloLemonchelloTroppi", "Perochello Lemonchello Troppi", References.PerochelloLemonchelloTroppi, References.MergeSound_Perochello_Lemonchello_Troppi,
			"PerochelloLemonchelloTroppiAmbalabu", "Perochello Lemonchello Troppi Ambalabu", References.PerochelloLemonchelloTroppiAmbalabu, References.MergeSound_Perochello_Lemonchello_Troppi_Ambalabu);

		// Pipi Avocado
		RegisterBrainrot(
			"PipiAvocado", "Pipi Avocado", BrainrotValueRarity.Rare, 70, References.PipiAvocado, References.MergeSound_Pipi_Avocado,
			"PipiAvocadoToasterino", "Pipi Avocado Toasterino", References.PipiAvocadoToasterino, References.MergeSound_Pipi_Avocado_Toasterino,
			"PipiAvocadoToasterinoBombardino", "Pipi Avocado Toasterino Bombardino", References.PipiAvocadoToasterinoBombardino, References.MergeSound_Pipi_Avocado_Toasterino_Bombardino);

		// Pipi Corni
		RegisterBrainrot(
			"PipiCorni", "Pipi Corni", BrainrotValueRarity.Common, 14, References.PipiCorni, References.MergeSound_Pipi_Corni,
			"PipiCorniTim", "Pipi Corni Tim", References.PipiCorniTim, References.MergeSound_Pipi_Corni_Tim,
			"PipiCorniTimTracotucotulu", "Pipi Corni Tim Tracotucotulu", References.PipiCorniTimTracotucotulu, References.MergeSound_Pipi_Corni_Tim_Tracotucotulu);

		// Pipi Kiwi
		RegisterBrainrot(
			"PipiKiwi", "Pipi Kiwi", BrainrotValueRarity.Common, 13, References.PipiKiwi, References.MergeSound_Pipi_Kiwi,
			"PipiKiwiTetetete", "Pipi Kiwi Tetetete", References.PipiKiwiTetetete, References.MergeSound_Pipi_Kiwi_Tetetete,
			"PipiKiwiTeteteteTobTobi", "Pipi Kiwi Tetetete Tob Tobi", References.PipiKiwiTeteteteTobTobi, References.MergeSound_Pipi_Kiwi_Tetetete_Tob_Tobi);

		// Pipi Potato
		RegisterBrainrot(
			"PipiPotato", "Pipi Potato", BrainrotValueRarity.Legendary, 1100, References.PipiPotato, References.MergeSound_Pipi_Potato,
			"PipiPotatoPipi", "Pipi Potato Pipi", References.PipiPotatoPipi, References.MergeSound_Pipi_Potato_Pipi,
			"PipiPotatoPipiTatata", "Pipi Potato Pipi Tatata", References.PipiPotatoPipiTatata, References.MergeSound_Pipi_Potato_Pipi_Tatata);

		// Pipi Watermelon
		RegisterBrainrot(
			"PipiWatermelon", "Pipi Watermelon", BrainrotValueRarity.Legendary, 1300, References.PipiWatermelon, References.MergeSound_Pipi_Watermelon,
			"PipiWatermelonQuivioli", "Pipi Watermelon Quivioli", References.PipiWatermelonQuivioli, References.MergeSound_Pipi_Watermelon_Quivioli,
			"PipiWatermelonQuivioliTrulicina", "Pipi Watermelon Quivioli Trulicina", References.PipiWatermelonQuivioliTrulicina, References.MergeSound_Pipi_Watermelon_Quivioli_Trulicina);

		// Quivioli Ameleonni
		RegisterBrainrot(
			"QuivioliAmeleonni", "Quivioli Ameleonni", BrainrotValueRarity.Legendary, 900, References.QuivioliAmeleonni, References.MergeSound_Quivioli_Ameleonni,
			"QuivioliAmeleonniBombardino", "Quivioli Ameleonni Bombardino", References.QuivioliAmeleonniBombardino, References.MergeSound_Quivioli_Ameleonni_Bombardino,
			"QuivioliAmeleonniBombardinoOctopussini", "Quivioli Ameleonni Bombardino Octopussini", References.QuivioliAmeleonniBombardinoOctopussini, References.MergeSound_Quivioli_Ameleonni_Bombardino_Octopussini);

		// SSundee Ambalabu
		RegisterBrainrot(
			"SSundeeAmbalabu", "SSundee Ambalabu", BrainrotValueRarity.Common, 12, References.SSundeeAmbalabu, References.MergeSound_SSundee_Ambalabu,
			"SSundeeAmbalabuTroppa", "SSundee Ambalabu Troppa", References.SSundeeAmbalabuTroppa, References.MergeSound_SSundee_Ambalabu_Troppa,
			"SSundeeAmbalabuTroppaSushi", "SSundee Ambalabu Troppa Sushi", References.SSundeeAmbalabuTroppaSushi, References.MergeSound_SSundee_Ambalabu_Troppa_Sushi);

		// Rhino Toasterino
		RegisterBrainrot(
			"RhinoToasterino", "Rhino Toasterino", BrainrotValueRarity.Mythic, 2100, References.RhinoToasterino, References.MergeSound_Rhino_Toasterino,
			"RhinoToasterinoGangster", "Rhino Toasterino Gangster", References.RhinoToasterinoGangster, References.MergeSound_Rhino_Toasterino_Gangster,
			"RhinoToasterinoGangsterTroppi", "Rhino Toasterino Gangster Troppi", References.RhinoToasterinoGangsterTroppi, References.MergeSound_Rhino_Toasterino_Gangster_Troppi);

		// Salamino Penguino
		RegisterBrainrot(
			"SalaminoPenguino", "Salamino Penguino", BrainrotValueRarity.Epic, 250, References.SalaminoPenguino, References.MergeSound_Salamino_Penguino,
			"SalaminoPenguinoSigma", "Salamino Penguino Sigma", References.SalaminoPenguinoSigma, References.MergeSound_Salamino_Penguino_Sigma,
			"SalaminoPenguinoSigmaTricTrac", "Salamino Penguino Sigma TricTrac", References.SalaminoPenguinoSigmaTricTrac, References.MergeSound_Salamino_Penguino_Sigma_TricTrac);

		// Schleemerini Monsterini
		RegisterBrainrot(
			"SchleemeriniMonsterini", "Schleemerini Monsterini", BrainrotValueRarity.Common, 1, References.SchleemeriniMonsterini, References.MergeSound_Schleemerini_Monsterini,
			"SchleemeriniMonsteriniGangster", "Schleemerini Monsterini Gangster", References.SchleemeriniMonsteriniGangster, References.MergeSound_Schleemerini_Monsterini_Gangster,
			"SchleemeriniMonsteriniGangsterChef", "Schleemerini Monsterini Gangster Chef", References.SchleemeriniMonsteriniGangsterChef, References.MergeSound_Schleemerini_Monsterini_Gangster_Chef);

		// Sigma Boy
		RegisterBrainrot(
			"SigmaBoy", "Sigma Boy", BrainrotValueRarity.Legendary, 1300, References.SigmaBoy, References.MergeSound_Sigma_Boy,
			"SigmaBoyStrawberini", "Sigma Boy Strawberini", References.SigmaBoyStrawberini, References.MergeSound_Sigma_Boy_Strawberini,
			"SigmaBoyStrawberiniCraberini", "Sigma Boy Strawberini Craberini", References.SigmaBoyStrawberiniCraberini, References.MergeSound_Sigma_Boy_Strawberini_Craberini);

		// Sixtee Seven
		RegisterBrainrot(
			"SixteeSeven", "Sixtee Seven", BrainrotValueRarity.Mythic, 13000, References.SixteeSeven, References.MergeSound_Sixtee_Seven,
			"SixteeSevenCannelloni", "Sixtee Seven Cannelloni", References.SixteeSevenCannelloni, References.MergeSound_Sixtee_Seven_Cannelloni,
			"SixteeSevenCannelloniStrawberini", "Sixtee Seven Cannelloni Strawberini", References.SixteeSevenCannelloniStrawberini, References.MergeSound_Sixtee_Seven_Cannelloni_Strawberini);

		// Smurfcat
		RegisterBrainrot(
			"Smurfcat", "Smurfcat", BrainrotValueRarity.Epic, 100, References.Smurfcat, References.MergeSound_Smurfcat,
			"SmurfcatBandito", "Smurfcat Bandito", References.SmurfcatBandito, References.MergeSound_Smurfcat_Bandito,
			"SmurfcatBanditoAssassino", "Smurfcat Bandito assassino", References.SmurfcatBanditoAssassino, References.MergeSound_Smurfcat_Bandito_assassino);

		// Spijuniro Golubiro
		RegisterBrainrot(
			"SpijuniroGolubiro", "Spijuniro Golubiro", BrainrotValueRarity.Mythic, 3500, References.SpijuniroGolubiro, References.MergeSound_Spijuniro_Golubiro,
			"SpijuniroGolubiroPipi", "Spijuniro Golubiro Pipi", References.SpijuniroGolubiroPipi, References.MergeSound_Spijuniro_Golubiro_Pipi,
			"SpijuniroGolubiroPipAmbalabu", "Spijuniro Golubiro Pip Ambalabu", References.SpijuniroGolubiroPipAmbalabu, References.MergeSound_Spijuniro_Golubiro_Pip_Ambalabu);

		// Strawberelli Flamingelli
		RegisterBrainrot(
			"StrawberelliFlamingelli", "Strawberelli Flamingelli", BrainrotValueRarity.Legendary, 1100, References.StrawberelliFlamingelli, References.MergeSound_Strawberelli_Flamingelli,
			"StrawberelliFlamingelliOrangutini", "Strawberelli Flamingelli Orangutini", References.StrawberelliFlamingelliOrangutini, References.MergeSound_Strawberelli_Flamingelli_Orangutini,
			"StrawberelliFlamingelliOrangutiniKiwi", "Strawberelli Flamingelli Orangutini Kiwi", References.StrawberelliFlamingelliOrangutiniKiwi, References.MergeSound_Strawberelli_Flamingelli_Orangutini_Kiwi);

		// Svinina Bombardino
		RegisterBrainrot(
			"SvininaBombardino", "Svinina Bombardino", BrainrotValueRarity.Common, 10, References.SvininaBombardino, References.MergeSound_Svinina_Bombardino,
			"SvininaBombardinoTititi", "Svinina Bombardino Tititi", References.SvininaBombardinoTititi, References.MergeSound_Svinina_Bombardino_Tititi,
			"SvininaBombardinoTititiTetetete", "Svinina Bombardino Tititi Tetetete", References.SvininaBombardinoTititiTetetete, References.MergeSound_Svinina_Bombardino_Tititi_Tetetete);

		// Talpa di Fero
		RegisterBrainrot(
			"TalpaDiFero", "Talpa di Fero", BrainrotValueRarity.Common, 9, References.TalpaDiFero, References.MergeSound_Talpa_di_Fero,
			"TalpaDiFeroCorni", "Talpa di Fero Corni", References.TalpaDiFeroCorni, References.MergeSound_Talpa_di_Fero_Corni,
			"TalpaDiFeroCorniWatermeloni", "Talpa di Fero Corni Watermeloni", References.TalpaDiFeroCorniWatermeloni, References.MergeSound_Talpa_di_Fero_Corni_Watermeloni);

		// Te Te Te Sahur
		RegisterBrainrot(
			"TeTeTeTeTeSahur", "Te Te Te Te Te Sahur", BrainrotValueRarity.Mythic, 9500, References.TeTeTeTeTeSahur, References.MergeSound_Te_Te_Te_Te_Te_Sahur,
			"TeteteteSahurTitititi", "Tetetete Sahur Titititi", References.TeteteteSahurTitititi, References.MergeSound_Tetetete_Sahur_Titititi,
			"TeteteteSahurTitititiDindin", "Tetetete Sahur Titititi Dindin", References.TeteteteSahurTitititiDindin, References.MergeSound_Tetetete_Sahur_Titititi_Dindin);

		// Ti Ti Ti Sahur
		RegisterBrainrot(
			"TiTiTiSahur", "Ti Ti Ti Sahur", BrainrotValueRarity.Epic, 225, References.TiTiTiSahur, References.MergeSound_Ti_Ti_Ti_Sahur,
			"TiTiTiSahurLuliloli", "Ti Ti Ti Sahur Luliloli", References.TiTiTiSahurLuliloli, References.MergeSound_Ti_Ti_Ti_Sahur_Luliloli,
			"TiTiTiSahurLuliloliTungtung", "Ti Ti Ti Sahur Luliloli Tungtung", References.TiTiTiSahurLuliloliTungtung, References.MergeSound_Ti_Ti_Ti_Sahur_Luliloli_Tungtung);

		// Tim Cheese
		RegisterBrainrot(
			"TimCheese", "Tim Cheese", BrainrotValueRarity.Common, 5, References.TimCheese, References.MergeSound_Tim_Cheese,
			"TimCheeseTatatata", "Tim Cheese Tatatata", References.TimCheeseTatatata, References.MergeSound_Tim_Cheese_Tatatata,
			"TimCheeseTatatataGangster", "Tim Cheese Tatatata Gangster", References.TimCheeseTatatataGangster, References.MergeSound_Tim_Cheese_Tatatata_Gangster);

		// Tob Tobi Tobi
		RegisterBrainrot(
			"TobTobiTobi", "Tob Tobi Tobi", BrainrotValueRarity.Mythic, 8500, References.TobTobiTobi, References.MergeSound_Tob_Tobi_Tobi,
			"TobTobiTobiTricTrac", "Tob Tobi Tobi Tric Trac", References.TobTobiTobiTricTrac, References.MergeSound_Tob_Tobi_Tobi_Tric_Trac,
			"TobTobiTobiTricTracAvocadini", "Tob Tobi Tobi Tric Trac Avocadini", References.TobTobiTobiTricTracAvocadini, References.MergeSound_Tob_Tobi_Tobi_Tric_Trac_Avocadini);

		// Tracotucotulu Delapeladustuz
		RegisterBrainrot(
			"TracotucotuluDelapeladustuz", "Tracotucotulu Delapeladustuz", BrainrotValueRarity.Mythic, 12000, References.TracotucotuluDelapeladustuz, References.MergeSound_Tracotucotulu_Delapeladustuz,
			"TracotucotuluDelapeladustuzTroppi", "Tracotucotulu Delapeladustuz Troppi", References.TracotucotuluDelapeladustuzTroppi, References.MergeSound_Tracotucotulu_Delapeladustuz_Troppi,
			"TracotucotuluDelapeladustuzTroppiTrulicina", "Tracotucotulu Delapeladustuz Troppi Trulicina", References.TracotucotuluDelapeladustuzTroppiTrulicina, References.MergeSound_Tracotucotulu_Delapeladustuz_Troppi_Trulicina);

		// Tric Trac Baraboom
		RegisterBrainrot(
			"TricTracBaraboom", "Tric Trac Baraboom", BrainrotValueRarity.Rare, 65, References.TricTracBaraboom, References.MergeSound_Tric_Trac_Baraboom,
			"TricTracBaraboomGangsta", "Tric Trac Baraboom Gangsta", References.TricTracBaraboomGangsta, References.MergeSound_Tric_Trac_Baraboom_Gangsta,
			"TricTracBaraboomGangstaWatermelon", "Tric Trac Baraboom Gangsta Watermelon", References.TricTracBaraboomGangstaWatermelon, References.MergeSound_Tric_Trac_Baraboom_Gangsta_Watermelon);

		// Trippi Troppi
		RegisterBrainrot(
			"TrippiTroppi", "Trippi Troppi", BrainrotValueRarity.Rare, 15, References.TrippiTroppi, References.MergeSound_Trippi_Troppi,
			"TrippiTroppiSmurf", "Trippi Troppi Smurf", References.TrippiTroppiSmurf, References.MergeSound_Trippi_Troppi_Smurf,
			"TrippiTroppiSmurfLemonchello", "Trippi Troppi Smurf Lemonchello", References.TrippiTroppiSmurfLemonchello, References.MergeSound_Trippi_Troppi_Smurf_Lemonchello);

		// Trulimero Trulicina
		RegisterBrainrot(
			"TrulimeroTrulicina", "Trulimero Trulicina", BrainrotValueRarity.Epic, 125, References.TrulimeroTrulicina, References.MergeSound_Trulimero_Trulicina,
			"TrulimeroTrulicinaCrostini", "Trulimero Trulicina Crostini", References.TrulimeroTrulicinaCrostini, References.MergeSound_Trulimero_Trulicina_Crostini,
			"TrulimeroTrulicinaCrostiniOrangutini", "Trulimero Trulicina Crostini Orangutini", References.TrulimeroTrulicinaCrostiniOrangutini, References.MergeSound_Trulimero_Trulicina_Crostini_Orangutini);

		// Trulimero Trulicina
		RegisterBrainrot(
			"DinDinDinDin", "Udin Din Din Dun", BrainrotValueRarity.Epic, 400, References.Udindindin, References.MergeSound_Udin_Din_Din_Dun,
			"DinDinDinDin_Tric_Trac", "Udin Din Din Dun Tric Trac", References.UdindindinTricTrac, References.MergeSound_Udin_Din_Din_Dun_Tric_Trac,
			"DinDinDinDin_Tric_Trac_Assassino", "Udin Din Din Dun Tric Trac Assassino", References.UdindindinTricTracAssassino, References.MergeSound_Udin_Din_Din_Dun_Tric_Trac_Assassino);

		// Strawberry Elephant
		RegisterBrainrot(
			"StrawberryElephant", "Strawberry Elephant", BrainrotValueRarity.Legendary, 1500, References.StrawberryElephant, References.MergeSound_Strawberry_Elephant,
			"StrawberryElephantAmbalabu", "Strawberry Elephant Ambalabu", References.StrawberryElephantAmbalabu, References.MergeSound_Strawberry_Elephant_Ambalabu,
			"StrawberryElephantAmbalabuGolubiro", "Strawberry Elephant Ambalabu Golubiro", References.StrawberryElephantAmbalabuGolubiro, References.MergeSound_Strawberry_Elephant_Ambalabu_Golubiro);

		// Baby Chimpanzini
		RegisterBrainrot("BabyChimpanzini", "Los Chimpancinitos", BrainrotValueRarity.Common, 10, References.ChimpanziniBaby, References.MergeSound_ChimpanziniBaby, eventId: "woods", onSpawn: (b) =>
		{
			// if (b.State == Brainrot.BrainrotState.InConveyor)
			{
				SFX.Play(References.MergeSound_ChimpanziniBaby, new SFX.PlaySoundDesc { Positional = false, Volume = 0.7f });
			}
		});
		RegisterBrainrot("BabyBrrBrr", "Los Brr Brr Patapim", BrainrotValueRarity.Common, 10, References.BrrBrrBaby, References.MergeSound_BrrBrrBaby, eventId: "woods", onSpawn: (b) =>
		{
			// if (b.State == Brainrot.BrainrotState.InConveyor)
			{
				SFX.Play(References.MergeSound_BrrBrrBaby, new SFX.PlaySoundDesc { Positional = false, Volume = 0.7f });
			}
		});
		RegisterBrainrot("BabyTralalerito", "Las Tralaleritas", BrainrotValueRarity.Common, 10, References.TralaleritoBaby, References.MergeSound_TralaleritoBaby, eventId: "woods", onSpawn: (b) =>
		{
			// if (b.State == Brainrot.BrainrotState.InConveyor)
			{
				SFX.Play(References.MergeSound_TralaleritoBaby, new SFX.PlaySoundDesc { Positional = false, Volume = 0.7f });
			}
		});
		RegisterBrainrot("BabyTrippiTroppi", "Los tripitopis", BrainrotValueRarity.Common, 10, References.TrippiTroppiBaby, References.MergeSound_TrippiTroppiBaby, eventId: "woods", onSpawn: (b) =>
		{
			// if (b.State == Brainrot.BrainrotState.InConveyor)
			{
				SFX.Play(References.MergeSound_TrippiTroppiBaby, new SFX.PlaySoundDesc { Positional = false, Volume = 0.7f });
			}
		});

		// Nights Monster
		RegisterBrainrot(
			"NightsMonster", "Night Monster", BrainrotValueRarity.Secret, 17500, References.NightsMonster, References.MergeSound_NightsMonster,
			"NightsMonsterCannelloni", "Night Monster Cannelloni", References.NightsMonsterCannelloni, References.MergeSound_NightsMonsterCannelloni,
			"NightsMonsterCannelloniTroppi", "Night Monster Cannelloni Troppi", References.NightsMonsterCannelloniTroppi, References.MergeSound_NightsMonsterCannelloniTroppi, eventId: "woods", onSpawn: (b) =>
			{
				// if (b.State == Brainrot.BrainrotState.InConveyor)
				{
					SFX.Play(References.MergeSound_NightsMonster, new SFX.PlaySoundDesc { Positional = false, Volume = 0.7f });
				}
			});

		RegisterBrainrot("LuckyBlock_Legendary", "Legendary Luck Block", BrainrotValueRarity.Legendary, 1400, References.LuckBlock_Legendary, References.MergeSound_LuckyBlock, onSpawn: (b) =>
		{
			// if (b.State == Brainrot.BrainrotState.InConveyor)
			{
				
			}
		});

		RegisterBrainrot("LuckyBlock_Mythic", "Mythic Luck Block", BrainrotValueRarity.Mythic, 12500, References.LuckBlock_Mythic, References.MergeSound_LuckyBlock, onSpawn: (b) =>
		{
			// if (b.State == Brainrot.BrainrotState.InConveyor)
			{
				SFX.Play(References.MergeSound_LuckyBlock, new SFX.PlaySoundDesc { Positional = false, Volume = 0.7f });
			}
		});

		RegisterBrainrot("LuckyBlock_Ethereal", "Ethereal Luck Block", BrainrotValueRarity.Ethereal, 17500, References.LuckBlock_Ethereal, References.MergeSound_LuckyBlock, onSpawn: (b) =>
		{
			// if (b.State == Brainrot.BrainrotState.InConveyor)
			{
				SFX.Play(References.MergeSound_LuckyBlock, new SFX.PlaySoundDesc { Positional = false, Volume = 0.7f });
			}
		});

		// RegisterSparkBrainrot("SixteeSeven", Store.SIXTEE_SEVEN_PRODUCT_ID);
		// RegisterSparkBrainrot("DinDinDinDin", Store.UDIN_PRODUCT_ID);
		// RegisterSparkBrainrot("StrawberryElephant", Store.STRAWBERRY_ELEPHANT_PRODUCT_ID);
	}
	public static void RegisterSparkBrainrot(string brainrotID, string sparksProductId)
	{
		Entries[brainrotID].StoreProductId = sparksProductId;
		Entries[brainrotID].IsNotStealable = true;
		// Also mark all evolutions in the chain as not stealable
		string currentId = brainrotID;
		string nextId;
		while (TryGetEvolution(currentId, out nextId))
		{
			if (Entries.ContainsKey(nextId))
			{
				Entries[nextId].IsNotStealable = true;
			}
			currentId = nextId;
		}
	}

	private static int AllocateStableId()
	{
		int id = NextStableId;
		NextStableId++;
		return id;
	}
	private static long CalculateBasePrice(BrainrotValueRarity rarity, double gps)
	{
		long RoundToStepLocal(double v, int step)
		{
			if (step <= 0) return (long)System.Math.Round(v);
			if (v <= 0) return 0;
			long rounded = (long)(System.Math.Round(v / step) * step);
			if (rounded == 0) rounded = step;
			return rounded;
		}
		double suggested;
		switch (rarity)
		{
			case BrainrotValueRarity.Common:
				suggested = System.Math.Max(0.0, 125.0 * gps - 100.0);
				return RoundToStepLocal(suggested, 25);
			case BrainrotValueRarity.Rare:
				suggested = 133.0 * gps;
				return RoundToStepLocal(suggested, 500);
			case BrainrotValueRarity.Epic:
				suggested = 160.0 * gps;
				return RoundToStepLocal(suggested, 2500);
			case BrainrotValueRarity.Legendary:
				suggested = 250.0 * gps;
				return RoundToStepLocal(suggested, 5000);
			case BrainrotValueRarity.Mythic:
				suggested = 260.0 * gps;
				return RoundToStepLocal(suggested, 100000);
			case BrainrotValueRarity.Ethereal:
				suggested = 350.0 * gps;
				return RoundToStepLocal(suggested, 200000);
			case BrainrotValueRarity.Primal:
				suggested = 400.0 * gps;
				return RoundToStepLocal(suggested, 250000);

			case BrainrotValueRarity.Secret:
				suggested = 450.0 * gps;
				return RoundToStepLocal(suggested, 50000);
			case BrainrotValueRarity.SecretForm1:
				suggested = 500.0 * gps;
				return RoundToStepLocal(suggested, 100000);
			case BrainrotValueRarity.SecretForm2:
				suggested = 550.0 * gps;
				return RoundToStepLocal(suggested, 150000);
			default:
				return 0;
		}
	}

	private static int CalculateBasePartsToEvolve(double gps)
	{
		int minParts = 10;   // ~30s
		int maxParts = 2800; // ~120m
		double partsFactor = 1.2;
		int parts = (int)System.Math.Ceiling(System.Math.Max(0.0, gps) * partsFactor);
		if (parts < minParts) parts = minParts;
		if (parts > maxParts) parts = maxParts;
		return parts;
	}

	public static void RegisterBrainrot(
		string baseKey, string baseName, BrainrotValueRarity baseRarity, long baseGoldGeneration, Texture baseTexture, AudioAsset baseSound, string eventId = "", Action<Enemy> onSpawn = null)
	{
		if (string.IsNullOrWhiteSpace(baseKey)) return;
		if (baseTexture == null) return;
		long gps = System.Math.Max(0, baseGoldGeneration);
		if (!Entries.ContainsKey(baseKey))
		{
			Entries[baseKey] = new BrainrotCatalogEntry
			{
				StableId = AllocateStableId(),
				Name = baseName ?? baseKey,
				BaseGoldGeneration = gps,
				BasePrice = CalculateBasePrice(baseRarity, gps),
				BasePartsToEvolve = CalculateBasePartsToEvolve(gps),
				Sprite = baseTexture,
				Rarity = baseRarity,
				Sound = baseSound,
				IsEvolution = false,
				EventId = eventId,
				OnSpawn = onSpawn
			};
		}
	}
	public static void RegisterBrainrot(
		string baseKey, string baseName, BrainrotValueRarity baseRarity, long baseGoldGeneration, Texture baseTexture, AudioAsset baseSound,
		string evo1Key, string evo1Name, Texture evo1Texture, AudioAsset evo1Sound,
		string evo2Key, string evo2Name, Texture evo2Texture, AudioAsset evo2Sound, string eventId = "", Action<Enemy> onSpawn = null)
	{
		if (string.IsNullOrWhiteSpace(baseKey) || string.IsNullOrWhiteSpace(evo1Key) || string.IsNullOrWhiteSpace(evo2Key)) return;
		if (baseTexture == null || evo1Texture == null || evo2Texture == null) return;

		long gps = System.Math.Max(0, baseGoldGeneration);
		long gpsEvo1 = System.Math.Max(0, baseGoldGeneration * 14 / 10);
		long gpsEvo2 = System.Math.Max(0, baseGoldGeneration * 16 / 10);

		if (gpsEvo1 == gps)
		{
			gpsEvo1 += 1;
			gpsEvo2 += 1;
		}

		if (!Entries.ContainsKey(baseKey))
		{
			Entries[baseKey] = new BrainrotCatalogEntry
			{
				StableId = AllocateStableId(),
				Name = baseName ?? baseKey,
				BaseGoldGeneration = gps,
				BasePrice = CalculateBasePrice(baseRarity, gps),
				BasePartsToEvolve = CalculateBasePartsToEvolve(gps),
				Sprite = baseTexture,
				Rarity = baseRarity,
				Sound = baseSound,
				IsEvolution = false,
				EventId = eventId,
				OnSpawn = onSpawn
			};
		}

        BrainrotValueRarity maxRarity = Enum.GetValues(typeof(BrainrotValueRarity)).Cast<BrainrotValueRarity>().Max();
		int baseR = (int)baseRarity;
		int evo1R = System.Math.Min((int)maxRarity, baseR + 1);
		int evo2R = System.Math.Min((int)maxRarity, baseR + 2);


		if (!Entries.ContainsKey(evo1Key))
		{
			Entries[evo1Key] = new BrainrotCatalogEntry
			{
				StableId = AllocateStableId(),
				Name = evo1Name ?? evo1Key,
				BaseGoldGeneration = gpsEvo1,
				BasePrice = CalculateBasePrice((BrainrotValueRarity)evo1R, gpsEvo1),
				BasePartsToEvolve = CalculateBasePartsToEvolve(gpsEvo1),
				Sprite = evo1Texture,
				Rarity = (BrainrotValueRarity)evo1R,
				Sound = evo1Sound,
				IsEvolution = true,
				EventId = eventId
			};
		}

		if (!Entries.ContainsKey(evo2Key))
		{
			Entries[evo2Key] = new BrainrotCatalogEntry
			{
				StableId = AllocateStableId(),
				Name = evo2Name ?? evo2Key,
				BaseGoldGeneration = gpsEvo2,
				BasePrice = CalculateBasePrice((BrainrotValueRarity)evo2R, gpsEvo2),
				BasePartsToEvolve = CalculateBasePartsToEvolve(gpsEvo2),
				Sprite = evo2Texture,
				Rarity = (BrainrotValueRarity)evo2R,
				Sound = evo2Sound,
				IsEvolution = true,
				EventId = eventId
			};
		}

		EvolutionMap[baseKey] = evo1Key;
		EvolutionMap[evo1Key] = evo2Key;
	}

	public static void RebuildSortedIndex()
	{
		EntriesByRarity = Entries
			.OrderBy(kv => kv.Value.Rarity)
			.ThenBy(kv => kv.Value.Name)
			.ToList();

		EntriesByRarityAllowedToSpawnInConveyor = new Dictionary<string, BrainrotCatalogEntry>();
		AllowedKeysByRarity = new Dictionary<BrainrotValueRarity, List<string>>();
		foreach (var kv in EntriesByRarity)
		{
			if (kv.Value.IsEvolution == false && kv.Value.StoreProductId == string.Empty && kv.Value.EventId == string.Empty)
			{
				EntriesByRarityAllowedToSpawnInConveyor.Add(kv.Key, kv.Value);
				if (!AllowedKeysByRarity.TryGetValue(kv.Value.Rarity, out var list))
				{
					list = new List<string>();
					AllowedKeysByRarity[kv.Value.Rarity] = list;
				}
				list.Add(kv.Key);
			}
		}
		EntriesByRarityAllowedToSpawnInConveyorShuffled = new List<KeyValuePair<string, BrainrotCatalogEntry>>(EntriesByRarityAllowedToSpawnInConveyor);

		// Build StableId maps (include all entries with StableId >= 0)
		StableIdToKey = new Dictionary<int, string>();
		KeyToStableId = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
		foreach (var kv in Entries)
		{
			int sid = kv.Value.StableId;
			if (sid >= 0)
			{
				if (!StableIdToKey.ContainsKey(sid)) StableIdToKey[sid] = kv.Key;
				if (!KeyToStableId.ContainsKey(kv.Key)) KeyToStableId[kv.Key] = sid;
			}
		}
	}

	public static bool TryGetEvolution(string fromCatalogId, out string toCatalogId)
	{
		if (fromCatalogId == null) { toCatalogId = null; return false; }
		if (EvolutionMap.TryGetValue(fromCatalogId, out toCatalogId))
		{
			if (toCatalogId != null && Entries.ContainsKey(toCatalogId)) return true;
		}
		toCatalogId = null;
		return false;
	}

	public static bool HasEvolution(string fromCatalogId)
	{
		return TryGetEvolution(fromCatalogId, out _);
	}

	public static void SetEvolution(string fromCatalogId, string toCatalogId)
	{
		if (string.IsNullOrEmpty(fromCatalogId) || string.IsNullOrEmpty(toCatalogId)) return;
		if (!Entries.ContainsKey(fromCatalogId)) return;
		if (!Entries.ContainsKey(toCatalogId)) return;
		EvolutionMap[fromCatalogId] = toCatalogId;
	}

	public static BrainrotCatalogEntry Get(string name)
	{
		if (Entries.TryGetValue(name, out var e)) return e;
		return new BrainrotCatalogEntry { Name = "Unknown", BaseGoldGeneration = 50, BasePrice = 100, Sprite = References.ZibraZurbraZibralini };
	}

	public static string GetRandomId()
	{
		// Two-stage: pick rarity by rarity weights; then pick entry within that rarity by entry weights
		if (EntriesByRarityAllowedToSpawnInConveyor.Count == 0) return "unknown";
		// Build rarity -> candidate count map from cache
		var rarityCounts = new System.Collections.Generic.Dictionary<BrainrotValueRarity, int>();
		foreach (var kv in AllowedKeysByRarity)
		{
			if (kv.Value != null && kv.Value.Count > 0)
			{
				rarityCounts[kv.Key] = kv.Value.Count;
			}
		}
		int rarityTotal = 0;
		float luck = GameManager.Instance != null ? System.MathF.Max(0f, GameManager.Instance.ServerLuck) : 1f;
		foreach (var r in rarityCounts.Keys)
		{
			if (!RaritySpawnWeights.TryGetValue(r, out int w)) w = 0;
			// Apply server luck to all rarities uniformly as requested
			float wf = w * luck;
			rarityTotal += System.Math.Max(0, (int)wf);
		}
		if (rarityTotal <= 0)
		{
			// fallback: first allowed id
			foreach (var key in EntriesByRarityAllowedToSpawnInConveyor.Keys)
				return key;
			return "cappuccino_assassino";
		}
		var seed = RNG.Seed((ulong)DateTime.UtcNow.Ticks);
		int rollR = RNG.RangeInt(ref seed, 0, rarityTotal);
		int accR = 0;
		BrainrotValueRarity chosenRarity = BrainrotValueRarity.Common;
		bool foundR = false;
		foreach (var r in rarityCounts.Keys)
		{
			if (!RaritySpawnWeights.TryGetValue(r, out int w)) w = 0;
			float wf = w * luck;
			accR += System.Math.Max(0, (int)wf);
			if (rollR < accR) { chosenRarity = r; foundR = true; break; }
		}
		if (!foundR)
		{
			foreach (var r in rarityCounts.Keys) { chosenRarity = r; break; }
		}
		var id = GetRandomIdByRarityWeighted(chosenRarity);
		if (!string.IsNullOrEmpty(id)) return id;
		// fallback
		foreach (var key in EntriesByRarityAllowedToSpawnInConveyor.Keys)
			return key;
		return "cappuccino_assassino";
	}

	public static string GetRandomIdByRarityWeighted(BrainrotValueRarity rarity)
	{
		// Use cached candidates for the requested rarity
		if (!AllowedKeysByRarity.TryGetValue(rarity, out var candidates) || candidates == null) return string.Empty;
		if (candidates.Count == 0) return string.Empty;

		int total = 0;
		for (int i = 0; i < candidates.Count; i++)
		{
			int w = GetEntrySpawnWeight(candidates[i]);
			total += System.Math.Max(0, w);
		}
		if (total <= 0)
		{
			// fallback: first candidate
			return candidates[0];
		}

		var seed = RNG.Seed((ulong)DateTime.UtcNow.Ticks);
		int roll = RNG.RangeInt(ref seed, 0, total);
		int acc = 0;
		for (int i = 0; i < candidates.Count; i++)
		{
			int w = System.Math.Max(0, GetEntrySpawnWeight(candidates[i]));
			acc += w;
			if (roll < acc) return candidates[i];
		}
		return candidates[candidates.Count - 1];
	}
	public static void Test()
	{
		var count = Entries.Count;
		Log.Info($"Count: {count}; {Random.Shared.Next(count, count)}");
	}

	// public static string GetRandomId()
	// {
	// 	int count = EntriesByRarityAllowedToSpawnInConveyor.Count;
	// 	if (count == 0) return "unknown";

	// 	//int idx = Random.Shared.Next(0, count);
	// 	var seed = RNG.Seed((ulong)DateTime.UtcNow.Ticks);
	// 	int idx = RNG.RangeInt(ref seed, 0, count);
	// 	int i = 0;
	// 	foreach (var key in EntriesByRarityAllowedToSpawnInConveyor.Keys)
	// 	{
	// 		if (i == idx) return key;
	// 		i++;
	// 	}
	// 	return "cappuccino_assassino";
	// }

	public static bool TryGetStableId(string key, out int stableId)
	{
		return KeyToStableId.TryGetValue(key, out stableId);
	}

	public static bool TryGetKeyFromStableId(int stableId, out string key)
	{
		return StableIdToKey.TryGetValue(stableId, out key);
	}

	// Export all brainrots (including evolutions) as CSV with columns: ID,Rarity,GPS,XPNeeded
	public static string ExportBrainrotsCsv(string outputPath = "res/CSV/Brainrots.csv")
	{
		try
		{
			var lines = new System.Collections.Generic.List<string>(Entries.Count + 4);
			lines.Add("ID,Rarity,GPS,XPNeeded");
			foreach (var kv in Entries)
			{
				string id = kv.Key;
				var e = kv.Value;
				int xpNeeded = ComputeXpNeededForNextStage(id, e);
				lines.Add($"{id},{e.Rarity},{e.BaseGoldGeneration},{xpNeeded}");
			}
			var dir = System.IO.Path.GetDirectoryName(outputPath);
			if (!string.IsNullOrEmpty(dir) && !System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
			System.IO.File.WriteAllLines(outputPath, lines);
			Log.Info($"Exported brainrots CSV to {outputPath}");
			return outputPath;
		}
		catch (System.Exception ex)
		{
			Log.Error($"Failed to export brainrots CSV: {ex}");
			return string.Empty;
		}
	}

	// Export names of brainrots that do NOT have a sound assigned (one name per line)
	public static string ExportBrainrotNamesWithoutSound(string outputPath = "res/CSV/BrainrotNames_NoSound.txt")
	{
		try
		{
			var lines = new System.Collections.Generic.List<string>(Entries.Count);
			foreach (var kv in Entries)
			{
				var e = kv.Value;
				if (e == null) continue;
				if (e.Sound != null) continue; // skip those that already have a sound
				string name = string.IsNullOrWhiteSpace(e.Name) ? kv.Key : e.Name;
				lines.Add('"' + name + '"' + ",");
			}
			lines.Sort(System.StringComparer.OrdinalIgnoreCase);
			var dir = System.IO.Path.GetDirectoryName(outputPath);
			if (!string.IsNullOrEmpty(dir) && !System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
			System.IO.File.WriteAllLines(outputPath, lines);
			Log.Info($"Exported {lines.Count} brainrot names without sound to {outputPath}");
			return outputPath;
		}
		catch (System.Exception ex)
		{
			Log.Error($"Failed to export brainrot names without sound: {ex}");
			return string.Empty;
		}
	}

	private static int ComputeXpNeededForNextStage(string id, BrainrotCatalogEntry e)
	{
		if (!HasEvolution(id)) return 0;
		int baseParts = e.BasePartsToEvolve > 0 ? e.BasePartsToEvolve : 50;
		float rarityMult = GetRarityXpFactor(e.Rarity);
		int needed = (int)System.MathF.Round(baseParts * rarityMult);
		return System.Math.Max(1, needed);
	}

	private static float GetRarityXpFactor(BrainrotValueRarity rarity)
	{
		switch (rarity)
		{
			case BrainrotValueRarity.Rare: return 2f;
			case BrainrotValueRarity.Epic: return 3f;
			case BrainrotValueRarity.Legendary: return 4f;
			case BrainrotValueRarity.Mythic: return 5f;
			case BrainrotValueRarity.Ethereal: return 6f;
			case BrainrotValueRarity.Primal: return 7f;
			default: return 1f; // Common
		}
	}
	

	private static readonly (BrainrotModifier modifier, int weight)[] _modifiers =
    {
        (BrainrotModifier.None,         80),
        (BrainrotModifier.Gold,         10),
        (BrainrotModifier.Diamond,      7),
        (BrainrotModifier.Rainbow,      2),
    };
	private static readonly (BrainrotModifier modifier, int weight)[] _all_modifiers =
    {
        (BrainrotModifier.None,         80),
        (BrainrotModifier.Gold,         10),
        (BrainrotModifier.Diamond,      7),
        (BrainrotModifier.Rainbow,      2),
        (BrainrotModifier.Woods,      1),
    };

    public static BrainrotModifier GetRandomModifier(bool includeAllModifiers = false)
    {
        // Event chance overlay first
        // var chances = EventManager.Instance?.GetAllowedModifierChances();
        // if (chances != null && chances.Count > 0)
        // {
        //     foreach (var kv in chances.OrderByDescending(k => k.Value))
        //     {
        //         double rollChance = Random.Shared.NextDouble();
        //         float luck = GameManager.Instance != null ? System.MathF.Max(0f, GameManager.Instance.ServerLuck) : 1f;
        //         float p = kv.Value;
        //         if (luck != 1f)
        //         {
        //             p *= luck;
        //             if (p > 1f) p = 1f;
        //         }
        //         if (rollChance <= p)
        //         {
        //             return kv.Key;
        //         }
        //     }
        // }

        // Fallback to base weighted table
        float luck2 = GameManager.Instance != null ? System.MathF.Max(0f, GameManager.Instance.ServerLuck) : 1f;
        int totalWeight = 0;
		// Scale non-None weights by server luck
		var modifiers = includeAllModifiers ? _all_modifiers : _modifiers;
        int[] scaled = new int[modifiers.Length];
        for (int i = 0; i < modifiers.Length; i++)
        {
            var (modifier, weight) = modifiers[i];
            int w = weight;
            if (modifier != BrainrotModifier.None && luck2 != 1f)
            {
                w = (int)System.MathF.Round(weight * luck2);
                if (weight > 0 && w <= 0) w = 1;
            }
            if (w < 0) w = 0;
            scaled[i] = w;
            totalWeight += w;
        }
        if (totalWeight <= 0) return modifiers.Last().modifier;
        int roll = Random.Shared.Next(totalWeight);
        int cumulative = 0;
        for (int i = 0; i < modifiers.Length; i++)
        {
            cumulative += scaled[i];
            if (roll < cumulative) return modifiers[i].modifier;
        }

        return modifiers.Last().modifier;
    }
}

public enum BrainrotValueRarity
{
	Common,
	Rare,
	Epic,
	Legendary,
	Mythic,

	Ethereal,
	Primal,

	Secret,
	SecretForm1,
	SecretForm2,
}

public enum BrainrotModifier
{
	None, // 0
	Gold, // 1
	Diamond, // 2
	Rainbow, // 3
	Woods, // 4
}


