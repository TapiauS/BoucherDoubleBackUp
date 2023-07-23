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
                Task.Run(async ()=>await model.LoadOneClientAsync(int.Parse(clientId))).Wait();
                BindingContext = model;
            } 
        }

        public UpdateClient()
        {
            InitializeComponent();

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

        public async void OnValidateAsync(object sender,EventArgs args)
        {
            App app = Application.Current as App;
            HttpClient httpClient =await app.PrepareQuery();
            string json=JsonConvert.SerializeObject(model.Client);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage httpResponse=await httpClient.PutAsync("Client", content);
            string jsonResult=await httpResponse.Content.ReadAsStringAsync();
            if(httpResponse.IsSuccessStatusCode) 
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

        public async void OnStopAsync(object sender,EventArgs args)
        {
            await Shell.Current.GoToAsync("..");
        }


    }
}