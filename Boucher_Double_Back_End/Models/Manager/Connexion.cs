
using MySqlConnector;

namespace Boucher_Double_Back_End.Models.Manager
{
    /// <summary>
    /// Singleton that allow to access to the database
    /// </summary>
    public class Connexion 
    {
        /// <summary>
        /// Used to retrieve the database access
        /// </summary>
        /// <returns>The app configuration info</returns>
        public static IConfigurationRoot GetConfiguration()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            return builder.Build();
        }

        public static MySqlConnection getConnexion()
        {
            IConfigurationRoot config = GetConfiguration();
            MySqlConnection SQLConnexion = new MySqlConnection(config.GetConnectionString("MyConnection"));
            SQLConnexion.Open();
            return SQLConnexion;
        }
    }
}
