using Boucher_DoubleModel.Models.Entitys;
using Newtonsoft.Json;
using System.Text;
using Boucher_Double_Front.ViewModel;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

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
            Resources = StyleDictionnary.GetInstance();
            InitializeComponent();
            BindingContext = model;
        }

        public void MailTextChanged(object sender, EventArgs e)
        {
            Entry entry=sender as Entry;
            string mailRegex = "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$";
            if (entry != null)
            {
                if (Regex.IsMatch(entry.Text, mailRegex))
                    entry.BackgroundColor = Color.FromRgba(255, 255, 0, 0.5);
                else
                    entry.BackgroundColor = Color.FromRgba(255, 0, 0, 0.5);
            }
        }

        public void OnPhoneNumberChanged(object sender, EventArgs e)
        {
            Entry entry = sender as Entry;
            string phoneNumberRegex = "^\\d{10}$";
            if (entry != null)
            {
                if (Regex.IsMatch(entry.Text, phoneNumberRegex))
                    entry.BackgroundColor = Color.FromRgba(255, 255, 0, 0.5);
                else
                    entry.BackgroundColor = Color.FromRgba(255, 0, 0, 0.5);
            }
        }

        public async void OnValidateAsync(object sender, EventArgs args)
        {
            App appCurrent = Application.Current as App;
            string phoneNumberRegex = "^\\d{10}$";
            string mailRegex = "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$";
            if (Regex.IsMatch(model.Client.Mail,mailRegex)&&Regex.IsMatch(model.Client.PhoneNumber,phoneNumberRegex)&&model.Client.Surname!=null&&model.Client.Name!=null&& model.Sellout.SelloutDate > model.Sellout.ReceiptDate) 
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
            else
            {
                await Shell.Current.DisplayAlert("Erreur de saisie", "Un des champ est non valide,ils sont tous obligatoire", "Ok");
            }
        }   

    }
}