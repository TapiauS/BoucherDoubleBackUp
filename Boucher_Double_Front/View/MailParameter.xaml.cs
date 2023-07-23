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
        private MailParameterModel model;
        public MailParameter()
        {

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
                    await Shell.Current.GoToAsync("..");
                }
            }
            else
            {
                appInstance.MailParameter = MailContentPicker.SelectedItem as MailContentParameter;
                appInstance.DatabaseHelper.SaveMailContentParameter(appInstance.MailParameter);
                await Shell.Current.GoToAsync("..");
            }
        }

        public void PickerSelectedIndexChanged(object sender, EventArgs e)
        {
            if (MailContentPicker.SelectedIndex >= 1)
            {
                model.CurrentMailParameter = MailContentPicker.SelectedItem as MailContentParameter;
                ValidationButton.Text = "Mettre a jour";
                BindingContext = null;
                BindingContext = model;
            }
            else
            {
                model.CurrentMailParameter = new MailContentParameter();
                ValidationButton.Text = "Valider";
                BindingContext = null;
                BindingContext = model;
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