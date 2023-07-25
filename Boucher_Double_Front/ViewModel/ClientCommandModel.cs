using Boucher_DoubleModel.Models.Entitys;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boucher_Double_Front.ViewModel
{
    public class ClientCommandModel
    {
        public List<SelloutLine> Lines
        {
            get
            {
                App appInstance = Application.Current as App;
                if (appInstance.ActivCommand != null)
                    return appInstance.ActivCommand.BuyedProducts;
                return null;
            }
        }

        public List<Event> CompatibleEvents { get; set; }

        public async Task GetAllEventAsync()
        {
            App app = Application.Current as App;
            HttpClient client = await app.PrepareQuery();
            HttpResponseMessage response = await client.GetAsync("Event");
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                List<Event> _events = JsonConvert.DeserializeObject<List<Event>>(json);
                List<Event> filteredEvents = _events.Where(_event => _event.End > app.ActivCommand.SelloutDate&&_event.Begin<app.ActivCommand.ReceiptDate).ToList() ;
                filteredEvents.Insert(0, new() { Name = "Aucune de ces options" });
                CompatibleEvents = filteredEvents;
            }
            else
                await Shell.Current.DisplayAlert("Erreur", "Erreur d'accés au serveur", "Ok");
        }
    }
}
