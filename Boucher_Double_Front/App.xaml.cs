using Boucher_Double_Front.Database;
using Boucher_Double_Front.Model;
using Boucher_Double_Front.View;
using Boucher_DoubleModel.Models.Entitys;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;



namespace Boucher_Double_Front
{
    public partial class App : Application
    {
        public User user=null;
        public string BaseUrl { get; }="https://boucheedouble.fr/api/";
        public  string CsrfToken { get; set; } = "";

        public Theme Theme { get; set; }
        public bool ClientProtectionActivated { get; set; } = false;
        public Menu ActivMenu { get; set; }
        public Sellout ActivCommand { get; set; }
        public BillParameter BillParameter { get; set; }
        public DatabaseHelper DatabaseHelper { get; set; }
        public MailContentParameter MailParameter { get; set; }
        public Boucher_DoubleModel.Models.Entitys.MailParameter MailAccessParameter { get; set; }
        public  User User 
        { get=>user; 
        set {
                if (value != null)
                {
                    user = value;
                    MainPage = new AppShell(user);
                }
            } }
        public App()
        {

            InitializeComponent();

        }


        protected override Window CreateWindow(IActivationState activationState)
        {
            string databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "database.db");
            DatabaseHelper = new DatabaseHelper(databasePath);
            Theme = DatabaseHelper.GetTheme() ?? new();
            Resources = StyleDictionnary.GetInstance();
            if (MainPage == null)
            {
                MainPage = new Home(); 
            }
            return base.CreateWindow(activationState);
        }

        public async Task<HttpClient> PrepareQuery()
        {
            HttpClient client = new HttpClient() { BaseAddress=new Uri(BaseUrl)};
            client.DefaultRequestHeaders.Add("X-CSRF-TOKEN", CsrfToken);
            HttpResponseMessage connectedResponse = await client.GetAsync("User/Connected");
            string content=await connectedResponse.Content.ReadAsStringAsync();
            if(connectedResponse.IsSuccessStatusCode && bool.Parse(content))
            {
                return client;
            }
            else
            {
                UserAccess userAccess = new UserAccess() { Password = User.Password, Pseudo = User.Login };
                if (await Connect(userAccess))
                {
                    return client;
                }
                else
                    throw new HttpRequestException("Erreur d'accés au serveur");
            }
        }
        protected async override void OnSleep()
        {
            // This function will be triggered when the application is about to sleep or shutdown.
            // You can perform any necessary cleanup or save any unsaved data here.

            // Call your custom shutdown function here
            //await DisconnecteurOnShutDownAsync();
        }

        public async Task<bool> Connect(UserAccess userAccess)
        {
            if (userAccess != null)
            {
                HttpClient client = new();

                string fullUrl = $"{BaseUrl}User/Connect";
                User user1=new User() { Login=userAccess.Pseudo,Password=userAccess.Password,Store=new Store()};
                string json = JsonConvert.SerializeObject(user1);
                StringContent content = new(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(fullUrl,content);
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var responseObject = JObject.Parse(jsonString);
                    if ((bool)responseObject["success"])
                    {
                        CsrfToken = (string)responseObject["csrf"];
                        User user = responseObject["user"].ToObject<User>();
                        User = user;
                        return true;

                    }
                    else
                    {
                        await MainPage.DisplayAlert("Erreur", "Erreur inconnue", "OK");
                        return false;
                    }
                }
                else return false;
            }
            else
                return false;
        }
       
        protected async Task DisconnecteurOnShutDownAsync()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-CSRF-TOKEN", CsrfToken);
            string fullUrl = $"{BaseUrl}User/Disconnect";
            HttpResponseMessage response = await client.GetAsync(fullUrl);
        }

        protected async override void OnStart()
        {

            UserAccess LogInfo = DatabaseHelper.GetKnowUser();
            if (await Connect(LogInfo))
            {
                HttpClient clientUp = await PrepareQuery();
                HttpResponseMessage response1 = await clientUp.GetAsync("MailParameter");
                if (response1.IsSuccessStatusCode)
                {
                    string jsonString = await response1.Content.ReadAsStringAsync();
                    List<Boucher_DoubleModel.Models.Entitys.MailParameter> mailParameters = JsonConvert.DeserializeObject<List<Boucher_DoubleModel.Models.Entitys.MailParameter>>(jsonString);
                    if (mailParameters.Count > 0)
                        MailAccessParameter = mailParameters[0];
                    BillParameter billParameter = DatabaseHelper.GetKnowBillParameter();
                    MailContentParameter MailContentParameter = DatabaseHelper.GetKnowMailContentParameter();
                    if (billParameter != null)
                        BillParameter = billParameter.IdStore == User.Store.IdStore ? DatabaseHelper.GetKnowBillParameter() : null;
                    if (MailContentParameter != null)
                        MailParameter = MailContentParameter.IdStore == User.Store.IdStore ? DatabaseHelper.GetKnowMailContentParameter() : null;
                }
            }
            if (User == null)
                MainPage = new LogView();
            else
                MainPage = new AppShell(User);
        }


        protected override void OnResume()
        {
        }

    }
}
