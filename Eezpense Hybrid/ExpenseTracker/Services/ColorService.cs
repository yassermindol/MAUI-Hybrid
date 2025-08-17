using System;

namespace ExpenseTracker.Services;

public class ColorService
{
    List<Color> ColorList = new List<Color>();
    public ColorService()
    {
        /*
        Colors.Add(Color.Red);
        Colors.Add(Color.Orange);
        Colors.Add(Color.Yellow);
        Colors.Add(Color.Green);
        Colors.Add(Color.Blue);
        Colors.Add(Color.Indigo);
        Colors.Add(Color.Violet);
        */

        ColorList.Add(Colors.Aquamarine); //1          
        ColorList.Add(Colors.PaleGreen); //2
        ColorList.Add(Colors.GreenYellow); //3
        ColorList.Add(Colors.LawnGreen);//4                    
        ColorList.Add(Colors.Lime);//5
        ColorList.Add(Colors.MediumSpringGreen);//6
        ColorList.Add(Colors.LimeGreen);//7
        ColorList.Add(Colors.LimeGreen);//8          
        ColorList.Add(Colors.MediumSeaGreen);//9            
        ColorList.Add(Colors.MediumSeaGreen);//10
        ColorList.Add(Colors.ForestGreen);//11
        ColorList.Add(Colors.Green);//12      
        ColorList.Add(Colors.DarkGreen); //13



        ColorList.Add(Colors.LightSeaGreen);//14            

        ColorList.Add(Colors.SeaGreen); //15


        ColorList.Add(Colors.DarkSeaGreen); //16
        ColorList.Add(Colors.DarkOliveGreen);//17
        ColorList.Add(Colors.YellowGreen);
        ColorList.Add(Colors.Beige);
        ColorList.Add(Colors.Khaki);
        ColorList.Add(Colors.Olive);
        ColorList.Add(Colors.DarkOliveGreen);

        ColorList.Add(Colors.Blue);
        ColorList.Add(Colors.Azure);
        ColorList.Add(Colors.SteelBlue);
        ColorList.Add(Colors.Aqua);
        ColorList.Add(Colors.Turquoise);

        ColorList.Add(Colors.Ivory);
        ColorList.Add(Colors.LemonChiffon);
        ColorList.Add(Colors.Yellow);
        ColorList.Add(Colors.Gold);
        ColorList.Add(Colors.MintCream);
        ColorList.Add(Colors.Goldenrod);
        ColorList.Add(Colors.Bisque);
        ColorList.Add(Colors.Tan);
        ColorList.Add(Colors.Orange);
        ColorList.Add(Colors.Salmon);

        ColorList.Add(Colors.OrangeRed);
        ColorList.Add(Colors.Sienna);
        ColorList.Add(Colors.PeachPuff);
        ColorList.Add(Colors.IndianRed);
        ColorList.Add(Colors.Pink);
        ColorList.Add(Colors.Red);
        ColorList.Add(Colors.Maroon);
        ColorList.Add(Colors.MistyRose);
        ColorList.Add(Colors.RosyBrown);
        ColorList.Add(Colors.RosyBrown);

        ColorList.Add(Colors.Crimson);
        ColorList.Add(Colors.Magenta);
        ColorList.Add(Colors.Plum);
        ColorList.Add(Colors.Violet);
        ColorList.Add(Colors.Fuchsia);
        ColorList.Add(Colors.Purple);
        ColorList.Add(Colors.Lavender);
        ColorList.Add(Colors.BlueViolet);
        ColorList.Add(Colors.Indigo);
        ColorList.Add(Colors.SkyBlue);
    }

    public IList<Color> GenerateColors(int numberOfColors)
    {
        return ColorList;
    }

    public Color GetColor(int colorIndex)
    {
        int lastIndex = ColorList.Count - 1;
        if (colorIndex > lastIndex)
            colorIndex = (colorIndex - lastIndex) - 1;

        try
        {
            var color = ColorList[colorIndex];
            return color;
        }
        catch
        {
            return GetRandomColor();
        }
    }


    private Color GetRandomColor()
    {
        var random = new Random();
        int index = random.Next(ColorList.Count - 1);
        return ColorList[index];
    }
}