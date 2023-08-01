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
    public partial class MailParameter : ContentPage
    {
        private MailParameterModel model=new();
        public MailParameter()
        {
            Resources = StyleDictionnary.GetInstance();
            InitializeComponent();
            Task.Run(async () => await model.LoadMailContentParameterAsync()).Wait();
            MailContentPicker.ItemsSource = model.MailParameters;
            MailContentPicker.ItemDisplayBinding = new Binding("Name");
        }

        public async void OnValidateAsync(object sender, EventArgs e)
        {
            App appInstance = Application.Current as App;
            HttpClient client =await appInstance.PrepareQuery();
            string json;
            if (MailContentPicker.SelectedIndex < 1)
            {
                json = JsonConvert.SerializeObject(model.CurrentMailParameter);
                StringContent content = new (json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("MailContentParameter", content);
                string contentString = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode && int.Parse(contentString) > 0)
                {
                    model.CurrentMailParameter.Id = int.Parse(contentString);
                    appInstance.MailParameter = model.CurrentMailParameter;
                    appInstance.DatabaseHelper.SaveMailContentParameter(model.CurrentMailParameter);
                    await Shell.Current.DisplayAlert("Succés", "Paramétrage créé et selectionné avec succés", "Ok");
                    await Shell.Current.GoToAsync($"..");
                }
                else
                    await Shell.Current.DisplayAlert("Erreur", "Erreur d'accés au serveur", "Ok");
            }
            else
            {
                json = JsonConvert.SerializeObject(model.CurrentMailParameter);
                StringContent content = new(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync("MailContentParameter", content);
                string contentString = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode && bool.Parse(contentString))
                {
                    appInstance.MailParameter = MailContentPicker.SelectedItem as MailContentParameter;
                    appInstance.DatabaseHelper.SaveMailContentParameter(appInstance.MailParameter);
                    await Shell.Current.DisplayAlert("Succés", "Paramétrage mis à jour et selectionné avec succés", "Ok");
                    await Shell.Current.GoToAsync("..");
                }
                else
                    await Shell.Current.DisplayAlert("Erreur", "Erreur d'accés au serveur", "Ok");
            }
        }

        public void PickerSelectedIndexChanged(object sender, EventArgs e)
        {
            if (MailContentPicker.SelectedIndex >= 1)
            {
                model.CurrentMailParameter = MailContentPicker.SelectedItem as MailContentParameter;
                ValidationButton.Text = "Mettre a jour";
                DeleteButton.IsVisible = true;
                BindingContext = null;
                BindingContext = model;
            }
            else
            {
                model.CurrentMailParameter = new() { IdStore = (Application.Current as App).User.Store.IdStore };
                ValidationButton.Text = "Valider";
                DeleteButton.IsVisible = false;
                BindingContext = null;
                BindingContext = model;
            }
        }

        public async void OnDeleteAsync(object sender,EventArgs args)
        {
            App appInstance = Application.Current as App;
            HttpClient client = await appInstance.PrepareQuery();
            HttpResponseMessage response=await client.DeleteAsync($"MailContentParameter/{model.CurrentMailParameter.Id}");
            string json=await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode && bool.Parse(json))
            {
                await Shell.Current.DisplayAlert("Succés", "Paramétrage supprimer avec succés", "Ok");
                await Shell.Current.GoToAsync($"{nameof(Home)}");
            }
            else
                await Shell.Current.DisplayAlert("Erreur", "Erreur d'accés au serveur", "Ok");
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
    }
}