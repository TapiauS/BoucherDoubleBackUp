using Boucher_DoubleModel.Models.Entitys;
using Microsoft.Maui.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace Boucher_Double_Front.ViewModel
{
    public class BillManagerModel
    {
        public BillParameter BillOption { get; set; } = new BillParameter();
        public List<BillParameter> ExistingsOption { get; set; } = new();

        public async Task GetAllOptionsAsync()
        {
            App appInstance = Application.Current as App;
            HttpClient client = await appInstance.PrepareQuery();
            HttpResponseMessage response = await client.GetAsync("BillParameter");
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                List<BillParameter> bills = JsonConvert.DeserializeObject<List<BillParameter>>(json);
                ExistingsOption = JsonConvert.DeserializeObject<List<BillParameter>>(json);
                ExistingsOption.Insert(0, new BillParameter() { Name = "Aucune option" });
            }
            else
            {
                throw new Exception("Server access error");
            }
        }

    }
}
