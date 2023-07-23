using Boucher_DoubleModel.Models.Entitys;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boucher_Double_Front.ViewModel
{
    public class AllSelloutModel
    {
        public List<Sellout> AwaitingSellouts { get; set; }

        public async Task<List<Sellout>> GetAllSelloutAsync()
        {
            App appInstance = Application.Current as App;
            HttpClient client = await appInstance.PrepareQuery();
            HttpResponseMessage response = await client.GetAsync($"Sellout");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                AwaitingSellouts = JsonConvert.DeserializeObject<List<Sellout>>(jsonString).Where(sellout => sellout.SelloutDate > DateTime.Now).ToList();
                return AwaitingSellouts;
            }
            else
            {
                await Shell.Current.DisplayAlert("Erreur", "Erreur de connexion avec le server", "Ok");
                return default;
            }

            
        }
    }
}
