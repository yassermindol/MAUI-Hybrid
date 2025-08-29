using System;
using OxyPlot;

namespace ExpenseTracker.ExtensionMethods;

public static class ColorExtensions
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

    public static string ToCssHex(this Color color)
    {
        if (color is null)
            return "red";
        try
        {
            int r = (int)(color.Red * 255);
            int g = (int)(color.Green * 255);
            int b = (int)(color.Blue * 255);
            int a = (int)(color.Alpha * 255);
            // If fully opaque, omit alpha (#RRGGBB)
            if (a == 255)
            {
                return $"#{r:X2}{g:X2}{b:X2}";
            }
            else
            {
                // Include alpha if not fully opaque (#RRGGBBAA)
                return $"#{r:X2}{g:X2}{b:X2}{a:X2}";
            }
        }
        catch (Exception e)
        {
            throw;
        }      
    }
}
