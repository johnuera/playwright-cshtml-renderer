
using System.Text.Json;

namespace HBarPdfRazor;

public static class Localisation
{


    public static JsonLocale LoadLocale(string culture)
    {
        var baseDir = Directory.GetCurrentDirectory();
        var localesDir = Path.Combine(baseDir, "assets", "locales");

        // Try exact: de-DE.json
        var file = Path.Combine(localesDir, $"{culture}.json");

        // Fallback: de.json (if you use neutral culture files)
        if (!File.Exists(file) && culture.Contains('-'))
        {
            var neutral = culture.Split('-')[0];
            var neutralFile = Path.Combine(localesDir, $"{neutral}.json");
            if (File.Exists(neutralFile))
                file = neutralFile;
        }

        if (!File.Exists(file))
            throw new FileNotFoundException($"Locale file not found for '{culture}'. Looked for: {file}");

        var json = File.ReadAllText(file);
        return new JsonLocale(json);
    }

}
public class JsonLocale
{
    private readonly JsonDocument _doc;

    public JsonLocale(string json) => _doc = JsonDocument.Parse(json);
    /// <summary>
    /// Use this to get Translation for single value string
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string T(string key)
    {
        var el = GetElement(key);
        if (el.ValueKind == JsonValueKind.String) return el.GetString() ?? "";
        return el.ToString();
    }

    /// <summary>
    /// Use this to get Translation for Array
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public List<string> TA(string key)
    {
        var el = GetElement(key);
        if (el.ValueKind != JsonValueKind.Array) return new();

        return el.EnumerateArray()
                 .Select(x => x.GetString() ?? "")
                 .ToList();
    }

    private JsonElement GetElement(string key)
    {
        var parts = key.Split('.', StringSplitOptions.RemoveEmptyEntries);
        JsonElement element = _doc.RootElement;

        foreach (var part in parts)
            element = element.GetProperty(part);

        return element;
    }
}
