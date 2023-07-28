
using System.Reflection;

namespace Boucher_Double_Front.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Parameter : ContentPage
    {
        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            App app = Application.Current as App;
            app.DatabaseHelper.SaveTheme(app.Theme);
            BindingContext = null;
            Shell.Current.Navigation.RemovePage(this);
        }

        private Dictionary<string, object> colorMap = new();
        public bool ClientSafe 
        { 
            get
            {
                App app=Application.Current as App;
                return app.ClientProtectionActivated;
            }
            set
            {
                App app = Application.Current as App;
                app.ClientProtectionActivated=value;
            }
        }
        public Parameter()
        {
            Resources = StyleDictionnary.GetInstance();
            InitializeComponent();
            const BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            foreach (var field in typeof(Colors).GetFields(flags))
            {
                colorMap[field.Name] = field.GetValue(null);
            }
            BackgroundColorPicker.ItemsSource= colorMap.Keys.ToList();
            ShellColorPicker.ItemsSource = colorMap.Keys.ToList();
            ButtonColorPicker.ItemsSource = colorMap.Keys.ToList();
            TextColorPicker.ItemsSource = colorMap.Keys.ToList();
            BindingContext = this;
        }

        private bool FirstAppear = true;
        protected override async void OnAppearing()
        {
            AppShell appShell = Shell.Current as AppShell;
            if(FirstAppear)
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
        public void OnBackgroundChanged(object sender, EventArgs e)
        {
            App app = Application.Current as App;
            Color color = colorMap[(string)BackgroundColorPicker.SelectedItem] as Color;

            app.Theme.BackgroundColorHexCode=color.ToArgbHex();
            Resources["BackgroundColor"] = color;
        }

        public void OnTextColorChanged(object sender, EventArgs e)
        {
            App app = Application.Current as App;
            Color color = colorMap[(string)TextColorPicker.SelectedItem] as Color;
            (Resources as StyleDictionnary).TextColor = color;
            Resources = null;
            Resources = StyleDictionnary.GetInstance();
            app.Theme.TextColorHexCode = color.ToArgbHex();
        }
        public void OnButtonColorChanged(object sender, EventArgs e)
        {
            App app = Application.Current as App;
            Color color = colorMap[(string)ButtonColorPicker.SelectedItem] as Color;
            Resources["ButtonColor"] = color;
            app.Theme.ButtonColorHexCode = color.ToArgbHex();
        }
        public void OnShellColorChanged(object sender, EventArgs e)
        {
            App app = Application.Current as App;
            Color color = colorMap[(string)ShellColorPicker.SelectedItem] as Color;
            Resources["ShellColor"] = color;
            app.Theme.ShellColorHexCode = color.ToArgbHex();
        }
        public async void OnDisconnectAsync(object sender,EventArgs args)
        {
            bool choice=await Shell.Current.DisplayAlert("Confirmation", "Voulez vous vraiment vous deconnecter?", "Oui", "Non");
            App app=Application.Current as App;
            if (choice)
            {
                HttpClient client =await app.PrepareQuery();
                HttpResponseMessage response = await client.GetAsync("User/Disconnect");
                if (response.IsSuccessStatusCode)
                {
                    app.DatabaseHelper.DeleteKnowUser();
                    app.User = null;
                    await Shell.Current.GoToAsync($"{nameof(LogView)}");
                }
                else
                    await Shell.Current.DisplayAlert("Erreur", "Erreur de connexion avec le serveur", "Ok");
            }
        } 
    }
}