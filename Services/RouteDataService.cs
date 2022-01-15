using MapFollow.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WpfMap.Services
{
    public class RouteDataService
    {
        public static async Task AddRouteDataAsync(RouteData newRouteData)
        {
            var uri = "https://localhost:44309/RouteData";
            HttpClient httpClient = new HttpClient();
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(newRouteData);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(uri, data);
        }
        public static async Task<List<RouteData>> GetRouteDataAsync()
        {
            var uri = "https://localhost:44309/RouteData";
            HttpClient httpClient = new HttpClient();

            var response = httpClient.GetAsync(uri).Result.Content.ReadAsStringAsync().Result;
            return  JsonConvert.DeserializeObject<List<RouteData>>(response);
        }
    }
}
