using System;
using OxyPlot;

namespace ExpenseTracker.ExtensionMethods;

public static class ColorExtension
{
    public static OxyColor ToOxyColor(this Color mauiColor)
    {
        return OxyColor.FromArgb(
            (byte)(mauiColor.Alpha * 255), // Alpha component  
            (byte)(mauiColor.Red * 255),   // Red component  
            (byte)(mauiColor.Green * 255), // Green component  
            (byte)(mauiColor.Blue * 255)    // Blue component  
        );
    }
}
