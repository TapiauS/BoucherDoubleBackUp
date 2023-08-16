using Boucher_DoubleModel.Models.Entitys;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boucher_Double_Front.ViewModel
{
    public class UpdateClientModel
    {
        public Client Client { get; set; }
        public async Task LoadOneClientAsync(int clientId)
        {
            App app = Application.Current as App;
            HttpClient httpClient = await app.PrepareQuery();
            HttpResponseMessage response = await httpClient.GetAsync($"Client/{clientId}");
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                Client = JsonConvert.DeserializeObject<Client>(json);
            }
            else
            {
                throw new Exception("Server error");
            }
        }
    }
}
