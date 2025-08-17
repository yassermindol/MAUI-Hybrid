namespace ExpenseTracker.Controls;

public class CustomDatePicker : DatePicker
{
    public static readonly BindableProperty HorizontalTextAlignmentProperty
        = BindableProperty.Create
        (
            propertyName: "HorizontalTextAlignment",
            returnType: typeof(TextAlignment),
            declaringType: typeof(CustomDatePicker),
            defaultValue: default(TextAlignment)
        );

    public TextAlignment HorizontalTextAlignment
    {
        get { return (TextAlignment)GetValue(HorizontalTextAlignmentProperty); }
        set { SetValue(HorizontalTextAlignmentProperty, value); }
    }


    public static readonly BindableProperty PlaceholderProperty
        = BindableProperty.Create
        (
            propertyName: "Placeholder",
            returnType: typeof(string),
            declaringType: typeof(CustomDatePicker),
            defaultValue: default(string)
        );
    public string Placeholder
    {
        get { return (string)GetValue(PlaceholderProperty); }
        set { SetValue(PlaceholderProperty, value); }
    }
}
