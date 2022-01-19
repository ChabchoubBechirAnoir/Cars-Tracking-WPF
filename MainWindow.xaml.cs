using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
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
            InitTimer();
        }
        public void mapView_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.mapCenter.HasValue)
            {
                mapView.SetPositionByKeywords("Paris");
                mapView.Zoom = 14;
            }
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            mapView.MapProvider = OpenStreetMapProvider.Instance;
            mapView.MinZoom = 14;
            mapView.MaxZoom = 17;
            mapView.MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter;
            mapView.CanDragMap = true;
            mapView.DragButton = MouseButton.Left;
            GMaps.Instance.Mode = AccessMode.ServerOnly;

            if (App.mapCenter.HasValue && mapView.CenterPosition != App.mapCenter.Value)
            {
                mapView.CenterPosition = App.mapCenter.Value;
            }
  


            mapView.ShowCenter = true;
            DrawAll();
        }
        public void DrawAll()
        {
            var routesInDb = RouteDataService.GetRouteDataAsync().Result;
            routesInDb = routesInDb.Where(m => m.Vehicle_Id != 99).ToList();

            DrawPolygon();
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
            lbTodoList.ItemsSource = routesInDb.TakeLast(10).Select(e => $"{e.Vehicule.Name} is at {e.X.ToString("N3")} {e.Y.ToString("N3")}");
        }
        public void CleanAll()
        {
            while (mapView.Markers?.FirstOrDefault() != null)
            {
                mapView.Markers.Remove(mapView.Markers.FirstOrDefault());
            }
        }
        private Timer timer1;
        public void InitTimer()
        {
            timer1 = new Timer();
            timer1.Elapsed += new ElapsedEventHandler(timer1_Tick);
            timer1.Interval = 1000; // in miliseconds
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                CleanAll();
                DrawAll();
                mapView_Loaded(sender, new RoutedEventArgs());
            });
        }
        private void DrawPolygon()
        {
            var routesInDb = RouteDataService.GetRouteDataAsync().Result;
            routesInDb = routesInDb.Where(m => m.Vehicle_Id == 99).ToList();

            List<PointLatLng> points = new List<PointLatLng>();
            if (routesInDb.Count == 4)
            {
                points.Add(new PointLatLng(routesInDb[0].X, routesInDb[0].Y));
                points.Add(new PointLatLng(routesInDb[1].X, routesInDb[1].Y));
                points.Add(new PointLatLng(routesInDb[2].X, routesInDb[2].Y));
                points.Add(new PointLatLng(routesInDb[3].X, routesInDb[3].Y));
                GMapPolygon polygon = new GMapPolygon(points);
                mapView.Markers.Add(polygon);
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
                Vehicle_Id = App.vehiculeId
            };

            _ = RouteDataService.AddRouteDataAsync(newRouteData);
        }
        private void DrawRoute()
        {
            var vehicles = VehicleService.GetVehicules().Result;
            List<Button> buttons = new List<Button>();
            foreach (var vehicle in vehicles)
            {
                var locations = mapView.Markers.Where(m => m.Tag!= null && (int)m.Tag == vehicle.Id)?.Select(m => m.Position)?.ToList();
                buttons.Add(new Button { ButtonContent = vehicle.Name, ButtonID = vehicle.Id, ButtonColor = vehicle.Color, ButtonDistance = CalculateDistance(vehicle.Id).ToString("N2") + "Km", Vehicule = vehicle, pointLatLng = locations.Last()  });

                if (locations!= null && locations.Count != 0)
                {
                    GMapRoute gmRoute = new GMapRoute(locations);
                    gmRoute.Tag = 100;
                    gmRoute.Shape = new Path
                    {
                        Stroke = vehicle.FromColorToBrushes(),
                        StrokeThickness = 4,
                        StrokeEndLineCap = PenLineCap.Triangle
                    };
                    CalculateDistance(vehicle.Id);
                    //mapView.Markers.Clear();
                    mapView.Markers.Add(gmRoute);

                    GMapMarker marker = new GMapMarker(new PointLatLng(locations.Last().Lat, locations.Last().Lng));
                    marker.Offset = new Point(-10, -5);
                    marker.Shape = new Ellipse
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = 4,
                        Width = 20,
                        Height = 20,
                        Fill = Brushes.Black
                    };
                    marker.Tag = 100;
                    mapView.Markers.Add(marker);

                    GMapMarker marker1 = new GMapMarker(new PointLatLng(locations.First().Lat, locations.First().Lng));
                    marker1.Offset = new Point(-10, -5);
                    marker1.Shape = new Ellipse
                    {
                        Stroke = Brushes.White,
                        StrokeThickness = 4,
                        Width = 20,
                        Height = 20,
                        Fill = Brushes.White
                    };
                    marker1.Tag = 100;
                    mapView.Markers.Add(marker1);
                }
            }
            ic.ItemsSource = buttons;
        }
        public double CalculateDistance(int vehicleId)
        {
            double distance = 0;
            var markers = mapView.Markers.Where(m => m.Tag != null && (int)m.Tag != 100 && (int)m.Tag == vehicleId).ToList();
            for (var i = 1 ; i < markers.Count; i++)
            {
                var initial = markers[i].Position;

                var final = markers[i-1].Position;
                distance += MathCalculation.distance(initial.Lat, final.Lat, initial.Lng, final.Lng);
            }
            return distance;
        }
    }
    public class Button
    {
        public string ButtonContent { get; set; }
        public int ButtonID { get; set; }
        public string ButtonColor { get; set; }
        public string ButtonDistance { get; set; }
        public Vehicule Vehicule { get; set; }

        private ICommand _saveCommand;

        public PointLatLng pointLatLng;

        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(
                        param => this.SaveObject(),
                        param => this.CanSave()
                    );
                }
                return _saveCommand;
            }
        }

        private bool CanSave()
        {
            return true;
        }

        private void SaveObject()
        {

            App.mapCenter = pointLatLng;
            App.vehiculeId = ButtonID;
        }
    }
}
