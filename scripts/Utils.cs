using AO;

public static class StringUtils
{
    public static string FormatMoney(double money)
    {
        return FormatMoney((long)money);
    }

    public static string FormatMoney(long money)
    {
        if (money < 1000)
        {
            return money.ToString();
        }
        else if (money < 1000000)
        {
            return (money / 1000).ToString() + "k";
        }
        else if (money < 1000000000)
        {
            return (money / 1000000).ToString() + "m";
        }
        else if (money < 1000000000000)
        {
            return (money / 1000000000).ToString() + "b";
        }
        else if (money < 1000000000000000)
        {
            return (money / 1000000000000).ToString() + "t";
        }
        else
        {
            return (money / 1000000000000000).ToString() + "q";
        }
    }
}

public static class UIUtils
{
	// thickness from 0 to 1;
	public static void DrawCross(Rect rect, Vector4 color, float thickness = 1f, float offsetX = 0f, float offsetY = 0f, bool doubleCross = false, float outlinesThickness = 0f)
	{
		float maxThickness = 1f;
		float actualThickness = maxThickness - (maxThickness * thickness);
		actualThickness = actualThickness * 0.5f * rect.Height;

		var lineRect = rect.InsetUnscaled(actualThickness, 0, actualThickness, 0);
		var crossColor = color;
		lineRect = lineRect.Offset(offsetX, offsetY);
		using (var _r1 = UI.PUSH_ROTATE_ABOUT_POINT(45f, lineRect.Center))
		{
			UI.Image(lineRect, UI.WhiteSprite, crossColor);
			if (outlinesThickness > 0f)
			{
				DrawOutlines(lineRect, Vector4.Black, 1f);
			}
		}
		if (doubleCross == true)
		{
			using (var _r2 = UI.PUSH_ROTATE_ABOUT_POINT(-45f, lineRect.Center))
			{
				UI.Image(lineRect, UI.WhiteSprite, crossColor);
				if (outlinesThickness > 0f)
				{
					DrawOutlines(lineRect, Vector4.Black, 1f);
				}
			}
		}
	}
	public static UI.TextSettings CenteredText(bool dropShadow = false)
	{
		UI.TextSettings textSettings = UI.TextSettings.Default;
		textSettings.HorizontalAlignment = UI.HorizontalAlignment.Center;
		textSettings.VerticalAlignment = UI.VerticalAlignment.Center;
		textSettings.DoAutofit = true;
		textSettings.AutofitMaxSize = 40;
		if (dropShadow)
		{
			textSettings.DropShadow = true;
			textSettings.DropShadowColor = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
			textSettings.DropShadowOffset = new Vector2(0.0f, -5f);
		}
		return textSettings;
	}

	public static UI.TextSettings WorldCenteredText(bool dropShadow = false)
	{
		UI.TextSettings textSettings = CenteredText(dropShadow);
		textSettings.AutofitMinSize /= 20;
		textSettings.AutofitMaxSize /= 20;
		return textSettings;
	}

	public static void IncreaseOutline(this UI.TextSettings settings, float delta)
	{
		settings.OutlineThickness += delta;
		if (settings.DropShadow)
		{
			settings.DropShadowOffset += new Vector2(0.0f, delta);
		}
	}

	public static Rect[] VerticalSlice(this Rect rect, int numSlices, float spacing)
	{
		if (numSlices <= 0) return Array.Empty<Rect>();

		Rect[] returnVal = new Rect[numSlices];
		float sliceSize = (rect.Width - spacing * (numSlices - 1)) / numSlices;

		for (int i = 0; i < numSlices - 1; i++)
		{
			returnVal[i] = rect.CutLeftUnscaled(sliceSize);
			rect.CutLeftUnscaled(spacing);
		}
		returnVal[^1] = rect;
		return returnVal;
	}

	public static Vector4 GetRarityColor(ItemRarity rarity)
	{
		return rarity switch
		{
			ItemRarity.Common => new Vector4(0.9f, 0.9f, 0.9f, 1),           // White (no tint)
			ItemRarity.Rare => new Vector4(0.3f, 0.7f, 1f, 1),        // Sky blue
			ItemRarity.Epic => new Vector4(0.8f, 0.3f, 1f, 1),        // Bright purple
			ItemRarity.Legendary => new Vector4(1f, 0.8f, 0.2f, 1),   // Golden yellow
			ItemRarity.Mythic => new Vector4(1f, 0.2f, 0.2f, 1),   // Red
			_ => new Vector4(1f, 1f, 1f, 1),
		};
	}


	public static Rect DrawRainbowText(Rect r, string text, float size)
	{
		var ts = UIUtils.CenteredText(false);
		ts.DoAutofit = false;
		ts.Size = size;
		ts.Font = UI.Fonts.BarlowBold;
		ts.HorizontalAlignment = UI.HorizontalAlignment.Left;
		ts.Outline = true;
		ts.OutlineThickness = 4;
		ts.OutlineColor = new Vector4(0f, 0f, 0f, 1f);


		float t = (float)Time.TimeSinceStartup * 0.20f;
		float hue = (0f + t) - MathF.Floor(0f + t);
		Vector3 rgb = HSVToRGB(hue, 1f, 1f);
		ts.Color = new Vector4(rgb.X, rgb.Y, rgb.Z, 1f);
		return UI.TextSync(r, text, ts);
	}
	public static Rect DrawRainbowText(Rect r, string text, UI.TextSettings ts)
	{
		float t = (float)Time.TimeSinceStartup * 0.20f;
		float hue = (0f + t) - MathF.Floor(0f + t);
		Vector3 rgb = HSVToRGB(hue, 1f, 1f);
		ts.Color = new Vector4(rgb.X, rgb.Y, rgb.Z, 1f);
		return UI.TextSync(r, text, ts);
	}

	public static Vector3 HSVToRGB(float h, float s, float v)
	{
		// h in [0,1), s,v in [0,1]
		h = (h - MathF.Floor(h));
		float i = MathF.Floor(h * 6f);
		float f = h * 6f - i;
		float p = v * (1f - s);
		float q = v * (1f - f * s);
		float t = v * (1f - (1f - f) * s);
		int ii = (int)i % 6;
		return ii switch
		{
			0 => new Vector3(v, t, p),
			1 => new Vector3(q, v, p),
			2 => new Vector3(p, v, t),
			3 => new Vector3(p, q, v),
			4 => new Vector3(t, p, v),
			_ => new Vector3(v, p, q),
		};
	}

	// Linearly interpolate two colors (RGBA)
	public static Vector4 Lerp(Vector4 a, Vector4 b, float t)
	{
		return new Vector4(
			AOMath.Lerp(a.X, b.X, t),
			AOMath.Lerp(a.Y, b.Y, t),
			AOMath.Lerp(a.Z, b.Z, t),
			AOMath.Lerp(a.W, b.W, t)
		);
	}

	// Draw animated gradient text that lerps between two provided colors over time
	public static Rect DrawRangeColorText(Rect r, string text, float size, Vector4 colorStart, Vector4 colorEnd, float speed = 0.8f, bool dropShadow = true, bool leftAligned = false)
	{
		var ts = CenteredText(dropShadow);
		ts.DoAutofit = false;
		ts.Size = size;
		ts.Font = UI.Fonts.BarlowBold;
		ts.HorizontalAlignment = leftAligned ? UI.HorizontalAlignment.Left : UI.HorizontalAlignment.Center;
		ts.Outline = true;
		ts.OutlineThickness = 4;
		ts.OutlineColor = new Vector4(0f, 0f, 0f, 1f);

		// t ping-pongs in [0,1]
		float t = (float)Time.TimeSinceStartup * MathF.Max(0.01f, speed);
		float pingPong = 0.5f * (1f + MathF.Sin(t));
		ts.Color = Lerp(colorStart, colorEnd, pingPong);
		return UI.TextSync(r, text, ts);
	}

	// Draw a rounded highlight behind a rect using a nine-sliced backplate
	public static void DrawHighlight(Rect rect, Vector4 color, float padding = 8f, float alpha = 0.6f, float corner = 100f)
	{
		var nine = new UI.NineSlice() { slice = new Vector4(corner, corner, corner, corner), sliceScale = 1f };
		var back = rect.Grow(padding);
		var tint = new Vector4(color.X, color.Y, color.Z, alpha);
		UI.Image(back, References.RoundedRectangle, tint, nine);
	}

	// Draw a pulsing highlight for extra emphasis
	public static void DrawHighlightPulse(Rect rect, Vector4 color, float padding = 10f, float minAlpha = 0.35f, float maxAlpha = 0.65f, float speed = 2f, float corner = 100f)
	{
		float s = (float)Time.TimeSinceStartup * MathF.Max(0.01f, speed);
		float osc = 0.5f * (1f + MathF.Sin(s));
		float a = AOMath.Lerp(minAlpha, maxAlpha, osc);
		DrawHighlight(rect, color, padding, a, corner);
	}

	// Draw just an outline around a rect (no fill). Optional: behindCurrentLayer draws one layer below.
	public static void DrawOutlineHighlight(Rect rect, Vector4 color, float thickness = 6f, float padding = 8f, bool behindCurrentLayer = false)
	{
		if (behindCurrentLayer)
		{
			using var _L = UI.PUSH_LAYER_RELATIVE(-1);
			var outer = rect.Grow(padding);
			DrawOutlines(outer, color, thickness);
			return;
		}
		var outer2 = rect.Grow(padding);
		DrawOutlines(outer2, color, thickness);
	}

	// Pulsing outline variant
	public static void DrawOutlineHighlightRainbow(Rect rect, float thickness = 6f, float padding = 8f, float maxAlpha = 0.75f, float speed = 2f, bool behindCurrentLayer = false)
	{
		// Rainbow hue cycle with steady alpha (use maxAlpha for clear visibility)
		float t = (float)Time.TimeSinceStartup * MathF.Max(0.01f, speed);
		float hue = (0f + t) - MathF.Floor(0f + t);
		Vector3 rgb = HSVToRGB(hue, 1f, 1f);
		var c = new Vector4(rgb.X, rgb.Y, rgb.Z, maxAlpha);
		DrawOutlineHighlight(rect, c, thickness, padding, behindCurrentLayer);
	}

	// Draw a floating arrow above a target rect. Uses Time.TimeSinceStartup for animation.
	public static void DrawFloatingArrowAbove(Rect target, Texture arrowTexture, float degrees = -90f, float floatSpeed = 5f, float floatHeight = 20f, float baseYOffset = 10f, float growLeft = 120f, float growTop = 120f, float growRight = 0f, float growBottom = 120f)
	{
		if (arrowTexture == null) return;
		float t = (float)Time.TimeSinceStartup;
		float yOffset = MathF.Sin(t * floatSpeed) * floatHeight;
		var newRect = target.TopRect().Grow(growLeft, growTop, growRight, growBottom).FitAspect(arrowTexture.Aspect).Offset(0, baseYOffset + yOffset);
		using var _ = UI.PUSH_ROTATE_ABOUT_POINT(degrees, newRect.Center);
		UI.Image(newRect, arrowTexture, Vector4.White);
	}

	// Draw attention-grabbing text above an anchor with smooth bump + wobble
	public static void DrawBumpingLabel(Rect anchorRect, string text, float baseSize = 24f, float scaleAmplitude = 0.12f, float scaleSpeed = 4f, float rotateAmplitudeDeg = 6f, float rotateSpeed = 2.2f, UI.HorizontalAlignment HAlignment = UI.HorizontalAlignment.Center)
	{
		//var pad = anchorRect.TopCenterRect();
		var pad = anchorRect;
		float t = (float)Time.TimeSinceStartup;
		float scale = 1f + scaleAmplitude * (0.5f * (1f + MathF.Sin(t * MathF.Max(0.01f, scaleSpeed))));
		float angle = MathF.Sin(t * MathF.Max(0.01f, rotateSpeed)) * rotateAmplitudeDeg;
		var ts = UIUtils.CenteredText(true); ts.DoAutofit = false; ts.Size = baseSize * scale; ts.HorizontalAlignment = HAlignment;
		using var _rot = UI.PUSH_ROTATE_ABOUT_POINT(angle, pad.Center);
		UI.TextAsync(pad, text, ts);
	}
	public static void DrawOutlines(Rect r, Vector4 color, float thickness)
	{
		var top = r.GrowTop(thickness).CutTop(thickness);
		var bottom = r.GrowBottom(thickness).CutBottom(thickness);
		var left = r.GrowLeft(thickness).CutLeft(thickness);
		var right = r.GrowRight(thickness).CutRight(thickness);

		bottom.Min = new Vector2(left.Min.X, bottom.Min.Y);
		bottom.Max = new Vector2(right.Max.X, bottom.Max.Y);

		top.Min = new Vector2(left.Min.X, top.Min.Y);
		top.Max = new Vector2(right.Max.X, top.Max.Y);

		UI.Image(top, UI.WhiteSprite, color);
		UI.Image(bottom, UI.WhiteSprite, color);
		UI.Image(left, UI.WhiteSprite, color);
		UI.Image(right, UI.WhiteSprite, color);
	}

	public static void DrawTimerOnRect(Rect rect, float timerLeft, float timerMax, Texture texture)
	{
		float t = MathF.Max(0f, MathF.Min(1f, timerMax > 0 ? timerLeft / timerMax : 0f));
		float fillHeight = rect.Height * t;

		var rectNew = new Rect(rect.Min, rect.Max);
		var fillrect = rectNew.CutBottomUnscaled(fillHeight);

		var maskScope = IM.MakeMaskScope(rect);
		{
			using var _ = IM.BUILD_MASK_SCOPE(maskScope);
			UI.Image(rect, texture, new Vector4(0.1f, 0.1f, 0.1f, 0.95f));
		}
		{
			using var _ = IM.USE_MASK_SCOPE(maskScope);
			UI.Image(fillrect, null, new Vector4(0f, 0f, 0f, 0.95f));
		}


		var left = timerMax - (timerMax - timerLeft);
		UI.TextAsync(rect, $"{left:0.0}s", UIUtils.CenteredText(true));
	}
}

public static class Utils
{
    public static float GetZOffset(this Spine_Animator spineAnimator)
    {
        return spineAnimator.Entity.Position.Y + spineAnimator.DepthOffset * spineAnimator.Entity.Scale.Y;
    }
    public static float GetZOffset(this Sprite_Renderer spineAnimator)
    {
        return spineAnimator.Entity.Position.Y + spineAnimator.DepthOffset * spineAnimator.Entity.Scale.Y;
    }

    public static float GetZOffset(this Entity entity)
    {
        return entity.Position.Y * entity.Scale.Y;
    }
}