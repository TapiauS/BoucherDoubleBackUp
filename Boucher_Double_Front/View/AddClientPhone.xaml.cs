using Boucher_Double_Front.ViewModel;
using Boucher_DoubleModel.Models.Entitys;
using System.Text.RegularExpressions;

namespace Boucher_Double_Front.View;

public partial class AddClientPhone : ContentPage
{
    private AddSelloutClientModel model = new();
    public AddClientPhone()
    {
        BindingContext = model;
        Task.Run(async() =>model.GetAllClientAsync()).Wait();
        Resources = StyleDictionnary.GetInstance();
        InitializeComponent();
    }

    public void OnPhoneNumberChanged(object sender, EventArgs e)
    {
        Entry entry = sender as Entry;
        string phoneNumberRegex = "^\\d{10}$";
        if(entry != null )
        {
            if (Regex.IsMatch(entry.Text, phoneNumberRegex))
                entry.BackgroundColor = Color.FromRgba(255, 255, 0, 0.5);
            else
                entry.BackgroundColor = Color.FromRgba(255, 0, 0, 0.5);
        }

    }
    public async void OnValidateAsync(object sender, EventArgs e)
    {
        App application = Application.Current as App;
        List<Client> compatibleClients = model.Clients.Where(client => client.PhoneNumber == model.Client.PhoneNumber).ToList();
        string phoneNumberRegex = "^\\d{10}$";
        application.ActivCommand = null;
        if (Regex.IsMatch(model.Client.PhoneNumber, phoneNumberRegex))
        {
            if (compatibleClients.Count > 0)
            {
                application.ActivCommand = new() { Client = compatibleClients[0] };
            }
            else
                application.ActivCommand = new() { Client = model.Client };
            await Shell.Current.GoToAsync($"{nameof(AddSelloutClient)}");
        }
        else
            await Shell.Current.DisplayAlert("Erreur d'entrée", "Numéro de télephone invalide", "Ok");
    }
}