using Boucher_Double_Back_End.Models.Manager;
using MySqlConnector;

namespace Boucher_Double_Back_End.Models.ServerModel
{
    public class RepeatingServices : BackgroundService
    {
        private readonly PeriodicTimer _timer=new(TimeSpan.FromDays(1));
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (await _timer.WaitForNextTickAsync(stoppingToken) && stoppingToken.IsCancellationRequested) 
            { 
                CheckRGPDAsync();
            }
        }

        private static async void CheckRGPDAsync() 
        {
            using MySqlConnection connection = Connexion.getConnexion();
            using MySqlCommand command = connection.CreateCommand();
            command.CommandText = "DELETE FROM person WHERE TIMESTAMPDIFF(MONTH, last_used, GETDATE()) > 18";
            command.ExecuteNonQuery();
        }
    }
}
