using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Boucher_DoubleModel.Models.Entitys;
using Newtonsoft.Json;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Xaml;
using Boucher_Double_Front.ViewModel;

namespace Boucher_Double_Front.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [QueryProperty(nameof(IdCategory), nameof(IdCategory))]
    [QueryProperty(nameof(IdMenu), nameof(IdMenu))]
    public partial class MenuManager : ContentPage
    {
        private MenuManagerModel model=new();
        private string idCategory = "0";
        public string IdCategory 
        { 
            get=>idCategory;
            set
            {
                idCategory = value;
                if (model.Menu != null&&model.Menu.Category==null&&IdMenu=="0")
                {
                    try
                    {
                        Task.Run(async () => await model.GetOneCategoryAsync(int.Parse(idCategory))).Wait();
                    }
                    catch(Exception) 
                    {
                        Shell.Current.DisplayAlert("Erreur", "Erreur Inconnue", "OK");
                    }
                }

            }
        }
        private string idMenu="0";
        public string IdMenu 
        { 
            get=>idMenu;
            set
            {
                if(model.Menu ==null)
                {
                    idMenu = value;
                    if (idMenu != "0")
                    {
                        DeleteButton.Text = "Supprimer";
                    }
                    try
                    {
                        Task.Run(async () => await model.LoadOneMenuAsync(int.Parse(idMenu))).Wait();
                        if (IdCategory != "0")
                        {
                            Task.Run(async () => await model.GetOneCategoryAsync(int.Parse(idCategory))).Wait();
                        }
                    }
                    catch (Exception) 
                    {
                        Shell.Current.DisplayAlert("Erreur", "Erreur d'accés au serveur", "Ok");
                    }

                }
            }
        }


        

        public MenuManager()
        {
            Resources = StyleDictionnary.GetInstance();
            InitializeComponent();
            BindingContext = model;
            allSoldProduct.ItemsSource = model.Lines;
            DeleteButton.Text = "Annuler";
        }



        public void OnDelete(object sender, EventArgs args)
        {
            Button button = (Button)sender;
            SelloutLine line = button.CommandParameter as SelloutLine;
            model.Lines.Remove(line);
        }

        public async void OnAbortAsync(object sender,EventArgs args)
        {
            App app = Application.Current as App;
            if (model.Menu.Id != 0)
            {
                bool choice = await Shell.Current.DisplayAlert("Confirmation", "Attention supprimer ce produit supprimera toutes les commandes associées", "Oui", "Non");
                if (choice)
                {
                    HttpClient client = await app.PrepareQuery();
                    HttpResponseMessage response = await client.DeleteAsync($"Menu/{model.Menu.Id}");
                    string result = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode && bool.Parse(result))
                    {
                        await Shell.Current.GoToAsync("..");
                    }
                    else
                        await Shell.Current.DisplayAlert("Erreur", "Erreur d'accés au serveur", "Ok");
                }
            }
            else
            {
                app.ActivMenu = null;
                await Shell.Current.GoToAsync("..");
            }
        } 

        public void AddProduct(object sender, EventArgs args)
        {
            Button button = (Button)sender;
            SelloutLine line = button.CommandParameter as SelloutLine;
            Product product = line.SoldProduct;
            App appInstance = Application.Current as App;
            appInstance.ActivMenu.AddProduct(product);
            RefreshSoldProducts();
        }

        public void RemoveProduct(object sender, EventArgs args)
        {
            Button button = (Button)sender;
            SelloutLine line = button.CommandParameter as SelloutLine;
            Product product = line.SoldProduct;
            App appInstance = Application.Current as App;
            appInstance.ActivMenu.RemoveProduct(product);
            RefreshSoldProducts();
        }

        public async void NewProductAsync(object sender, EventArgs args)
        {
            await Shell.Current.GoToAsync($"{nameof(AllCategoryList)}");
        }

        public async void OnValidateAsync(object sender, EventArgs args)
        {
            App app = Application.Current as App;
            if (model.Lines.Count == 0)
            {
                await Shell.Current.DisplayAlert("Erreur", "Aucun produit selectionné", "OK");
            }
            else
            {
                HttpClient client =await app.PrepareQuery();
                string json = JsonConvert.SerializeObject(app.ActivMenu);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response;
                if (model.Menu.Id == 0)
                {
                    response = await client.PostAsync("Menu", content);
                    var jsonString = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode && int.Parse(jsonString) > 0)
                    {
                        model.Menu.Id = int.Parse(jsonString);
                        if (await model.SendPictureAsync(client))
                        {
                            app.ActivMenu = null;
                            await Shell.Current.GoToAsync($"..");
                        }
                        else
                        {
                            await Shell.Current.DisplayAlert("Erreur", "Erreur lors de l'envoie de l'image d'illustration", "Ok");
                            await Shell.Current.GoToAsync("..");
                            return;
                        }
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Erreur", "Erreur Inconnue", "OK");
                        return;
                    }
                }
                else
                {
                    response = await client.PutAsync("Menu", content);
                    var jsonString = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode && bool.Parse(jsonString))
                    {
                        if (await model.SendPictureAsync(client))
                        {
                            app.ActivMenu = null;
                            await Shell.Current.GoToAsync($"..");
                        }
                        else
                        {
                            await Shell.Current.DisplayAlert("Erreur", "Erreur lors de l'envoie de l'image d'illustration", "Ok");
                            await Shell.Current.GoToAsync("..");
                            return;
                        }
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Erreur", "Erreur Inconnue", "OK");
                        return;
                    }
                }
            }
        }
        async void OnPickPhotoButtonClicked(object sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.PickAsync(PickOptions.Images);
                if (result != null)
                {
                    model.ImageSource = result.FullPath;
                    if (result.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
                        result.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
                    {
                        var stream = await result.OpenReadAsync();
                        model.FileResult = result;
                        BindingContext = null;
                        BindingContext = model;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erreur", "Erreur de selection de l'image", "Ok");
            }
        }
        private void RefreshSoldProducts()
        {
            allSoldProduct.ItemsSource = null;
            allSoldProduct.ItemsSource = model.Lines;
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