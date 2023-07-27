using Boucher_DoubleModel.Models.Entitys;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui;
using Microsoft.Maui.Controls.Xaml;
using Boucher_Double_Front.ViewModel;

namespace Boucher_Double_Front.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AllClient : ContentPage
    {
        public AllClientModel model = new();
        public AllClient()
        {
            Resources = StyleDictionnary.GetInstance();
            InitializeComponent();
            Task.Run(async ()=>await model.GetAllClientAsync()).Wait();
            allClient.ItemsSource = model.Clients;
            BindingContext = model;
        }



        public async void OnDeleteAsync(object sender, EventArgs e)
        {

            if (!await Shell.Current.DisplayAlert("Suppression", "Attention supprimer ce client supprimera toutes les commandes associée", "Annuler", "Valider"))
            {
                App app = Application.Current as App;
                HttpClient client = await app.PrepareQuery();
                Button button = (Button)sender;
                Client supressedclient = button.CommandParameter as Client;
                HttpResponseMessage response = await client.DeleteAsync($"Client/{supressedclient.Id}");
                if (response.IsSuccessStatusCode)
                {
                    model.Clients.Remove(supressedclient);
                    BindingContext = null;
                    allClient.ItemsSource = model.Clients;
                    BindingContext = model;
                }
                else
                    await Shell.Current.DisplayAlert("erreur", "Erreur de connexion avec le serveur", "Ok");
            }
        }

        public async void OnUpdateAsync(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            Client updatedclient = button.CommandParameter as Client;
            await Shell.Current.GoToAsync($"{nameof(UpdateClient)}?{nameof(UpdateClient.ClientId)}={updatedclient.Id}");
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
    }
}