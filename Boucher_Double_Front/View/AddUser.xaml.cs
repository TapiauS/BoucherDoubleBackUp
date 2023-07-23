using Boucher_DoubleModel.Models;
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
using System.Net.Http.Json;
using Boucher_Double_Front.ViewModel;

namespace Boucher_Double_Front.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddUser : ContentPage
    {

        private AddUserModel model=new();

        public AddUser()
        {
            InitializeComponent();
            RolePicker.ItemsSource=Enum.GetValues(typeof(Role)).Cast<Role>().ToList();
            Task.Run(async () => await model.GetAllStoreAsync()).Wait();
            StorePicker.ItemsSource = model.Stores;
            StorePicker.ItemDisplayBinding = new Binding("Name");
            BindingContext = model;
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


        public async void ValidateUserAsync(object sender,EventArgs args)
        {
            string password = PasswordChecker.Text;
            if (password==model.NewUser.Password)
            {
                if (model.NewUser.Password != "" && model.NewUser.Mail != "" && model.NewUser.Surname != "" && model.NewUser.Name != "" && model.NewUser.Login != "" && model.NewUser.PhoneNumber != "")
                {
                    App app = Application.Current as App;
                    HttpClient client = await app.PrepareQuery();
                    if (model.NewUser.Store == null)
                        model.NewUser.Store =app.user.Store;
                    string json=JsonConvert.SerializeObject(model.NewUser);
                    StringContent content = new (json, Encoding.UTF8, "application/json");
                    HttpResponseMessage responseMessage=await client.PostAsync("User", content);
                    if(responseMessage.IsSuccessStatusCode)
                    {
                        string jsoncontent=await responseMessage.Content.ReadAsStringAsync();
                        if(bool.Parse(jsoncontent))
                        {
                            await Shell.Current.DisplayAlert("Succés", "Compte créé avec succés", "Ok");
                        }
                        else
                        {
                            await Shell.Current.DisplayAlert("Erreur", "Mot de passe ou pseudo indisponible", "Ok");
                        }
                    }
                }

                
            }
            else
            {
                await Shell.Current.DisplayAlert("Mot de passe invalide", "Les deux mot de passe ne sont pas identique", "Ok");
            }
        }

        public async void ValidateStoreAsync(object sender,EventArgs args)
        {

            if (StorePicker.SelectedIndex > 0)
            {
                model.NewUser.Store = StorePicker.SelectedItem as Store;
                model.NewStore =model.NewUser.Store;
                BindingContext = null;
                BindingContext = model;
            }

            else
            {
                App appInstance = Application.Current as App;
                HttpClient client = await appInstance.PrepareQuery();
                string json = JsonConvert.SerializeObject(model.NewStore);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("Store", content);
                string jsonresult=await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode&&bool.Parse(jsonresult)) 
                {
                    model.NewUser.Store = model.NewStore;
                }
            }

        }

    }
}