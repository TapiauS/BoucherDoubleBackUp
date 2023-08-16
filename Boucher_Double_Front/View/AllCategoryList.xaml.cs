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
                try
                {
                    allCategory.ItemsSource = Task.Run(async () => await model.GetSubCategoryAsync(int.Parse(value))).Result;
                }
                catch (Exception ex)
                {
                    Shell.Current.DisplayAlert("Erreur", "Erreur Inconnue", "OK");
                }

                BindingContext = null;
                BindingContext = model;
            } }


        public AllCategoryList()
        {
            Resources = StyleDictionnary.GetInstance();
            InitializeComponent();
            BindingContext = model;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if(IdContainer == "0")
            {
                try
                {
                    allCategory.ItemsSource = await model.GetAllCategoryAsync();
                }
                catch(Exception ex)
                {
                    Shell.Current.DisplayAlert("Erreur", "Erreur Inconnue", "OK");
                }
            }

        }


        private async void AddToCommand(object sender, EventArgs args)
        {
            Button button = sender as Button;
            Product product = (Product)button.CommandParameter;
            App appInstance = Application.Current as App;
            appInstance.ActivCommand.AddProduct(product);
            await Shell.Current.GoToAsync($"{nameof(ClientCommand)}");
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
