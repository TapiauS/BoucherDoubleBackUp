using Boucher_DoubleModel.Models.Entitys;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boucher_Double_Front.ViewModel
{
    public class AddSelloutClientModel
    {

        public Client Client { get; set; } = new () ;
        public Sellout Sellout { get; set; } = new Sellout();
        public List<Client> Clients { get; set; }

        public bool IsEditable { get=>Client.Id == 0; }

        public AddSelloutClientModel() 
        {
            App app = Application.Current as App;
            if (app.ActivCommand != null)
            {
                Client = app.ActivCommand.Client;
            }
        }
        public async Task GetAllClientAsync()
        {
            App appInstance = Application.Current as App;
            HttpClient client = await appInstance.PrepareQuery();

            HttpResponseMessage httpResponse = await client.GetAsync("Client");
            if (httpResponse.IsSuccessStatusCode)
            {
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                Clients = JsonConvert.DeserializeObject<List<Client>>(jsonString);
                Clients.Insert(0, new Client() { PhoneNumber = "Aucune selection" });
            }
            else
                throw new Exception("Servor access error");
        }

    }
}
