using Boucher_Double_Front.ViewModel;
using Boucher_DoubleModel.Models.Entitys;

namespace Boucher_Double_Front.View;

public partial class AddClientPhone : ContentPage
{
    private AddSelloutClientModel model = new();
    public AddClientPhone()
    {
        BindingContext = model;
        Task.Run(async() =>model.GetAllClientAsync()).Wait();
        InitializeComponent();
    }

    public async void OnValidateAsync(object sender, EventArgs e)
    {
        App application = Application.Current as App;
        List<Client> compatibleClients = model.Clients.Where(client => client.PhoneNumber == model.Client.PhoneNumber).ToList();
        application.ActivCommand = null;
        if (compatibleClients.Count > 0)
        {
            application.ActivCommand = new() { Client = compatibleClients[0] };
        }
        await Shell.Current.GoToAsync($"{nameof(AddSelloutClient)}");
    }
}