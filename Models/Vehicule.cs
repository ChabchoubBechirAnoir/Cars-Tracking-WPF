using System;
using System.Windows.Media;

namespace MapFollow.Models
{
    public class Vehicule
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Color { get; set; }

        public double TotalDistance { get; set; }

        public SolidColorBrush FromColorToBrushes()
        {
            switch (Color)
            {
                case "Red":
                    return Brushes.Red;
                    break;
                case "Blue":
                    return Brushes.Blue;
                    break;
                case "Green":
                    return Brushes.Green;
                    break;
                case "Black":
                    return Brushes.Black;
                    break;
                default:
                    return Brushes.Yellow;
                    break;
            }

        }
    }
}
