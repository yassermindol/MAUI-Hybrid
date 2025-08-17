using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Settings;
using Kotlin.Properties;
using System.Collections.ObjectModel;

namespace ExpenseTracker.Features.OnBoarding.Models;
public partial class CategorySlideModel : SlideModel
{
    [ObservableProperty]
    ObservableCollection<string> categories = new ObservableCollection<string>();

    [ObservableProperty]
    bool isAddButtonEnabled;

    [ObservableProperty]
    string addMessage;

    string categoryText;
    public string CategoryText
    {
        get => categoryText;
        set
        {
            SetProperty(ref categoryText, value);
            IsAddButtonEnabled = !string.IsNullOrWhiteSpace(value);
        }
    }

   
    [RelayCommand]
    private void AddCategory()
    {
        if (Categories.Contains(CategoryText, new CaseInsensitiveStringComparer()))
        {
            ShowMessage("","Category already exists.");
            return;
        }

        Categories.Add(CategoryText);
        CategoryTableDb.Add(CategoryText);
        IsAddButtonEnabled = false;
        CategoryText = "";

        if (Categories.Count > 1)
            AddMessage = "Below expense categories were saved.";
        else if (Categories.Count == 1)
            AddMessage = "Below expense category was saved.";
        else
            AddMessage = "";
    }

    [RelayCommand]
    private void Done()
    {
        AppSettings.Account.OnBoardingCompleted = true;
        AppSettings.Account.StartDateHome = DateTime.Now;
        AppSettings.Account.StartDateDaily = DateTime.Now;
        AppSettings.Account.StartDateRange = DateTime.Now;
        _navigation.Home();
    }
}

public class CaseInsensitiveStringComparer : IEqualityComparer<string>
{
    public bool Equals(string? x, string? y)
    {
        // Handle nulls
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        // Compare ignoring case
        return string.Equals(x, y, StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode(string obj)
    {
        // Handle null
        return obj?.ToLowerInvariant().GetHashCode() ?? 0;
    }
}
