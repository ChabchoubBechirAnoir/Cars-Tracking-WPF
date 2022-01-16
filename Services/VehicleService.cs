using MapFollow.Models;
using System;
using System.Collections.Generic;
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
    public class VehicleService
    {
        public static async Task AddVehiculeAsync(Vehicule newVehicule)
        {
            var uri = "https://localhost:44309/Vehicule";
            HttpClient httpClient = new HttpClient();
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(newVehicule);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(uri, data).ConfigureAwait(false);
        }
        public static async Task<List<Vehicule>> GetVehicules()
        {
            var uri = "https://localhost:44309/Vehicule";
            HttpClient httpClient = new HttpClient();

            var response = httpClient.GetAsync(uri).Result.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<List<Vehicule>>(response);
        }
    }
}
