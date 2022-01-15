using System;

namespace MapFollow.Models
{
    public class RouteData
    {
        public string Id { get; set; }

        public double X  { get; set; }

        public double Y { get; set; }

        public Guid Vehicle_Id { get; set; }
    }
}
