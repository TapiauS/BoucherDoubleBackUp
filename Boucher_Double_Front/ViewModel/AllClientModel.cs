using Boucher_DoubleModel.Models.Entitys;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boucher_Double_Front.ViewModel
{
    public class AllClientModel
    {
        public List<Client> Clients { get; set; }

        public async Task GetAllClientAsync()
        {
            App app = Application.Current as App;
            HttpClient client = await app.PrepareQuery();
            HttpResponseMessage response = await client.GetAsync("Client");
            if (response.IsSuccessStatusCode)
            {
                string jsonContent = await response.Content.ReadAsStringAsync();
                Clients = JsonConvert.DeserializeObject<List<Client>>(jsonContent);
            }
            else
            {
                throw new Exception("Servor access error");
            }
        }
    }
}
