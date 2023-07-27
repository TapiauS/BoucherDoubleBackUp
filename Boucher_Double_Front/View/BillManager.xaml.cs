﻿using Boucher_DoubleModel.Models.Entitys;
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
    public partial class BillManager : ContentPage
    {
        private BillManagerModel model=new();
        public BillManager()
        {
            Resources = StyleDictionnary.GetInstance();
            InitializeComponent();
            Task.Run(async () => await model.GetAllOptionsAsync()).Wait();
            BillPicker.ItemsSource = model.ExistingsOption;
            BillPicker.ItemDisplayBinding = new Binding("Name");
            BindingContext = model;
        }                                                                           



        public void PickerSelectedIndexChanged(object sender, EventArgs e)
        {
            if(BillPicker.SelectedIndex >= 1)
            {
                model.BillOption=BillPicker.SelectedItem as BillParameter;
                ValidationButton.Text = "Mettre à jour";
                BindingContext = null;
                BindingContext = model;
            }
            else
            {
                model.BillOption =new() { IdStore = (Application.Current as App).User.Store.IdStore };
                ValidationButton.Text = "Valider";
                BindingContext = null;
                BindingContext = model;
            }
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            BindingContext = null;
            Shell.Current.Navigation.RemovePage(this);
        }

        public async void OnValidateAsync(Object sender, EventArgs e)
        {
            App appInstance = Application.Current as App;
            HttpClient client =await appInstance.PrepareQuery();
            string json;
            if (model.BillOption.Id==0)
            {
                json = JsonConvert.SerializeObject(model.BillOption);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("BillParameter", content);
                string contentString = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode && int.Parse(contentString) > 0)
                {
                    appInstance.BillParameter = model.BillOption;
                    appInstance.DatabaseHelper.SaveBillParameter(model.BillOption);
                    await Shell.Current.GoToAsync("..");
                }
                else
                    await Shell.Current.DisplayAlert("Erreur", "Erreur d'accés au serveur", "Ok");
            } 
            else
            {
                json = JsonConvert.SerializeObject(model.BillOption);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync("BillParameter", content);
                string contentString = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode && bool.Parse(contentString))
                {
                    appInstance.BillParameter = BillPicker.SelectedItem as BillParameter;
                    appInstance.DatabaseHelper.SaveBillParameter(appInstance.BillParameter);
                    await Shell.Current.GoToAsync("..");
                }
                else
                    await Shell.Current.DisplayAlert("Erreur", "Erreur d'accés au serveur", "Ok");
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