using Boucher_DoubleModel.Models.Entitys;
using Boucher_DoubleModel.Models;
using Microsoft.Maui.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boucher_Double_Front.ViewModel
{
    public class AddUserModel
    {
        public bool IsSuperAdmin
        {
            get
            {
                App app = Application.Current as App;
                return app.User.Role >= Role.SUPERADMIN;
            }
        }

        public bool IsEditable { get; set; } = true;
        public List<Store> Stores { get; set; }
        public Store NewStore { get; set; } = new();

        public User NewUser { get; set; }=new();


        public async Task GetAllStoreAsync()
        {
            if (IsSuperAdmin)
            {
                App appInstance = Application.Current as App;
                HttpClient client = await appInstance.PrepareQuery();
                HttpResponseMessage response = await client.GetAsync("Store");
                if (response.IsSuccessStatusCode)
                {
                    string jsonContent = await response.Content.ReadAsStringAsync();
                    List<Store> stores = JsonConvert.DeserializeObject<List<Store>>(jsonContent);
                    stores.Insert(0, new Store { Name = "Aucune de ces options" });
                    Stores = stores;
                }
                else
                    throw new Exception("Servor access error");
            }
        }
    }
}
