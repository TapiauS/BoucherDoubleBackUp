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
    [QueryProperty(nameof(IdContainer), nameof(IdContainer))]
    public partial class AllCategoryList : ContentPage
    {
        private AllCategoryModel model=new();
        private string idContainer = "0";
        public string IdContainer { 
            get=>idContainer; 
            set
            {
                idContainer = value;
                allCategory.ItemsSource=Task.Run(async ()=>await model.GetSubCategoryAsync(int.Parse(value))).Result;
                menuList.ItemsSource = Task.Run(async () => await model.GetAllMenuAsync()).Result;
            } }


        public AllCategoryList()
        {
            InitializeComponent();
            BindingContext = model;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if(IdContainer == "0")
                allCategory.ItemsSource = await model.GetAllCategoryAsync();
        }


        public async void OnUpdateMenuClickedAsync(object sender, EventArgs e)
        {
            Button button = sender as Button;
            Menu menu = button.CommandParameter as Menu;
            await Shell.Current.GoToAsync($"{nameof(MenuManager)}?{nameof(MenuManager.IdMenu)}={menu.Id}");
        }

        public async void NewMenu(object sender,EventArgs e)
        {
            if (IdContainer != "0")
            {
                App app = Application.Current as App;
                app.ActivMenu = new Menu() { Content = new List<SelloutLine>(), Price = new Price(0.01m), Name = "", PicturePath = "" };
                await Shell.Current.GoToAsync($"{nameof(MenuManager)}?{nameof(MenuManager.IdCategory)}={IdContainer}");
            }
            else
                await Shell.Current.DisplayAlert("Erreur", "Aucune catégorie de produit sélectionner", "Ok");
        } 

        async void GetProductAsync(object sender, EventArgs args)
        {
            Button button= sender as Button;
            Category category= (Category)button.CommandParameter;
            if (category.SubCategory.Count == 0)
                await Shell.Current.GoToAsync($"{nameof(OneCategoryList)}?{nameof(OneCategoryList.CategoryId)}={category.Id}");
            else
                await Shell.Current.GoToAsync($"{nameof(AllCategoryList)}?{nameof(IdContainer)}={category.Id}&{nameof(OneCategoryList.CategoryId)}={category.Id}");
        }

        async void UpdateCategoryAsync(object sender, EventArgs args)
        {
            Button button = sender as Button;
            Category category = (Category)button.CommandParameter;
            await Shell.Current.GoToAsync($"{nameof(CategoryManager)}?{nameof(CategoryManager.CategoryId)}={category.Id}");
        }

        async void NewCategory(object sender,EventArgs args)
        {
            await Shell.Current.GoToAsync(nameof(CategoryManager)); 
        }

        async void AddSubCategoryAsync(object sender, EventArgs args)
        {
            Button button = sender as Button;
            Category category = (Category)button.CommandParameter;
            await Shell.Current.GoToAsync($"{nameof(CategoryManager)}?{nameof(CategoryManager.IdContainer)}={category.Id}");
        }

        async Task GetSubCategoryAsync(int id)
        {
            App appInstance = Application.Current as App;
            HttpClient client = await appInstance.PrepareQuery();
            HttpResponseMessage response = await client.GetAsync($"Category/SubCategory/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                allCategory.ItemsSource=JsonConvert.DeserializeObject<List<Category>>(jsonString);
            }                     
            else
            {
                await Shell.Current.DisplayAlert("Erreur", "Erreur Inconnue", "OK");
                return;
            }
        }
    } 
}
