using Boucher_Double_Front.Model;
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
    [QueryProperty(nameof(CategoryId), nameof(CategoryId))]
    public partial class OneCategoryList : ContentPage
    {
        private OneCategoryListModel model=new();
        private string categoryId;
        public string CategoryId { get=>categoryId; 
            set
            { 
                categoryId = value;
                allProduct.ItemsSource = Task.Run(async ()=>await model.GetAllProductAsync(int.Parse(categoryId))).Result;
                menuList.ItemsSource=Task.Run(async()=>await model.GetAllMenuAsync(int.Parse(categoryId))).Result;
            } }
        public OneCategoryList()
        {
            Resources = StyleDictionnary.GetInstance();
            InitializeComponent();
            BindingContext = model;
        }

        

        private async void OnUpdateClickedAsync(object sender,EventArgs args)
        {
            Button button=sender as Button;
            Product product = (Product)button.CommandParameter;

            await Shell.Current.GoToAsync($"{nameof(ProductManager)}?{nameof(ProductManager.IdCategory)}={product.Category.Id}&{nameof(ProductManager.IdProduct)}={product.Id}");
        }

        private async void AddToCommand(object sender, EventArgs args)
        {
            Button button = sender as Button;
            Product product = (Product)button.CommandParameter;
            App appInstance = Application.Current as App;
            appInstance.ActivCommand.AddProduct(product);
            await Shell.Current.GoToAsync($"{nameof(ClientCommand)}");
        }

        private async void AddToMenu(object sender,EventArgs args)
        {
            Button button = sender as Button;
            Product product = (Product)button.CommandParameter;
            App appInstance = Application.Current as App;
            appInstance.ActivMenu.AddProduct(product);
            await Shell.Current.GoToAsync($"{nameof(MenuManager)}");
        }

        private async void NewProductClickedAsync(object sender,EventArgs args)
        {
            await Shell.Current.GoToAsync($"{nameof(ProductManager)}?{nameof(ProductManager.IdCategory)}={CategoryId}");
        }



        public async void OnUpdateMenuClickedAsync(object sender, EventArgs e)
        {
            Button button = sender as Button;
            Menu menu = button.CommandParameter as Menu;
            App app = Application.Current as App;
            app.ActivMenu = menu;
            await Shell.Current.GoToAsync($"{nameof(MenuManager)}?{nameof(MenuManager.IdMenu)}={menu.Id}");
        }

        public async void NewMenu(object sender, EventArgs e)
        {
            if (CategoryId != "0")
            {
                App app = Application.Current as App;
                app.ActivMenu = new () { Content = new List<SelloutLine>(), Price = new Price(0.01m), Name = "", PicturePath = "" };
                await Shell.Current.GoToAsync($"{nameof(MenuManager)}?{nameof(MenuManager.IdCategory)}={CategoryId}");
            }
            else
                await Shell.Current.DisplayAlert("Erreur", "Aucune catégorie de produit sélectionner", "Ok");
        }

        protected override async void OnAppearing()
        {
            AppShell appShell = Shell.Current as AppShell;
            if (await appShell.AskLogin())
            {
                base.OnAppearing();
            }
            else
                await appShell.GoToAsync(nameof(Home));
        }

    }
}