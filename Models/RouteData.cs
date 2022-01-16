using System;
using System.Windows.Media;

namespace MapFollow.Models
{
    public class RouteData
    {
        public double X  { get; set; }

        public double Y { get; set; }

        public int Vehicle_Id { get; set; }

        public Vehicule Vehicule { get; set; }
    }
}
