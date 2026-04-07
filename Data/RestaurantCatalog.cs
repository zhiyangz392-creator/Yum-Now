namespace Yum_Now;

internal static class RestaurantCatalog
{
    public static readonly MenuSeed[] DefaultMenu =
    {
        new("Chef Special", 8.90m),
        new("Set Meal", 10.50m),
        new("House Drink", 2.80m)
    };

    private static readonly List<RestaurantMenuProfile> RestaurantProfiles = new()
    {
        new("McDonald's", new[] { "mcdonald", "mcdonald's", "mcdonalds" }, new[]
        {
            new MenuSeed("Big Mac Meal", 8.90m),
            new MenuSeed("McSpicy Meal", 9.50m),
            new MenuSeed("Filet-O-Fish Meal", 8.20m),
            new MenuSeed("McChicken", 5.80m),
            new MenuSeed("Fries (M)", 3.60m),
            new MenuSeed("Coke (M)", 2.80m)
        }),
        new("KFC", new[] { "kfc", "kentucky fried chicken" }, new[]
        {
            new MenuSeed("2pc Chicken Meal", 9.80m),
            new MenuSeed("Zinger Burger Meal", 9.20m),
            new MenuSeed("Famous Bowl", 7.90m),
            new MenuSeed("Popcorn Chicken", 6.80m),
            new MenuSeed("Whipped Potato", 3.20m),
            new MenuSeed("Pepsi (M)", 2.70m)
        }),
        new("Burger King", new[] { "burger king" }, new[]
        {
            new MenuSeed("Whopper Meal", 9.40m),
            new MenuSeed("Mushroom Swiss", 8.90m),
            new MenuSeed("Chicken Royale", 8.40m),
            new MenuSeed("Onion Rings", 4.10m),
            new MenuSeed("Fries (M)", 3.40m),
            new MenuSeed("Coke (M)", 2.70m)
        }),
        new("Subway", new[] { "subway" }, new[]
        {
            new MenuSeed("Chicken Teriyaki 6-inch", 7.90m),
            new MenuSeed("Tuna 6-inch", 8.10m),
            new MenuSeed("Italian B.M.T. 6-inch", 8.30m),
            new MenuSeed("Cookie", 1.80m),
            new MenuSeed("Bottled Water", 2.20m),
            new MenuSeed("Combo Upgrade", 3.50m)
        }),
        new("Din Tai Fung", new[] { "din tai fung" }, new[]
        {
            new MenuSeed("Xiao Long Bao (10pcs)", 13.80m),
            new MenuSeed("Pork Chop Fried Rice", 14.80m),
            new MenuSeed("Shrimp & Pork Wonton Noodle Soup", 13.80m),
            new MenuSeed("Hot & Sour Soup", 8.20m),
            new MenuSeed("Steamed Vegetable & Pork Dumpling", 12.50m),
            new MenuSeed("Honey Lemon Drink", 4.20m)
        }),
        new("Crystal Jade", new[] { "crystal jade" }, new[]
        {
            new MenuSeed("Roasted Duck", 18.80m),
            new MenuSeed("Pork Dumplings", 9.80m),
            new MenuSeed("Seafood Hor Fun", 15.50m),
            new MenuSeed("Sweet & Sour Pork", 14.90m),
            new MenuSeed("Yangzhou Fried Rice", 13.20m),
            new MenuSeed("Chinese Tea", 2.80m)
        }),
        new("Song Fa Bak Kut Teh", new[] { "song fa", "bak kut teh" }, new[]
        {
            new MenuSeed("Signature Pork Ribs Soup", 10.80m),
            new MenuSeed("Braised Pork Trotter", 7.50m),
            new MenuSeed("You Tiao", 2.20m),
            new MenuSeed("Salted Vegetables", 3.20m),
            new MenuSeed("Braised Peanuts", 3.20m),
            new MenuSeed("Chinese Tea", 2.00m)
        }),
        new("JUMBO Seafood", new[] { "jumbo seafood", "jumbo" }, new[]
        {
            new MenuSeed("Chili Crab", 68.00m),
            new MenuSeed("Black Pepper Crab", 68.00m),
            new MenuSeed("Cereal Prawn", 28.00m),
            new MenuSeed("Sambal Kangkong", 14.00m),
            new MenuSeed("Fried Mantou", 8.00m),
            new MenuSeed("Lime Juice", 4.50m)
        }),
        new("No Signboard Seafood", new[] { "no signboard seafood", "no signboard" }, new[]
        {
            new MenuSeed("White Pepper Crab", 72.00m),
            new MenuSeed("Salted Egg Prawn", 30.00m),
            new MenuSeed("Steamed Fish", 38.00m),
            new MenuSeed("Sambal Lala", 18.00m),
            new MenuSeed("Bean Sprouts", 10.00m),
            new MenuSeed("Chinese Tea", 2.50m)
        }),
        new("Ya Kun Kaya Toast", new[] { "ya kun" }, new[]
        {
            new MenuSeed("Kaya Toast Set A", 5.60m),
            new MenuSeed("French Toast Set", 6.20m),
            new MenuSeed("Soft-Boiled Eggs", 2.40m),
            new MenuSeed("Kopi", 2.20m),
            new MenuSeed("Teh", 2.20m),
            new MenuSeed("Iced Kopi", 2.80m)
        }),
        new("Toast Box", new[] { "toast box" }, new[]
        {
            new MenuSeed("Peanut Butter Toast Set", 6.20m),
            new MenuSeed("Laksa", 8.90m),
            new MenuSeed("Mee Siam", 7.90m),
            new MenuSeed("Curry Chicken", 9.20m),
            new MenuSeed("Kopi", 2.30m),
            new MenuSeed("Teh", 2.30m)
        }),
        new("Starbucks", new[] { "starbucks" }, new[]
        {
            new MenuSeed("Caffe Latte", 7.20m),
            new MenuSeed("Cappuccino", 7.20m),
            new MenuSeed("Caramel Macchiato", 8.20m),
            new MenuSeed("Chocolate Croissant", 5.50m),
            new MenuSeed("Blueberry Muffin", 4.80m),
            new MenuSeed("Cheesecake", 7.50m)
        }),
        new("The Daily Cut", new[] { "the daily cut" }, new[]
        {
            new MenuSeed("Build Your Own Bowl", 13.90m),
            new MenuSeed("Grilled Chicken Bowl", 14.50m),
            new MenuSeed("Beef Bowl", 16.80m),
            new MenuSeed("Roasted Veggies", 4.50m),
            new MenuSeed("Iced Black Coffee", 3.80m),
            new MenuSeed("Kombucha", 6.20m)
        }),
        new("LingZhi Vegetarian", new[] { "lingzhi vegetarian", "ling zhi" }, new[]
        {
            new MenuSeed("Vegetarian Bento", 12.80m),
            new MenuSeed("Braised Tofu", 10.80m),
            new MenuSeed("Mushroom Fried Rice", 11.50m),
            new MenuSeed("Vegetarian Laksa", 10.90m),
            new MenuSeed("Double-Boiled Soup", 8.20m),
            new MenuSeed("Barley Drink", 3.20m)
        }),
        new("Li Wei Vegetarian", new[] { "li wei vegetarian", "li wei" }, new[]
        {
            new MenuSeed("Vegetarian Mixed Rice", 7.50m),
            new MenuSeed("Vegetarian Noodles", 6.80m),
            new MenuSeed("Fried Beancurd", 5.20m),
            new MenuSeed("Curry Vegetables", 6.50m),
            new MenuSeed("Soup of the Day", 4.50m),
            new MenuSeed("Iced Lemon Tea", 2.80m)
        })
    };

    public static bool TryGetProfileMenu(string restaurantName, out List<MenuSeed> items, out string source)
    {
        items = new List<MenuSeed>();
        source = string.Empty;

        if (string.IsNullOrWhiteSpace(restaurantName))
        {
            return false;
        }

        var profile = RestaurantProfiles.FirstOrDefault(p =>
            p.Keywords.Any(keyword =>
                restaurantName.Contains(keyword, StringComparison.OrdinalIgnoreCase)));

        if (profile == null)
        {
            return false;
        }

        items = profile.Items.ToList();
        source = $"matched real restaurant profile ({profile.DisplayName})";
        return true;
    }

    public static List<MenuSeed> BuildCategoryMenu(string tag, string placeType)
    {
        var key = $"{tag} {placeType}".ToLowerInvariant();

        if (key.Contains("fast food"))
        {
            return new List<MenuSeed>
            {
                new("Chicken Burger", 6.90m),
                new("Beef Burger", 7.50m),
                new("Fries", 3.40m),
                new("Nuggets", 5.80m),
                new("Soft Drink", 2.70m)
            };
        }

        if (key.Contains("chinese"))
        {
            return new List<MenuSeed>
            {
                new("Fried Rice", 8.90m),
                new("Dim Sum Platter", 11.80m),
                new("Roast Duck Rice", 9.80m),
                new("Wanton Noodle", 7.80m),
                new("Chinese Tea", 2.50m)
            };
        }

        if (key.Contains("indian"))
        {
            return new List<MenuSeed>
            {
                new("Chicken Biryani", 9.90m),
                new("Butter Chicken", 10.80m),
                new("Masala Dosa", 7.20m),
                new("Garlic Naan", 3.20m),
                new("Teh Tarik", 2.40m)
            };
        }

        if (key.Contains("sushi") || key.Contains("japanese"))
        {
            return new List<MenuSeed>
            {
                new("Salmon Sushi", 9.80m),
                new("Chicken Katsu Don", 10.90m),
                new("Tonkotsu Ramen", 12.50m),
                new("Gyoza", 5.80m),
                new("Green Tea", 2.60m)
            };
        }

        if (key.Contains("seafood"))
        {
            return new List<MenuSeed>
            {
                new("Chili Crab", 58.00m),
                new("Cereal Prawn", 24.00m),
                new("Steamed Fish", 32.00m),
                new("Sambal Kangkong", 12.00m),
                new("Rice", 1.50m)
            };
        }

        if (key.Contains("coffee") || key.Contains("cafe"))
        {
            return new List<MenuSeed>
            {
                new("Caffe Latte", 6.80m),
                new("Americano", 5.50m),
                new("Croissant", 4.20m),
                new("Cheesecake", 7.20m),
                new("Iced Tea", 3.50m)
            };
        }

        return new List<MenuSeed>();
    }

    public static List<MenuSeed> BuildRouteMenu(string menuRaw)
    {
        if (string.IsNullOrWhiteSpace(menuRaw))
        {
            return new List<MenuSeed>();
        }

        var names = menuRaw
            .Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var menu = new List<MenuSeed>();
        const decimal basePrice = 6.80m;

        for (var i = 0; i < names.Count; i++)
        {
            menu.Add(new MenuSeed(names[i], basePrice + i * 1.20m));
        }

        return menu;
    }

    internal sealed record MenuSeed(string Name, decimal Price);

    private sealed class RestaurantMenuProfile
    {
        public RestaurantMenuProfile(string displayName, IEnumerable<string> keywords, IEnumerable<MenuSeed> items)
        {
            DisplayName = displayName;
            Keywords = keywords.ToList();
            Items = items.ToList();
        }

        public string DisplayName { get; }
        public List<string> Keywords { get; }
        public List<MenuSeed> Items { get; }
    }
}
