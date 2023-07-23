using System.Text.RegularExpressions;

namespace Boucher_DoubleModel.Models.Entitys
{
    /// <summary>
    /// Represent an User of the app
    /// </summary>
    public class User : Client
    {
        private string login;
        private string password;
        public int IdUser { get; set; }
        public string Login {
            get
            {
                return login;
            } 
            set
            {
                string loginRegex = @"^[A-Za-z0-9]{0,}$";
                if (Regex.IsMatch(value.Trim(), loginRegex))
                {
                    login = value;
                }
                else
                    throw new BoucherDoubleModelException("Login not valide");
            } }
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                string passwordRegex= @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{0,}$";
                if (Regex.IsMatch(value, passwordRegex))
                {
                    password = value;
                }
                else
                    //throw new BoucherDoubleModelException("Password not valide");
                    password = value;
            }
        }
        public Store Store { get; set; }
        public Role Role { get; set; } = Role.NORMAL;
    }
}
