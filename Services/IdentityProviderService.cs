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
    class IdentityProviderService
    {
        public static async Task<bool> LoginUser(User newUser)
        {
            var uri = "https://localhost:44309/api/User/authenticate";
            HttpClient httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(newUser);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(uri, data).ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }
        public static async Task<bool> RegisterUser(User newUser)
        {
            var uri = "https://localhost:44309/api/User/register";
            HttpClient httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(newUser);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(uri, data).ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }
    }
}
