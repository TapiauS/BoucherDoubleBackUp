using Boucher_DoubleModel.Models.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui;
using Microsoft.Maui.Controls.Xaml;
using Newtonsoft.Json;
using Boucher_Double_Front.ViewModel;
using System.Text.RegularExpressions;

namespace Boucher_Double_Front.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [QueryProperty(nameof(ClientId), nameof(ClientId))]
    public partial class UpdateClient : ContentPage
    {
        private UpdateClientModel model = new();
        private string clientId;
        public string ClientId 
        { 
            get=>clientId;
            set 
            { 
                clientId = value;
                try
                {
                    Task.Run(async () => await model.LoadOneClientAsync(int.Parse(clientId))).Wait();
                }
                catch(Exception ex) 
                {
                    Shell.Current.DisplayAlert("Erreur", "Erreur d'accés au serveur", "OK");
                }
                BindingContext = model;
            } 
        }

        public UpdateClient()
        {
            Resources = StyleDictionnary.GetInstance();
            InitializeComponent();
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            Shell.Current.Navigation.RemovePage(this);
        }


        private bool FirstAppear = true;
        protected override async void OnAppearing()
        {
            AppShell appShell = Shell.Current as AppShell;
            if (FirstAppear)
            {
                if (await appShell.AskLogin())
                {
                    base.OnAppearing();
                    FirstAppear = false;
                }
                else
                {
                    FirstAppear = false;
                    await appShell.GoToAsync(nameof(Home));
                }
            }
        }

        public void MailTextChanged(object sender, EventArgs e)
        {
            Entry entry = sender as Entry;
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

        public async void OnValidateAsync(object sender,EventArgs args)
        {
            App app = Application.Current as App;
            string phoneNumberRegex = "^\\d{10}$";
            string mailRegex = "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$";
            if (model.Client.Name != "" && model.Client.Surname != "" && Regex.IsMatch(model.Client.PhoneNumber, phoneNumberRegex) && Regex.IsMatch(model.Client.Mail, mailRegex))
            {
                HttpClient httpClient = await app.PrepareQuery();
                string json = JsonConvert.SerializeObject(model.Client);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage httpResponse = await httpClient.PutAsync("Client", content);
                string jsonResult = await httpResponse.Content.ReadAsStringAsync();
                if (httpResponse.IsSuccessStatusCode)
                {
                    if (bool.Parse(jsonResult))
                        await Shell.Current.GoToAsync("..");
                    else
                        await Shell.Current.DisplayAlert("Erreur", "Mail non disponible", "Ok");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erreur", "Erreur d'accés au serveur", "Ok");
                }
            }
            else
                await Shell.Current.DisplayAlert("Erreur", "Certains champs sont incorrect , ils sont tous obligatoire", "Ok");
        }

        public async void OnStopAsync(object sender,EventArgs args)
        {
            await Shell.Current.GoToAsync("..");
        }


    }
}