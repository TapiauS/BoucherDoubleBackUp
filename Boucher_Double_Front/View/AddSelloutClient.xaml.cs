using Boucher_DoubleModel.Models.Entitys;
using Newtonsoft.Json;
using System.Text;
using Boucher_Double_Front.ViewModel;
using System.Collections.ObjectModel;

namespace Boucher_Double_Front.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddSelloutClient : ContentPage
    {
        private AddSelloutClientModel model=new();
        public AddSelloutClient()
        {
            App appCurrent = Application.Current as App;
            model.Sellout = new Sellout() { Client = model.Client, ReceiptDate = DateTime.Now, SelloutDate=DateTime.Now,Store=appCurrent.User.Store };
            InitializeComponent();
            BindingContext = model;
        }

        public async void OnValidateAsync(object sender, EventArgs args)
        {
            App appCurrent = Application.Current as App;
            if ((model.Client.Mail!=null&&model.Client.PhoneNumber!=null&&model.Client.Surname!=null&&model.Client.Name!=null) && model.Sellout.SelloutDate > model.Sellout.ReceiptDate) 
            {
                HttpClient client=await appCurrent.PrepareQuery();
                string json = JsonConvert.SerializeObject(model.Client);
                StringContent content = new(json, Encoding.UTF8, "application/json");
                if (model.Client.Id == 0)
                {
                    HttpResponseMessage response = await client.PostAsync("Client", content);
                    var jsonString = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode&&int.Parse(jsonString)>0&&model.Sellout.SelloutDate>model.Sellout.ReceiptDate) 
                    {
                        model.Client.Id = int.Parse(jsonString);
                        model.Sellout.Client = model.Client;
                        appCurrent.ActivCommand = model.Sellout;
                        await Shell.Current.GoToAsync($"{nameof(ClientCommand)}");
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Erreur", "Erreur Inconnue", "OK");
                        return;
                    }
                }
                else
                {
                    model.Sellout.Client = model.Client;
                    appCurrent.ActivCommand = model.Sellout;
                    await Shell.Current.GoToAsync($"{nameof(ClientCommand)}");
                }
            }
        }   

    }
}