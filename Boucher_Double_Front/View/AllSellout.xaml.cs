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
    public partial class AllSellout : ContentPage
    {
        private AllSelloutModel model = new();
        public AllSellout()
        {
            Resources = StyleDictionnary.GetInstance();
            InitializeComponent();
            allSellout.ItemsSource=Task.Run(async () => await model.GetAllSelloutAsync()).Result;
            BindingContext = model;
        }


        public async void OnDeleteAsync(object sender, EventArgs e)
        {
            Button button = sender as Button;
            Sellout sellout = button.CommandParameter as Sellout;
            App app = Application.Current as App;
            HttpClient client = await app.PrepareQuery();
            HttpResponseMessage response = await client.DeleteAsync($"Sellout/{sellout.Id}");
            string jsonresult = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode && bool.Parse(jsonresult))
            {
                model.AwaitingSellouts.Remove(sellout);
                allSellout.ItemsSource = null;
                allSellout.ItemsSource = model.AwaitingSellouts;
            }
            else
                await Shell.Current.DisplayAlert("Erreur", "Erreur de connexion avec le serveur", "Ok");
        }

        public async void OnLabelTappedAsync(object sender, EventArgs e)
        {
            App app = Application.Current as App;
            Label lbl = sender as Label;
            Sellout sellout = lbl.BindingContext as Sellout;
            if (app.ActivCommand != null)
            {
                bool result = await DisplayAlert("Confirmation", "Attention vous allez changer de commande en cours", "Oui", "Non");
                if (result)
                {
                    app.ActivCommand = sellout;
                    await Shell.Current.GoToAsync($"{nameof(ClientCommand)}");
                }
            }
            else
            {
                app.ActivCommand = sellout;
                await Shell.Current.GoToAsync($"{nameof(ClientCommand)}");
            }

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