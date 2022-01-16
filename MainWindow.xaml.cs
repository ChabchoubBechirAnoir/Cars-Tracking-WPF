using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using MapFollow.Models;
using Microsoft.Maps.MapControl.WPF;
using WpfMap.Services;

namespace WpfMap
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void mapView_Loaded(object sender, RoutedEventArgs e)
        {
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            mapView.MapProvider = OpenStreetMapProvider.Instance;
            mapView.MinZoom = 2;
            mapView.MaxZoom = 17;
            mapView.Zoom = 2;
            mapView.MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter;
            mapView.CanDragMap = true;
            mapView.DragButton = MouseButton.Left;
            GMaps.Instance.Mode = AccessMode.ServerOnly;
            mapView.SetPositionByKeywords("Paris");
            mapView.Zoom = 12;
            mapView.ShowCenter = true;
            var routesInDb = RouteDataService.GetRouteDataAsync().Result;
            foreach (var route in routesInDb)
            {
                GMapMarker marker = new GMapMarker(new PointLatLng(route.X, route.Y));
                marker.Shape = new Ellipse
                {
                    Width = 10,
                    Height = 10,
                    Stroke = route.Vehicule.FromColorToBrushes(),
                    StrokeThickness = 1.5
                };
                marker.Tag = route.Vehicule.Id;
                mapView.Markers.Add(marker);
            }
            DrawRoute();
            while (mapView.Markers.FirstOrDefault(m => (int)m.Tag != 100) != null)
            {
                mapView.Markers.Remove(mapView.Markers.FirstOrDefault(m => (int)m.Tag != 100));
            }
        }
        private void GoToLocation(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtLocation.Text))
            {
                mapView.SetPositionByKeywords(txtLocation.Text);
            }
        }
        private void MapWithPushpins_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            double lat = mapView.FromLocalToLatLng(((int)e.GetPosition(this).X) + 1, ((int)e.GetPosition(this).Y) + 1).Lat;
            double lng = mapView.FromLocalToLatLng(((int)e.GetPosition(this).X) + 1, ((int)e.GetPosition(this).Y) + 1).Lng;

            GMapMarker marker = new GMapMarker(new PointLatLng(lat, lng));
            marker.Shape = new Ellipse
            {
                Width = 10,
                Height = 10,
                Stroke = Brushes.DarkRed,
                StrokeThickness = 1.5
            };
            mapView.Markers.Add(marker);
            var newRouteData = new RouteData()
            {
                X = lat,
                Y = lng,
                Vehicle_Id = 5
            };

            RouteDataService.AddRouteDataAsync(newRouteData);
        }
        private void DrawRoute()
        {
            var vehicles = VehicleService.GetVehicules().Result;
            foreach (var vehicle in vehicles)
            {
                var locations = mapView.Markers.Where(m => (int)m.Tag == vehicle.Id)?.Select(m => m.Position)?.ToList();
                if (locations!= null && locations.Count != 0)
                {
                    GMapRoute gmRoute = new GMapRoute(locations);
                    gmRoute.Tag = 100;
                    gmRoute.Shape = new Path
                    {
                        Width = 10,
                        Height = 10,
                        Stroke = vehicle.FromColorToBrushes(),
                        StrokeThickness = 1.5
                    };
                    CalculateDistance();
                    //mapView.Markers.Clear();
                    mapView.Markers.Add(gmRoute);
                }
            }
        }
        private void CalculateDistance()
        {
            double distance = 0;
            for (var i = 1 ; i < mapView.Markers.Where(m => (int)m.Tag != 100).ToList().Count; i++)
            {
                var initial = mapView.Markers[i].Position;

                var final = mapView.Markers[i-1].Position;
                distance += MathCalculation.distance(initial.Lat, final.Lat, initial.Lng, final.Lng);
            }
            txtLocation.Text = distance.ToString();
        }
    }
}
