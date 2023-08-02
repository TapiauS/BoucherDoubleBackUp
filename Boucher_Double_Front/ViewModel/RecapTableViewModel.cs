using Boucher_Double_Front.Model;
using Boucher_DoubleModel.Models.Entitys;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boucher_Double_Front.ViewModel
{
    public class RecapTableViewModel
    {
        public List<IGrouping<int,CompletedSellout>> CompletedSellouts { get; set;}
        public List<Event> AllEvents { get; set; } = new();
        private List<Sellout> sellouts=new();
        private List<Sellout> filteredSellouts =new();

        public List<IGrouping<int,Product>> Products { get; set; } = new();
        private List<Product> products = new();
        public Dictionary<int,Client> IdClientPair { get; set; }=new();
        public Dictionary<int,Category> IdCategoryPair { get; set; }=new();   



        public Dictionary<int,int> Total { get;set; }=new();

        public RecapTableViewModel()
        {
            Task.Run(async () => await GetAllProductAsync()).Wait();
            Task.Run(async() =>await GetAllMenuAsync()).Wait();
            Task.Run(async() => await GetAllSelloutAsync()).Wait();
            Task.Run(async() => await GetAllEventAsync()).Wait();  
            DefineTableInfo();
        }

        private void DefineTableInfo()
        {
            CompletedSellouts = filteredSellouts.Select(sellout => new CompletedSellout(products, sellout)).GroupBy(completedsellout=>completedsellout.Sellout.Client.Id).ToList();
            foreach (var key in Total.Keys)
            {
                Total[key] = 0;
            }
            foreach (var selloutgroup in CompletedSellouts)
            {
                foreach(var sellout in selloutgroup)
                {
                    foreach (SelloutLine selloutLine in sellout.Lines)
                    {
                        Total[selloutLine.SoldProduct.Id] += selloutLine.Quantity;
                    }
                }
            }
        }

        public async Task GetAllMenuAsync()
        {
            App app = Application.Current as App;
            HttpClient client = await app.PrepareQuery();
            HttpResponseMessage httpResponse = await client.GetAsync("Menu");
            if (httpResponse.IsSuccessStatusCode)
            {
                string jsoncontent = await httpResponse.Content.ReadAsStringAsync();
                List<Menu> menus = JsonConvert.DeserializeObject<List<Menu>>(jsoncontent);
                List<IGrouping<int, Menu>> groupedMenus = menus.GroupBy(product => product.Category.Id).ToList() ;
                Products.AddRange(groupedMenus);
                foreach (var group in Products)
                {
                    products.AddRange(group);
                }
                foreach (Product product in products)
                {
                    Total.Add(product.Id, 0);
                    if(!IdCategoryPair.ContainsKey(product.Category.Id))
                        IdCategoryPair.Add(product.Category.Id, product.Category);
                }
            }
            else
            {
                await Shell.Current.DisplayAlert("Erreur", "Erreur Inconnue", "OK");
            }
        }
        public void Filter(Event? _event)
        {
            if(_event == null)
            {
                filteredSellouts = sellouts;
                DefineTableInfo();
            }
            else
            {
                filteredSellouts = sellouts.Where(sellout => sellout.Event == _event).ToList();
                DefineTableInfo();
            }
        }


        public async Task GetAllProductAsync()
        {
            App appInstance = Application.Current as App;
            HttpClient client = await appInstance.PrepareQuery();
            HttpResponseMessage response = await client.GetAsync("Product");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                Products.AddRange(JsonConvert.DeserializeObject<List<Product>>(jsonString).GroupBy(product => product.Category.Id));
            }
            else
            {
                await Shell.Current.DisplayAlert("Erreur", "Erreur Inconnue", "OK");
            }
        }

        public async Task GetAllEventAsync()
        {
            App app = Application.Current as App;
            HttpClient client = await app.PrepareQuery();
            HttpResponseMessage response = await client.GetAsync("Event");
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                List<Event> _events = JsonConvert.DeserializeObject<List<Event>>(json);
                _events.Insert(0, new() { Name = "Aucune de ces options" });
                AllEvents = _events;
            }
            else
                await Shell.Current.DisplayAlert("Erreur", "Erreur d'accés au serveur", "Ok");
        }
        public async Task GetAllSelloutAsync()
        {
            App appInstance = Application.Current as App;
            HttpClient client = await appInstance.PrepareQuery();
            HttpResponseMessage response = await client.GetAsync($"Sellout");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                sellouts = JsonConvert.DeserializeObject<List<Sellout>>(jsonString).Where(sellout => sellout.SelloutDate > DateTime.Now).ToList();
                filteredSellouts = sellouts;
                foreach(Sellout sellout in filteredSellouts)
                {
                    if(!IdClientPair.ContainsKey(sellout.Client.Id))
                        IdClientPair.Add(sellout.Client.Id,sellout.Client);
                }
            }
            else
            {
                await Shell.Current.DisplayAlert("Erreur", "Erreur de connexion avec le server", "Ok");
            }
        }
    }
}
