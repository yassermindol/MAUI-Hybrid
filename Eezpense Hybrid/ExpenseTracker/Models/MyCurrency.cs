namespace ExpenseTracker.Models;

public class MyCurrency
{
    public string Country { get; set; }
    public string Name { get; set; }
    public string Symbol { get; set; }
    public string NameSymbol => $"{Name} ({Symbol})";

    List<MyCurrency> currencies = null;
    public List<MyCurrency> Currencies
    {
        get
        {
            if (currencies == null)
            {
                currencies = GetCurrencies();
            }
            return currencies;
        }
    }

    private List<MyCurrency> GetCurrencies()
    {
        var result = new List<MyCurrency>
        {
            // Asian currencies
            new MyCurrency { Country = "Bangladesh", Name = "Bangladeshi Taka", Symbol = "৳" },
            new MyCurrency { Country = "China", Name = "Chinese Yuan", Symbol = "¥" },
            new MyCurrency { Country = "India", Name = "Indian Rupee", Symbol = "₹" },
            new MyCurrency { Country = "Indonesia", Name = "Indonesian Rupiah", Symbol = "Rp" },
            new MyCurrency { Country = "Japan", Name = "Japanese Yen", Symbol = "¥" },
            new MyCurrency { Country = "Malaysia", Name = "Malaysian Ringgit", Symbol = "RM" },
            new MyCurrency { Country = "Myanmar", Name = "Myanmar Kyat", Symbol = "Ks" },
            new MyCurrency { Country = "Nepal", Name = "Nepalese Rupee", Symbol = "रु" },
            new MyCurrency { Country = "Philippines", Name = "Philippine Peso", Symbol = "₱" },
            new MyCurrency { Country = "Singapore", Name = "Singapore Dollar", Symbol = "$" },
            new MyCurrency { Country = "South Korea", Name = "South Korean Won", Symbol = "₩" },
            new MyCurrency { Country = "Sri Lanka", Name = "Sri Lankan Rupee", Symbol = "Rs" },
            new MyCurrency { Country = "Taiwan", Name = "New Taiwan Dollar", Symbol = "$" }, // Optional
            new MyCurrency {Country = "Thailand", Name = "Thai Baht", Symbol = "฿"},
            new MyCurrency {Country = "Vietnam", Name = "Vietnamese Dong", Symbol = "₫"},

            // Major global currencies
            new MyCurrency { Country = "Australia", Name = "Australian Dollar", Symbol = "$" },
            new MyCurrency { Country = "Austria", Name = "Austrian Euro", Symbol = "€" }, // Part of Eurozone
            new MyCurrency { Country = "Belgium", Name = "Belgian Euro", Symbol = "€" },
            new MyCurrency { Country = "Brazil", Name = "Brazilian Real", Symbol = "R$" },
            new MyCurrency { Country = "Canada", Name = "Canadian Dollar", Symbol = "$" },
            new MyCurrency { Country = "France", Name = "French Euro", Symbol = "€" },
            new MyCurrency { Country = "Germany", Name = "German Euro", Symbol = "€" },
            new MyCurrency { Country = "Mexico", Name = "Mexican Peso", Symbol = "$" },
            new MyCurrency { Country = "New Zealand", Name = "New Zealand Dollar", Symbol = "$" },
            new MyCurrency { Country = "Russia", Name = "Russian Ruble", Symbol = "₽" }, // Included as per request
            new MyCurrency { Country = "South Africa", Name = "South African Rand", Symbol = "R" },
            new MyCurrency { Country = "Switzerland", Name = "Swiss Franc", Symbol = "Fr" },

            // Middle East
            new MyCurrency { Country = "Israel", Name = "Israeli New Shekel", Symbol = "₪" },
            new MyCurrency { Country = "Saudi Arabia", Name = "Saudi Riyal", Symbol = "ر.س" },
            new MyCurrency { Country = "UAE", Name = "United Arab Emirates Dirham", Symbol = "د.إ" },

            // Additional popular currencies
            new MyCurrency { Country = "United Kingdom", Name = " British Pound Sterling", Symbol = "£" },
            new MyCurrency { Country = "United States", Name = "US Dollar", Symbol = "$" },
            new MyCurrency { Country = "Turkey", Name = "Turkish Lira", Symbol = "₺" },
            new MyCurrency { Country = "Hungary", Name = "Hungarian Forint", Symbol = "Ft" },
            new MyCurrency { Country = "Poland", Name = "Polish Zloty", Symbol = "zł" },
            new MyCurrency { Country = "Sweden", Name = "Swedish Krona", Symbol = "kr" },
            new MyCurrency { Country = "Norway", Name = "Norwegian Krone", Symbol = "kr" },
            new MyCurrency { Country = "Denmark", Name = "Danish Krone", Symbol = "kr" },
        };

        var sorted = result.OrderBy(c => c.Name).ToList();
        return sorted;
    }
}

