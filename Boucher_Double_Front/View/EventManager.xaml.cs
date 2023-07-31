using Boucher_Double_Front.ViewModel;
using Boucher_DoubleModel.Models.Entitys;
using Microsoft.Maui.Storage;
using Newtonsoft.Json;
using System.Text;

namespace Boucher_Double_Front.View;

public partial class EventManager : ContentPage
{
	private EventManagerModel model=new();
	public EventManager()
	{
        Resources = StyleDictionnary.GetInstance();
        InitializeComponent();
        Task.Run(async () => await model.GetAllEvent()).Wait();
        EventPicker.ItemsSource = model.ExistingsOption;
        EventPicker.ItemDisplayBinding = new Binding("Name");
        BindingContext = model;
	}


    public async void OnDeleteAsync(object sender,EventArgs args)
    {
        App app=Application.Current as App;
        HttpClient client = await app.PrepareQuery();
        HttpResponseMessage response = await client.DeleteAsync($"Event/{model.ActivEvent.Id}") ;
        string json=await response.Content.ReadAsStringAsync() ;
        if (response.IsSuccessStatusCode && bool.Parse(json))
            await Shell.Current.GoToAsync($"{nameof(Home)}");
        else
            await Shell.Current.DisplayAlert("Erreur", "Erreur d'accés au serveur", "Ok");
    } 
	public void PickerSelectedIndexChanged(object sender, EventArgs e)
	{
        if (EventPicker.SelectedIndex >= 1)
        {
            model.ActivEvent = EventPicker.SelectedItem as Event;
            ValidationButton.Text = "Mettre à jour";
            DeleteButton.IsVisible = true;
            BindingContext = null;
            BindingContext = model;
        }
        else
        {
            model.ActivEvent = new() { Store = (Application.Current as App).User.Store };
            ValidationButton.Text = "Valider";
            DeleteButton.IsVisible = false;
            BindingContext = null;
            BindingContext = model;
        }
    }

    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
    }

    public async void OnValidateAsync(object sender, EventArgs e)
    {
        App appInstance = Application.Current as App;
        HttpClient client = await appInstance.PrepareQuery();
        string json;
        if (model.ActivEvent.Id == 0)
        {
            json = JsonConvert.SerializeObject(model.ActivEvent);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("Event", content);
            string contentString = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode && int.Parse(contentString) > 0)
            {
                await Shell.Current.GoToAsync($"{nameof(Home)}");
            }
            else
                await Shell.Current.DisplayAlert("Erreur", "Erreur d'accés au serveur", "Ok");
        }
        else
        {
            json = JsonConvert.SerializeObject(model.ActivEvent);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync("Event", content);
            string contentString = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode && bool.Parse(contentString))
            {
                await Shell.Current.GoToAsync($"{nameof(Home)}");
            }
            else
                await Shell.Current.DisplayAlert("Erreur", "Erreur d'accés au serveur", "Ok");
        }
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

}