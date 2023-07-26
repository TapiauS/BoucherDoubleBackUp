using Boucher_Double_Front.View;
using Boucher_DoubleModel.Models.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui;
using Microsoft.Maui.Controls.Xaml;
using MailParameter = Boucher_Double_Front.View.MailParameter;

namespace Boucher_Double_Front
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppShell : Shell
    {
        public  User User { get; set; } = new User();
        public  Store Store { get; set; } = new Store();
        private Color backGroundColor;
        public Color BackGroundColor 
        { 
            get=>backGroundColor;
            set 
            {
                backGroundColor = value;
                this.Resources["BackgroundColor"] = value;
            }
        }

        private Color shellColor;
        public Color ShellColor
        {
            get=> shellColor;
            set
            {
                shellColor = value;
                Resources["ShellColor"] = value;
            }
        }
        public string StoreImage
        {
            get
            {
                App app=Application.Current as App;
                return app.BaseUrl + "Store/image/" + app.User.Store.LogoPath;
            }
        }


        public bool IsAdmin { get=> User.Role >= Boucher_DoubleModel.Models.Role.ADMIN; }
        public AppShell(User user)
        {
            BackgroundColor = Colors.Blue;
            ShellColor = Colors.Red;
            InitializeComponent();
            Routing.RegisterRoute(nameof(LogView), typeof(LogView));
            Routing.RegisterRoute(nameof(Home), typeof(Home));
            Routing.RegisterRoute(nameof(AddSelloutClient),typeof(AddSelloutClient));
            Routing.RegisterRoute(nameof(AddUser), typeof(AddUser));
            Routing.RegisterRoute(nameof(AllCategoryList), typeof(AllCategoryList));
            Routing.RegisterRoute(nameof(AllSellout), typeof(AllSellout));
            Routing.RegisterRoute(nameof(BillManager), typeof(BillManager));
            Routing.RegisterRoute(nameof(CategoryManager), typeof(CategoryManager));
            Routing.RegisterRoute(nameof(ClientCommand), typeof(ClientCommand));
            Routing.RegisterRoute(nameof(DataBaseManager), typeof(DataBaseManager));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(MailAccessParameter), typeof(MailAccessParameter));
            Routing.RegisterRoute(nameof(MailParameter), typeof(MailParameter));
            Routing.RegisterRoute(nameof(OneCategoryList), typeof(OneCategoryList));
            Routing.RegisterRoute(nameof(OneSellout), typeof(OneSellout));
            Routing.RegisterRoute(nameof(Parameter), typeof(Parameter));
            Routing.RegisterRoute(nameof(ProductManager), typeof(ProductManager));
            Routing.RegisterRoute(nameof(RestaurantInfo), typeof(RestaurantInfo));
            Routing.RegisterRoute(nameof(UpdateClient),typeof(UpdateClient));
            Routing.RegisterRoute(nameof(MenuManager), typeof(MenuManager));
            Routing.RegisterRoute(nameof(AddClientPhone), typeof(AddClientPhone));
            Routing.RegisterRoute(nameof(EventManager), typeof(EventManager));
            Routing.RegisterRoute(nameof(RecapTableView), typeof(RecapTableView));

            User = user;
            BindingContext = this;
        }




        public async Task<bool> AskLogin()
        {
            App app=Application.Current as App;
            if (app.ClientProtectionActivated)
            {
                string pseudo = await DisplayPromptAsync("Nom d'utilisateur", "Entrez votre pseudo", "Valider", "Annuler");
                if (pseudo == app.User.Login)
                {
                    return app.User.Password == await DisplayPromptAsync("Nom d'utilisateur", "Entrez votre mot de passe", "Valider", "Annuler");
                }
                else
                    return false;
            }
            else
                return true;
        } 
    }
}