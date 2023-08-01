
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Boucher_DoubleModel.Models.Entitys;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Xaml;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;
using Application = Microsoft.Maui.Controls.Application;

namespace Boucher_Double_Front.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RestaurantInfo : ContentPage
    {
        ImageSource Image { get; set; }
        public string PicturePath { get; set; }
        FileResult FileResult { get; set; }

        public Store Store 
        {
            get
            {
                App app=Application.Current as App; 
                return app.User.Store;
            }
        }
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

        public void MailTextChanged(object sender, EventArgs e)
        {
            Entry entry = sender as Entry;
            string mailRegex = "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$";
            if (entry != null)
            {
                if (Regex.IsMatch(entry.Text, mailRegex))
                    entry.BackgroundColor = Color.FromRgba(255, 255, 0, 0.5);
                else
                    entry.BackgroundColor = Color.FromRgba(255, 0, 0, 0.5);
            }
        }

        public void OnPhoneNumberChanged(object sender, EventArgs e)
        {
            Entry entry = sender as Entry;
            string phoneNumberRegex = "^\\d{10}$";
            if (entry != null)
            {
                if (Regex.IsMatch(entry.Text, phoneNumberRegex))
                    entry.BackgroundColor = Color.FromRgba(255, 255, 0, 0.5);
                else
                    entry.BackgroundColor = Color.FromRgba(255, 0, 0, 0.5);
            }

        }


        public async void OnValidateAsync(object sender,EventArgs e)
        {
            App app = Microsoft.Maui.Controls.Application.Current as App;
            string phoneNumberRegex = "^\\d{10}$";
            string mailRegex = "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$";
            if (Regex.IsMatch(Store.PhoneNumber, phoneNumberRegex) && Regex.IsMatch(Store.Mail, mailRegex))
            {
                HttpClient httpClient = await app.PrepareQuery();
                string json = JsonConvert.SerializeObject(Store);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage httpResponse = await httpClient.PutAsync("Store", content);
                if (httpResponse.IsSuccessStatusCode)
                {
                    string result = await httpResponse.Content.ReadAsStringAsync();
                    if (bool.Parse(result))
                    {
                        await Shell.Current.DisplayAlert("Succés", "Modification sauvegardée avec succés", "Ok");
                        if (FileResult != null)
                        {
                            var stream = await FileResult.OpenReadAsync();
                            HttpContent httpContent = new StreamContent(stream);

                            var formData = new MultipartFormDataContent
                                {
                                    { httpContent, "file",FileResult.FileName }
                                };
                            var response = await httpClient.PostAsync($"Store/upload/{app.User.Store.IdStore}", formData);
                            string resultString = await response.Content.ReadAsStringAsync();
                            if (response.IsSuccessStatusCode)
                            {
                                AppShell appShell = Shell.Current as AppShell;
                                appShell.StoreImage = FileResult.FullPath;
                                await Shell.Current.DisplayAlert("Succés", "Image sauvegardée avec succés", "Ok");
                            }
                            else
                            {
                                await Shell.Current.DisplayAlert("Erreur", "Erreur lors de la sauvegarde de l'image", "Ok");
                            }
                        }
                        await Shell.Current.GoToAsync($"..");
                    }
                    else
                        await Shell.Current.DisplayAlert("Erreur", "Erreur de sauvegarde du nom", "Ok");
                }
                else
                    await Shell.Current.DisplayAlert("Erreur", "Erreur d'accés au serveur", "Ok");
            }
            else
                await Shell.Current.DisplayAlert("Erreur", "Mail ou numéro de télephone invalide", "Ok");
        }
        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            Shell.Current.Navigation.RemovePage(this);
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
            }
            catch (Exception ex)
            {
                // The user canceled or something went wrong
            }
        }
    }
}