using Boucher_Double_Front.ViewModel;
using Boucher_DoubleModel.Models.Entitys;
using Newtonsoft.Json;

namespace Boucher_Double_Front.View;

public partial class UserManager : ContentPage
{
	private UserManagerModel model = new();
	public UserManager()
	{
		Resources = StyleDictionnary.GetInstance();
        InitializeComponent();
        GetAllUsers();
        GetAllStore();

    }

    //Used to avoid a thread error

    public async void OnDeleteUserAsync(object sender, EventArgs e)
	{
        bool choice = await Shell.Current.DisplayAlert("Confirmation", "Attention cette action est définitive", "Oui", "Non");
		if(choice) 
		{ 
			App app=Application.Current as App;
			Button button=sender as Button;
			HttpClient client =await app.PrepareQuery();
			User user=button.CommandParameter as User;
			if(user != null)
			{
				HttpResponseMessage response = await client.DeleteAsync($"User/{user.IdUser}");
				string json=await response.Content.ReadAsStringAsync();
				if (response.IsSuccessStatusCode && bool.Parse(json))
				{
					model.Users = model.Users.Where(_user => _user.IdUser != user.IdUser).ToList();
					allUser.ItemsSource = null;
					allUser.ItemsSource = model.Users;
                    if(user.IdUser==app.User.IdUser)
                    {
                        HttpClient client1 = await app.PrepareQuery();
                        HttpResponseMessage response1 = await client.GetAsync("User/Disconnect");
                        if (response1.IsSuccessStatusCode)
                        {
                            app.DatabaseHelper.DeleteKnowUser();
                            app.User = null;
                            await Shell.Current.GoToAsync($"{nameof(LogView)}");
                        }
                        else
                            await Shell.Current.DisplayAlert("Erreur", "Erreur de connexion avec le serveur", "Ok");
                    }
				}
				else
					await Shell.Current.DisplayAlert("Erreur", "Erreur d'accés au serveur", "Ok");
			}
		}
    }

    public async void GetAllUsers()
    {
        App app = Application.Current as App;
        HttpClient client = await app.PrepareQuery();
        HttpResponseMessage response = await client.GetAsync("User");
        if (response.IsSuccessStatusCode)
        {
            string json = await response.Content.ReadAsStringAsync();
            model.Users = JsonConvert.DeserializeObject<List<User>>(json);
            allUser.ItemsSource = model.Users;
        }
        else
            await Shell.Current.DisplayAlert("Erreur", "Erreur d'accés au serveur", "Ok");
    }


    public async void GetAllStore()
    {
        App app = Application.Current as App;
        HttpClient client = await app.PrepareQuery();
        HttpResponseMessage response = await client.GetAsync("Store");
        if (response.IsSuccessStatusCode)
        {
            string json = await response.Content.ReadAsStringAsync();
            model.Stores= JsonConvert.DeserializeObject<List<Store>>(json);
            allStore.ItemsSource = model.Stores;
            
        }
        else
            await Shell.Current.DisplayAlert("Erreur", "Erreur d'accés au serveur", "Ok");
    }

    public async void OnDeleteStoreAsync(object sender,EventArgs e)
	{
        bool choice = await Shell.Current.DisplayAlert("Confirmation", "Attention cette action est définitive", "Oui", "Non");
        if (choice)
        {
            App app = Application.Current as App;
            Button button = sender as Button;
            HttpClient client = await app.PrepareQuery();
            Store store = button.CommandParameter as Store;
            if (store != null)
            {
                HttpResponseMessage response = await client.DeleteAsync($"Store/{store.Id}");
                string json = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode && bool.Parse(json))
                {
                    model.Stores = model.Stores.Where(_store => _store.IdStore != store.IdStore).ToList();
                    allStore.ItemsSource = null;
                    allStore.ItemsSource = model.Users;
                }
                else
                    await Shell.Current.DisplayAlert("Erreur", "Erreur d'accés au serveur", "Ok");
            }
        }
    }
}