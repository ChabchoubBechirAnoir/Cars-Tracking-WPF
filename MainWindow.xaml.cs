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
        public Guid guid = Guid.NewGuid();
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
            mapView.SetPositionByKeywords("Tunisie");
            mapView.Zoom = 7;
            mapView.ShowCenter = true;
            var routesInDb = RouteDataService.GetRouteDataAsync().Result;
            foreach (var route in routesInDb)
            {
                GMapMarker marker = new GMapMarker(new PointLatLng(route.X, route.Y));
                marker.Shape = new Ellipse
                {
                    Width = 10,
                    Height = 10,
                    Stroke = Brushes.DarkRed,
                    StrokeThickness = 1.5
                };
                mapView.Markers.Add(marker);
            }
            var lineRoute = DrawRoute();
            mapView.Markers.Clear();
            mapView.Markers.Add(lineRoute);
        }
        private void ButtonAddName_Click(object sender, RoutedEventArgs e)
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
                Id = mapView.Markers.Count.ToString(),
                X = lat,
                Y = lng,
                Vehicle_Id = guid
            };

            RouteDataService.AddRouteDataAsync(newRouteData);
            if (mapView.Markers.Count % 10 == 0)
            {
                DrawRoute();
            }
        }
        private GMapRoute DrawRoute()
        {
            var locations = mapView.Markers.Where(m => m.Tag != "Route").Select(m => m.Position).TakeLast(10).ToList();
            GMapRoute gmRoute = new GMapRoute(locations);
            gmRoute.Tag = "Route";
            CalculateDistance();
            //mapView.Markers.Clear();
            mapView.Markers.Add(gmRoute);
            return gmRoute;
        }
        private void CalculateDistance()
        {
            double distance = 0;
            for (var i = 1 ; i < mapView.Markers.Where(m => m.Tag != "Route").ToList().Count; i++)
            {
                var initial = mapView.Markers[i].Position;

                var final = mapView.Markers[i-1].Position;
                distance += MathCalculation.distance(initial.Lat, final.Lat, initial.Lng, final.Lng);
            }
            txtLocation.Text = distance.ToString();
        }
    }
}
