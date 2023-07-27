
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui;
using Microsoft.Maui.Controls.Xaml;
using static System.Net.Mime.MediaTypeNames;

namespace Boucher_Double_Front.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RestaurantInfo : ContentPage
    {
        ImageSource Image { get; set; }
        public string PicturePath { get; set; }
        FileResult FileResult { get; set; }
        public RestaurantInfo()
        {
            Resources = StyleDictionnary.GetInstance();
            InitializeComponent();
            BindingContext = this;
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

        async void OnPickPhotoButtonClicked(object sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.PickAsync(PickOptions.Images);
                if (result != null)
                {
                    PicturePath = result.FullPath;
                    BindingContext = null;
                    BindingContext = this;
                    if (result.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
                        result.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
                    {
                        var stream = await result.OpenReadAsync();
                        Image = ImageSource.FromStream(() => stream);
                    }
                }
                FileResult = result;
                if (result != null)
                {
                    App app= Microsoft.Maui.Controls.Application.Current as App;
                    HttpClient httpClient =await app.PrepareQuery();
                    var stream = await result.OpenReadAsync();
                    HttpContent httpContent = new StreamContent(stream);

                    var formData = new MultipartFormDataContent
                    {
                        { httpContent, "file",result.FileName }
                    };
                    var response = await httpClient.PostAsync($"Store/upload/{app.User.Store.IdStore}", formData);
                    string resultString= await response.Content.ReadAsStringAsync();    
                    if (response.IsSuccessStatusCode)
                    {
                        await Shell.Current.DisplayAlert("Succés", "Image sauvegardée avec succés", "Ok");
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Erreur", "Erreur lors de la sauvegarde de l'image", "Ok");
                    }
                }
            }
            catch (Exception ex)
            {
                // The user canceled or something went wrong
            }
        }
    }
}