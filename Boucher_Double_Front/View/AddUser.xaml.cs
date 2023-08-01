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
using System.Text.RegularExpressions;

namespace Boucher_Double_Front.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddUser : ContentPage
    {

        private AddUserModel model=new();

        public AddUser()
        {
            Resources = StyleDictionnary.GetInstance();
            InitializeComponent();
            App app = Application.Current as App;
            RolePicker.ItemsSource=Enum.GetValues(typeof(Role)).Cast<Role>().Where(role=>role<=app.User.Role).ToList();
            Task.Run(async () => await model.GetAllStoreAsync()).Wait();
            StorePicker.ItemsSource = model.Stores;
            StorePicker.ItemDisplayBinding = new Binding("Name");
            BindingContext = model;
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

        public void OnPasswordChanged(object sender, EventArgs e)
        {
            Entry entry=sender as Entry;
            string passwordRegex = "^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$";
            if(entry != null)
            {
                if(Regex.IsMatch(entry.Text, passwordRegex))
                    entry.BackgroundColor = Color.FromRgba(255, 255, 0, 0.5);
                else
                    entry.BackgroundColor = Color.FromRgba(255, 0, 0, 0.5);
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


        public async void ValidateUserAsync(object sender,EventArgs args)
        {
            string password = PasswordChecker.Text;
            string phoneNumberRegex = "^\\d{10}$";
            string passwordRegex = "^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$";
            string mailRegex = "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$";
            if (password==model.NewUser.Password)
            {
                if (Regex.IsMatch(model.NewUser.Password, passwordRegex) && Regex.IsMatch(model.NewUser.Mail, mailRegex) && model.NewUser.Surname != "" && model.NewUser.Name != "" && model.NewUser.Login != "" && Regex.IsMatch(model.NewUser.PhoneNumber, phoneNumberRegex))
                {
                    App app = Application.Current as App;
                    HttpClient client = await app.PrepareQuery();
                    if (model.NewUser.Store == null)
                        model.NewUser.Store = app.user.Store;
                    string json = JsonConvert.SerializeObject(model.NewUser);
                    StringContent content = new(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage responseMessage = await client.PostAsync("User", content);
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        string jsoncontent = await responseMessage.Content.ReadAsStringAsync();
                        if (int.Parse(jsoncontent) > 0)
                        {
                            await Shell.Current.DisplayAlert("Succés", "Compte créé avec succés", "Ok");
                        }
                        else
                        {
                            await Shell.Current.DisplayAlert("Erreur", "Mot de passe ou pseudo indisponible", "Ok");
                        }
                    }
                    else
                        await Shell.Current.DisplayAlert("Erreur", "Erreur d'accés au serveur", "Ok");
                }
                else
                    await Shell.Current.DisplayAlert("Erreur de saisie", "Certains champs sont incorrects, ils sont tous obligatoire", "Ok");
            }
            else
            {
                await Shell.Current.DisplayAlert("Mot de passe invalide", "Les deux mot de passe ne sont pas identique", "Ok");
            }
        }

        public async void ValidateStoreAsync(object sender,EventArgs args)
        {
            string phoneNumberRegex = "^\\d{10}$";
            string mailRegex = "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$";
            if (StorePicker.SelectedIndex > 0)
            {
                model.NewUser.Store = StorePicker.SelectedItem as Store;
                model.NewStore =model.NewUser.Store;
                BindingContext = null;
                BindingContext = model;
            }

            else
            {
                if(Regex.IsMatch(model.NewStore.Mail, mailRegex)&&  model.NewStore.Name != "" && Regex.IsMatch(model.NewStore.PhoneNumber, phoneNumberRegex) && model.NewStore.Adress != "" && model.NewStore.Town != "")
                {
                    App appInstance = Application.Current as App;
                    HttpClient client = await appInstance.PrepareQuery();
                    string json = JsonConvert.SerializeObject(model.NewStore);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("Store", content);
                    string jsonresult = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode && int.Parse(jsonresult) > 0)
                    {
                        model.NewUser.Store = model.NewStore;
                    }
                    else
                        await Shell.Current.DisplayAlert("Erreur", "Erreur d'accés au serveur", "Ok");
                }
                else
                    await Shell.Current.DisplayAlert("Erreur de saisie", "Certains champs sont incorrects, ils sont tous obligatoire", "Ok");
            }

        }

    }
}