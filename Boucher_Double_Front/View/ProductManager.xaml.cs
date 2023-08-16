using Boucher_DoubleModel.Models.Entitys;
using Newtonsoft.Json;
using System.Text;
using Boucher_Double_Front.ViewModel;

namespace Boucher_Double_Front.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [QueryProperty(nameof(IdProduct), nameof(IdProduct))]
    [QueryProperty(nameof(IdCategory), nameof(IdCategory))]
    public partial class ProductManager : ContentPage
    {
        private ProductManagerModel model = new();
        private string idProduct = "0";
        public string IdProduct { 
            get => idProduct; 
            set { 
                idProduct = value;
                try
                {
                    Task.Run(async () => await model.LoadProductAsync(value)).Wait();
                }
                catch(Exception ex)
                {
                    Shell.Current.DisplayAlert("Erreur", "Erreur Inconnue", "OK");
                }
                BindingContext = model;
            } }
        public string idCategory;
        public string IdCategory { get=>idCategory; set
            {
                idCategory = value;
                try
                {
                    Task.Run(async () => await model.GetOneCategoryAsync(int.Parse(idCategory))).Wait();
                }
                catch(Exception ex) 
                {
                    Shell.Current.DisplayAlert("Erreur", "Erreur Inconnue", "OK");
                }
                if(IdProduct == "0")
                    BindingContext = model;
            } }

        public ProductManager()
        {
            Resources = StyleDictionnary.GetInstance();
            InitializeComponent();
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

        public async void OnSaveAsync(object sender, EventArgs args)
        {
            App app = Application.Current as App;
            if (model.Product.Price.GetPrice() > 0 && model.Product.Name!="")
            {
                if (IdProduct == "0")
                {
                    HttpClient client =await app.PrepareQuery();
                    model.Product.Category = model.Category;
                    string json = JsonConvert.SerializeObject(model.Product);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("Product", content);
                    var jsonString = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode && int.Parse(jsonString) > 0)
                    {
                        model.Product.Id = int.Parse(jsonString);
                        if(await model.SendPictureAsync(client))
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
                    HttpClient client =await app.PrepareQuery();
                    string json = JsonConvert.SerializeObject(model.Product);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync("Product", content);
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
                        await Shell.Current.GoToAsync("..");
                        return;
                    }
                }
            }
            else
            {
                await Shell.Current.DisplayAlert("Erreur", "Nom ou prix incorrect", "OK");
                return;
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
                        BindingContext =model;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erreur", "Erreur de chargement de l'image", "OK");
            }
        }



        public async void OnDeleteAsync(object sender, EventArgs args)
        {
            App appInstance = Application.Current as App;
            bool choice = await Shell.Current.DisplayAlert("Confirmation", "Attention supprimer ce produit supprimera toutes les commandes associées", "Oui", "Non");
            if (choice)
            {
                App app = Application.Current as App;
                if (IdProduct != "0")
                {
                    HttpClient client = await app.PrepareQuery();
                    HttpResponseMessage response = await client.DeleteAsync($"Product/{IdProduct}");
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
    }
}