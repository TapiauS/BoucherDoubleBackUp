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
		InitializeComponent();
		Task.Run(async ()=>await model.GetAllEvent()).Wait();
		BindingContext = model;
	}

	public void PickerSelectedIndexChanged(object sender, EventArgs e)
	{
        if (EventPicker.SelectedIndex >= 1)
        {
            model.Event = EventPicker.SelectedItem as Event;
            ValidationButton.Text = "Mettre à jour";
            BindingContext = null;
            BindingContext = model;
        }
        else
        {
            model.Event = new() { Store = (Application.Current as App).User.Store };
            ValidationButton.Text = "Valider";
            BindingContext = null;
            BindingContext = model;
        }
    }

    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        BindingContext = null;
        Shell.Current.Navigation.RemovePage(this);
    }

    public async void OnValidateAsync(object sender, EventArgs e)
    {
        App appInstance = Application.Current as App;
        HttpClient client = await appInstance.PrepareQuery();
        string json;
        if (model.Event.Id == 0)
        {
            json = JsonConvert.SerializeObject(model.Event);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("Event", content);
            string contentString = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode && int.Parse(contentString) > 0)
            {
                await Shell.Current.GoToAsync("..");
            }
            else
                await Shell.Current.DisplayAlert("Erreur", "Erreur d'accés au serveur", "Ok");
        }
        else
        {
            json = JsonConvert.SerializeObject(model.Event);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync("Event", content);
            string contentString = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode && bool.Parse(contentString))
            {
                await Shell.Current.GoToAsync("..");
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