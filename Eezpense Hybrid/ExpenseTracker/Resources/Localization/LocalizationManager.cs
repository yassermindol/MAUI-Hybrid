using System.ComponentModel;
using System.Globalization;

namespace ExpenseTracker.Resources.Localization;

public class LocalizationManager : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private LocalizationManager()
    {
        AppResources.Culture = CultureInfo.CurrentCulture;
    }
    private static readonly Lazy<LocalizationManager> instance = new(() => new LocalizationManager());
    public static LocalizationManager Instance => instance.Value;

    public object this[string key] => AppResources.ResourceManager.GetObject(key, AppResources.Culture) ?? Array.Empty<byte>();

    //Use this when we want to have our own language selection in the app. Get first the selected language see example below
    //For now we use the phone's setting for the language preference.
    public void SetLanguage(string selectedLanguage)
    {
        CultureInfo culture;
        if (selectedLanguage == "English")
            culture = new System.Globalization.CultureInfo("en-US");
        else
            culture = new CultureInfo("es-ES");

        AppResources.Culture = culture;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
    }
}
