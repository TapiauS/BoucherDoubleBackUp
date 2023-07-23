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
    public partial class BillManager : ContentPage
    {
        private BillManagerModel model=new();
        public BillManager()
        {
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
                model.BillOption =new BillParameter();
                ValidationButton.Text = "Valider";
                BindingContext = null;
                BindingContext = model;
            }
        }

        public async void OnValidateAsync(Object sender, EventArgs e)
        {
            App appInstance = Application.Current as App;
            HttpClient client =await appInstance.PrepareQuery();
            string json;
            if (BillPicker.SelectedIndex < 1)
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
            } 
            else
            {
                appInstance.BillParameter = BillPicker.SelectedItem as BillParameter;
                appInstance.DatabaseHelper.SaveBillParameter(appInstance.BillParameter);
                await Shell.Current.GoToAsync("..");
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