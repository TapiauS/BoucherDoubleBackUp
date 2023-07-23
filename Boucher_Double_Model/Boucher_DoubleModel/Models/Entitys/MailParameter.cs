using MailKit.Security;
using SQLite;
using System.Text.RegularExpressions;

namespace Boucher_DoubleModel.Models.Entitys
{
    /// <summary>
    /// Represent the parameter of the mail box of a given store
    /// </summary>
    public class MailParameter
    {
        public int Id { get; set; }
        public int IdStore { get; set; }
        public string Password { get; set; }
        public string? Login { get; set; }
        public SecureSocketOptions? ConnexionType { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }


        public static SecureSocketOptions? GetOption(string optionName)
        {
            foreach(SecureSocketOptions option in Enum.GetValues(typeof(SecureSocketOptions)))
            {
                if(option.ToString() == optionName)
                    return option;
            }
            return null;
        }
    }
}
