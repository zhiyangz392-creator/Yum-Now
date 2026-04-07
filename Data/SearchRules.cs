namespace Yum_Now;

internal static class SearchRules
{
    private static readonly Dictionary<string, string[]> MenuByTag = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Fast Food Restaurant"] = new[] { "Burger", "Fries", "Fried chicken" },
        ["Chinese Restaurant"] = new[] { "Fried rice", "Dim sum", "Roast duck" },
        ["Indian Restaurant"] = new[] { "Biryani", "Butter chicken", "Naan" },
        ["Sushi Restaurant"] = new[] { "Sushi", "Ramen", "Donburi" },
        ["Seafood Restaurant"] = new[] { "Chili crab", "Grilled fish", "Prawn noodles" },
        ["Coffee Shop"] = new[] { "Coffee", "Croissant", "Cake" }
    };

    private static readonly Dictionary<string, string[]> TagAliases = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Fast Food Restaurant"] = new[] { "fast food", "burger" },
        ["Chinese Restaurant"] = new[] { "chinese restaurant", "dim sum" },
        ["Indian Restaurant"] = new[] { "indian restaurant", "biryani" },
        ["Sushi Restaurant"] = new[] { "sushi restaurant", "japanese restaurant" },
        ["Seafood Restaurant"] = new[] { "seafood restaurant", "chili crab" },
        ["Coffee Shop"] = new[] { "coffee shop", "cafe" }
    };

    public static IEnumerable<string> BuildQueries(string tag)
    {
        var normalizedTag = tag.Trim();
        var queries = new List<string>();

        if (!string.IsNullOrWhiteSpace(normalizedTag))
        {
            queries.Add($"{normalizedTag} in Singapore");
            queries.Add($"{normalizedTag} Singapore");
        }

        foreach (var alias in ResolveAliases(normalizedTag))
        {
            queries.Add($"{alias} in Singapore");
        }

        queries.Add("restaurant in Singapore");

        return queries.Distinct(StringComparer.OrdinalIgnoreCase);
    }

    public static string[] ResolveMenuItems(string tag)
    {
        if (MenuByTag.TryGetValue(tag, out var directMenu))
        {
            return directMenu;
        }

        foreach (var (canonicalTag, aliases) in TagAliases)
        {
            if (aliases.Any(alias => alias.Equals(tag, StringComparison.OrdinalIgnoreCase)) &&
                MenuByTag.TryGetValue(canonicalTag, out var aliasMenu))
            {
                return aliasMenu;
            }
        }

        return new[] { "Chef special", "Set meal", "House drink" };
    }

    private static IEnumerable<string> ResolveAliases(string tag)
    {
        return TagAliases.TryGetValue(tag, out var aliases)
            ? aliases
            : Array.Empty<string>();
    }
}

