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
    public partial class ClientCommand : ContentPage
    {
        private ClientCommandModel model=new();


        public ClientCommand()
        {
            InitializeComponent();

        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            BindingContext = null;
            Shell.Current.Navigation.RemovePage(this);
        }

        public void OnDelete(object sender,EventArgs args)
        {
            Button button = (Button)sender;
            SelloutLine line = button.CommandParameter as SelloutLine;
            model.Lines.Remove(line);
        }
        public async void PickerSelectedIndexChanged(object sender,EventArgs args)
        {
            App app = Application.Current as App;
            if (EventPicker.SelectedIndex >= 1)
            {
                app.ActivCommand.Event = EventPicker.SelectedItem as Event;
            }
            else
            {
                app.ActivCommand.Event = null;
            }
        }
        protected override async void OnAppearing()
        {
            App app = Application.Current as App;
            if (app.ActivCommand != null)
            {
                Task.Run(async () => await model.GetAllEventAsync()).Wait();
                BindingContext = model;
                allSoldProduct.ItemsSource = model.Lines;
                base.OnAppearing();
            }
            else
            {
                await Shell.Current.DisplayAlert("Erreur de commande", "Aucune commande active", "Ok");
                await Shell.Current.GoToAsync(nameof(Home));
            }    
        }


        public void AddProduct(object sender,EventArgs args)
        {
            Button button = (Button)sender;
            SelloutLine line = button.CommandParameter as SelloutLine;
            Product product = line.SoldProduct;
            App appInstance = Application.Current as App;
            appInstance.ActivCommand.AddProduct(product);
            RefreshSoldProducts();
        }

        public void RemoveProduct(object sender, EventArgs args)
        {
            Button button = (Button)sender;
            SelloutLine line = button.CommandParameter as SelloutLine;
            Product product = line.SoldProduct;
            App appInstance = Application.Current as App;
            appInstance.ActivCommand.RemoveProduct(product);
            RefreshSoldProducts();
        }

        public async void NewProductAsync(object sender, EventArgs args)
        {
            await Shell.Current.GoToAsync($"{nameof(AllCategoryList)}");    
        }

        public async void OnValidateAsync(object sender, EventArgs args)
        {
            App app = Application.Current as App;
            if(model.Lines.Count == 0||app.BillParameter==null)
            {
                if(model.Lines.Count == 0)
                    await Shell.Current.DisplayAlert("Erreur", "Aucun produit selectionné", "OK");
                if(app.BillParameter == null)
                    await Shell.Current.DisplayAlert("Erreur", "Aucun paramétrage de facture sélectionné", "OK");
                return;
            }
            else
            {
                HttpClient client =await app.PrepareQuery();
                string json = JsonConvert.SerializeObject(app.ActivCommand);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response;
                if (app.ActivCommand.Id == 0)
                {
                    response = await client.PostAsync("Sellout", content);
                    var jsonString = await response.Content.ReadAsStringAsync();
                    if(response.IsSuccessStatusCode&& int.Parse(jsonString)>0)
                    {
                        app.ActivCommand.Id = int.Parse(jsonString);
                        await Shell.Current.GoToAsync($"{nameof(OneSellout)}");
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Erreur", "Erreur Inconnue", "OK");
                        return;
                    }
                }
                else
                {
                    response = await client.PutAsync("Sellout", content);
                    var jsonString = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode &&bool.Parse(jsonString)) 
                    {
                        await Shell.Current.GoToAsync($"{nameof(OneSellout)}");
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Erreur", "Erreur Inconnue", "OK");
                        return;
                    }
                }
            }
        }

        private void RefreshSoldProducts()
        {
            BindingContext=null;
            allSoldProduct.ItemsSource = null;
            BindingContext = model;
            allSoldProduct.ItemsSource = model.Lines;
        }

    }
}