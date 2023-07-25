using Boucher_DoubleModel.Models.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui;
using Microsoft.Maui.Controls.Xaml;

namespace Boucher_Double_Front.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Parameter : ContentPage
    {
        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            BindingContext = null;
            Shell.Current.Navigation.RemovePage(this);
        }
        public bool ClientSafe 
        { 
            get
            {
                App app=Application.Current as App;
                return app.ClientProtectionActivated;
            }
            set
            {
                App app = Application.Current as App;
                app.ClientProtectionActivated=value;
            }
        }
        public Parameter()
        {
            InitializeComponent();
            BindingContext = this;
        }

        private bool FirstAppear = true;
        protected override async void OnAppearing()
        {
            AppShell appShell = Shell.Current as AppShell;
            if(FirstAppear)
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

        public async void OnDisconnectAsync(object sender,EventArgs args)
        {
            bool choice=await Shell.Current.DisplayAlert("Confirmation", "Voulez vous vraiment vous deconnecter?", "Oui", "Non");
            App app=Application.Current as App;
            if (choice)
            {
                HttpClient client =await app.PrepareQuery();
                HttpResponseMessage response = await client.GetAsync("User/Disconnect");
                if (response.IsSuccessStatusCode)
                {
                    app.DatabaseHelper.DeleteKnowUser();
                    app.User = null;
                    await Shell.Current.GoToAsync($"{nameof(LogView)}");
                }
                else
                    await Shell.Current.DisplayAlert("Erreur", "Erreur de connexion avec le serveur", "Ok");
            }
        } 
    }
}