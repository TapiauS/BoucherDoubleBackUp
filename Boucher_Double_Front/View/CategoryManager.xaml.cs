using Boucher_DoubleModel.Models.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Xaml;
using Newtonsoft.Json;
using Boucher_Double_Front.ViewModel;
using MailKit;
using FFImageLoading;

namespace Boucher_Double_Front.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [QueryProperty(nameof(CategoryId), nameof(CategoryId))]
    [QueryProperty(nameof(IdContainer), nameof(IdContainer))]
    public partial class CategoryManager : ContentPage
    {
        private CategoryManagerModel model=new();
        public string IdContainer { get; set; } = "0";

        public string categoryId = "0";
        public string CategoryId 
        { 
            get=>categoryId;
            set
            { 
                categoryId = value;
                if(categoryId != "0")
                {
                    Task.Run(async () => await model.GetOneCategoryAsync(int.Parse(categoryId))).Wait();
                    BindingContext = null;
                    BindingContext = model;
                }
            } 
        } 
        public CategoryManager()
        {
            Resources = StyleDictionnary.GetInstance();
            InitializeComponent();
            BindingContext = model;
        }


        public async void OnValidateAsync(object sender,EventArgs args)
        {
            App appInstance = Application.Current as App;
            string namePattern = @"^(?=.*[a-zA-Z])[a-zA-Z\s]+$";
            if (int.Parse(CategoryId) == 0)
            {
                HttpClient client =await appInstance.PrepareQuery();
                model.Category.Store=appInstance.User.Store;
                model.Category.IdContainer = int.Parse(IdContainer);
                string json = JsonConvert.SerializeObject(model.Category);
                if(!string.IsNullOrEmpty(model.Category.Name) || !Regex.IsMatch(model.Category.Name, namePattern))
                {
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("Category", content);
                    var jsonString = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode && int.Parse(jsonString)>0)
                    {
                        model.Category.Id = int.Parse(jsonString);
                        if (await model.SendPictureAsync(client))
                            await Shell.Current.GoToAsync("..");
                        else
                        {
                            await Shell.Current.DisplayAlert("Erreur", "Erreur lors de l'envoie de l'image d'illustration", "Ok");
                            await Shell.Current.GoToAsync("..");
                            return;
                        }
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erreur", "Nom invalide", "OK");
                    return;
                }
            }
            else
            {
                HttpClient client =await appInstance.PrepareQuery();
                string json = JsonConvert.SerializeObject(model.Category);
                if (!string.IsNullOrEmpty(model.Category.Name) || !Regex.IsMatch(model.Category.Name, namePattern))
                {
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync("Category", content);
                    var jsonString = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode && bool.Parse(jsonString))
                    {
                        if (await model.SendPictureAsync(client))
                            await Shell.Current.GoToAsync("..");
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
                    await Shell.Current.DisplayAlert("Erreur", "Nom invalide", "OK");
                    return;
                }
            }
        }



        public async void OnSuppressionAsync(object sender,EventArgs args)
        {
            App appInstance = Application.Current as App;
            bool choice = await Shell.Current.DisplayAlert("Confirmation", "Attention supprimer cette catégorie supprimera tout les produits et toutes leurs commandes associés", "Oui", "Non");
            if(choice)
            {
                if (int.Parse(CategoryId) != 0)
                {
                    HttpClient client = await appInstance.PrepareQuery();
                    HttpResponseMessage response = await client.DeleteAsync($"Category/{CategoryId}");
                    var jsonString = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode && bool.Parse(jsonString))
                    {
                        await Shell.Current.GoToAsync("..");
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Erreur", "Erreur Inconnue", "OK");
                        return;
                    }
                }
            }

        }

        public async void OnPickPhotoButtonClicked(object sender, EventArgs e)
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
                // The user canceled or something went wrong
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