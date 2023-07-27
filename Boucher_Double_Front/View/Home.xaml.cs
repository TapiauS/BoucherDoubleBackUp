using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui;
using Microsoft.Maui.Controls.Xaml;

namespace Boucher_Double_Front.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Home : ContentPage
    {

        public Home()
        {
            Resources = StyleDictionnary.GetInstance();
            InitializeComponent();
            BindingContext = Application.Current as App;
            welcomeLabel.SetBinding(Label.TextProperty, "User.Store.Name", stringFormat: "Bienvenue {0}");
        }
    }
}