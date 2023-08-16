using Boucher_DoubleModel.Models.Entitys;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boucher_Double_Front.ViewModel
{
    class EventManagerModel
    {
        public List<Event> ExistingsOption { get; set; }

        public Event ActivEvent { 
            get; 
            set; } 

        public async Task GetAllEvent()
        {
            App app =Application.Current as App;
            HttpClient client=await app.PrepareQuery();
            HttpResponseMessage response=await client.GetAsync("Event");
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                List<Event> _events = JsonConvert.DeserializeObject<List<Event>>(json);
                _events.Insert(0, new() { Name = "Aucune de ces options" });
                ExistingsOption = _events;
            }
            else
                throw new Exception("Server access error");
        }

    }
}
