using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;

class SpineParser
{
    static void Main(string[] args)
    {
        try
        {
            if (args == null || args.Length < 2 || string.IsNullOrWhiteSpace(args[0]) || string.IsNullOrWhiteSpace(args[1]))
            {
                Console.Error.WriteLine("Usage: SpineParser <path-to-spine-file> <animations|skins>");
                return;
            }

            string spineFilePath = args[0];
            string requestedOutput = args[1].Trim().ToLowerInvariant();

            if (requestedOutput != "animations" && requestedOutput != "skins")
            {
                Console.Error.WriteLine("Second argument must be either 'animations' or 'skins'.");
                return;
            }

            if (!File.Exists(spineFilePath))
            {
                Console.Error.WriteLine($"Spine file not found at: {spineFilePath}");
                return;
            }
            string jsonContent = File.ReadAllText(spineFilePath);

            using (JsonDocument document = JsonDocument.Parse(jsonContent))
            {
                JsonElement root = document.RootElement;

                if (requestedOutput == "animations")
                {
                    if (root.TryGetProperty("animations", out JsonElement animations))
                    {
                        foreach (JsonProperty animation in animations.EnumerateObject())
                        {
                            Console.WriteLine(animation.Name);
                        }
                    }
                    else
                    {
                        // No stdout on purpose; errors go to stderr
                        Console.Error.WriteLine("No 'animations' property found in the JSON.");
                    }
                }
                else if (requestedOutput == "skins")
                {
                    if (root.TryGetProperty("skins", out JsonElement skins))
                    {
                        foreach (JsonElement skin in skins.EnumerateArray())
                        {
                            if (skin.TryGetProperty("name", out JsonElement skinName))
                            {
                                Console.WriteLine(skinName.GetString());
                            }
                        }
                    }
                    else
                    {
                        Console.Error.WriteLine("No 'skins' property found in the JSON.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error parsing Spine file: {ex.Message}");
            Console.Error.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }
}
