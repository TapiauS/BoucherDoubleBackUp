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

        private bool firstAppear = true;

        public ClientCommand()
        {
            InitializeComponent();
            App app = Application.Current as App;
            if (app.ActivCommand != null)
            {
                try
                {
                    Task.Run(async () => await model.GetAllEventAsync()).Wait();
                    allCategory.ItemsSource=Task.Run(async() =>await model.GetAllCategoryAsync()).Result;
                }
                catch(Exception ex)
                {
                    Shell.Current.DisplayAlert("Erreur", "Erreur d'accés au serveur", "OK");
                }
                BindingContext = model;
                EventPicker.ItemsSource = model.CompatibleEvents;
                EventPicker.ItemDisplayBinding=new Binding("Name");
                allSoldProduct.ItemsSource = model.Lines;
                base.OnAppearing();
            }
        }


        public async void OnBackClicked(object sender, EventArgs e)
        {
            if (model.ActiveCategory.Category.SubCategory.Count>0) 
            { 
                model.ParentCategory = model.ParentCategory.Where(category=>category.Category.Id!=model.ActiveCategory.Category.Id).ToList();
                model.ActiveCategory = model.ParentCategory.Count>0?model.ParentCategory.Last():null;
                if (model.ParentCategory.Count <= 0)
                {
                    model.ActiveCategory = null;
                    mainCategory.IsVisible = true;
                    oneSubCategory.IsVisible = false;
                    BackButton.IsVisible = false;
                }
            }
            else
            {
                model.ParentCategory = model.ParentCategory.Where(category => category.Category.Id != model.ActiveCategory.Category.Id).ToList();
                model.ActiveCategory = model.ParentCategory.Count > 0 ? model.ParentCategory.Last() : null;
                productList.IsVisible = false;
                if (model.ParentCategory.Count <= 0)
                {
                    model.ActiveCategory = null;
                    mainCategory.IsVisible = true;
                    oneSubCategory.IsVisible = false;
                    BackButton.IsVisible = false;
                }
                else
                {
                    subCategory.ItemsSource = await model.GetSubCategoryAsync(model.ActiveCategory.Category.Id);
                    oneSubCategory.IsVisible = true;
                }
            }
        }

        private async void AddToCommand(object sender, EventArgs args)
        {
            Button button = sender as Button;
            Product product = (Product)button.CommandParameter;

            App appInstance = Application.Current as App;
            appInstance.ActivCommand?.AddProduct(product);
            allSoldProduct.ItemsSource = null;
            RefreshSoldProducts();
        }


        async void GetProductAsync(object sender, EventArgs args)
        {
            Button button = sender as Button;
            BackButton.IsVisible = true;
            Category category = (Category)button.CommandParameter;
            if (category.SubCategory.Count == 0)
            {
                model.ActiveCategory = new() { Category = category, ImageSource = $"{(Application.Current as App).BaseUrl}Category/image/{category.PicturePath}" };
                model.ParentCategory.Add(new() { Category = category, ImageSource = $"{(Application.Current as App).BaseUrl}Category/image/{category.PicturePath}" });
                subCategory.ItemsSource = null;
                allProduct.ItemsSource = await model.GetAllProductAsync(model.ActiveCategory.Category.Id);
                productList.IsVisible = true;
                mainCategory.IsVisible = false;
                oneSubCategory.IsVisible = false;
            }
            else
            {
                model.ActiveCategory = new() { Category = category, ImageSource = $"{(Application.Current as App).BaseUrl}Category/image/{category.PicturePath}" };
                model.ParentCategory.Add(new() { Category = category, ImageSource = $"{(Application.Current as App).BaseUrl}Category/image/{category.PicturePath}" });
                allProduct.ItemsSource=null;
                subCategory.ItemsSource = await model.GetSubCategoryAsync(model.ActiveCategory.Category.Id);
                productList.IsVisible = false;
                mainCategory.IsVisible = false;
                oneSubCategory.IsVisible = true;
            }
        }


        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (width > 600)
                phoneChecker.Orientation=StackOrientation.Horizontal;
            else
                phoneChecker.Orientation=StackOrientation.Vertical;
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
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
            if(firstAppear)
            {
                App app = Application.Current as App;
                firstAppear = false;
                if (app.ActivCommand != null)
                {
                    base.OnAppearing();
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erreur de commande", "Aucune commande active", "Ok");
                    await Shell.Current.GoToAsync(nameof(Home));
                }

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