using System.Windows.Input;

namespace ExpenseTracker.Controls;

public partial class CustomPickerView : ContentView
{
    public CustomPickerView()
    {
        InitializeComponent();
        // Set binding context to itself for easy binding
        BindingContext = this;
    }

    public static readonly BindableProperty SelectedTextProperty =
        BindableProperty.Create(nameof(SelectedText), typeof(string), typeof(CustomPickerView), default(string));

    public static readonly BindableProperty TapCommandProperty =
        BindableProperty.Create(nameof(TapCommand), typeof(ICommand), typeof(CustomPickerView), default(ICommand));

    public string SelectedText
    {
        get => (string)GetValue(SelectedTextProperty);
        set => SetValue(SelectedTextProperty, value);
    }

    public ICommand TapCommand
    {
        get => (ICommand)GetValue(TapCommandProperty);
        set => SetValue(TapCommandProperty, value);
    }
}