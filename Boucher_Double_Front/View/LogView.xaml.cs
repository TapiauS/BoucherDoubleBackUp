using Boucher_DoubleModel.Models.Entitys;
using System.Net.Http;
using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Xaml;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Generic;
using Boucher_Double_Front.Model;

namespace Boucher_Double_Front.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LogView : ContentPage
    {
        public User User { get; set; }
        public LogView()
        {
            App appInstance = Application.Current as App;
            if (appInstance.User == null)
            {
                User = new User() { Login = "Placeholder", Password = "Placeholder" };
                InitializeComponent();
                BindingContext = this;
            }
            else
            {
                User = appInstance.User;
                InitializeComponent();
                BindingContext = this;
            }
        }

        public async void ConnectAsync(object sender, EventArgs args)
        {
            App appInstance = Application.Current as App;
            if (appInstance.User == null)
            {
                UserAccess userAccess = new UserAccess() { Password=User.Password,Pseudo=User.Login};
                if(await appInstance.Connect(userAccess))
                {
                    HttpClient clientUp = await appInstance.PrepareQuery();
                    HttpResponseMessage response1 = await clientUp.GetAsync("MailParameter");
                    if (response1.IsSuccessStatusCode)
                    {
                        string jsonString = await response1.Content.ReadAsStringAsync();
                        List<Boucher_DoubleModel.Models.Entitys.MailParameter> mailParameters = JsonConvert.DeserializeObject<List<Boucher_DoubleModel.Models.Entitys.MailParameter>>(jsonString);
                        if (mailParameters.Count > 0)
                            appInstance.MailAccessParameter = mailParameters[0];
                        appInstance.DatabaseHelper.SaveUserInfo(new Model.UserAccess() { Pseudo = User.Login, Password = User.Password });
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Erreur", "Erreur inconnue", "OK");
                    }
                }
            }
            else
            {
                AppShell shell = Shell.Current as AppShell;
                if (await shell.AskLogin())
                {
                    HttpClient client = await appInstance.PrepareQuery();
                    string json = JsonConvert.SerializeObject(User);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync($"User", content);
                    var jsonString = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode && bool.Parse(jsonString))
                    {
                        appInstance.User.Login = User.Login;
                        appInstance.User.Password = User.Password;
                    }
                }
                else
                    shell.DisplayAlert("Erreur", "Erreur de mot de passe ou de login", "Ok");
            }
        }
    }
}