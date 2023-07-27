
using MailKit.Security;
using Newtonsoft.Json;
using System.Text;

namespace Boucher_Double_Front.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MailAccessParameter : ContentPage
    {
        public Boucher_DoubleModel.Models.Entitys.MailParameter MailParameter { get; set; }=new() ;
        public MailAccessParameter()
        {
            Resources = StyleDictionnary.GetInstance();
            InitializeComponent();
            App app=Application.Current as App;
            if(app.MailAccessParameter != null )
                MailParameter=app.MailAccessParameter;
            ConnexionPicker.ItemsSource=Enum.GetValues(typeof(SecureSocketOptions));
            MailParameter.IdStore = app.User.Store.IdStore;
            MailAdress.Text = "Adresse mail: "+app.User.Store.Mail;
            BindingContext = this;
        }

        public async void OnValidateAsync(object sender, EventArgs e)
        {
            try
            {
                App app = Application.Current as App;
                if (ConnexionPicker.SelectedIndex > 0)
                    MailParameter.ConnexionType = (SecureSocketOptions)ConnexionPicker.SelectedItem;
                if (app.MailAccessParameter == null)
                {
                    HttpClient client = await app.PrepareQuery();
                    string json = JsonConvert.SerializeObject(MailParameter);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("MailParameter", content);
                    var jsonString = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode && int.Parse(jsonString) > 0)
                    {
                        app.MailAccessParameter = MailParameter;
                        await Shell.Current.GoToAsync("..");
                    }
                }
                else
                {
                    HttpClient client = await app.PrepareQuery();
                    string json = JsonConvert.SerializeObject(MailParameter);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync("MailParameter", content);
                    var jsonString = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode && bool.Parse(jsonString))
                    {
                        app.MailAccessParameter = MailParameter;
                        await Shell.Current.GoToAsync("..");
                    }
                }
                
            }

            catch { }
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