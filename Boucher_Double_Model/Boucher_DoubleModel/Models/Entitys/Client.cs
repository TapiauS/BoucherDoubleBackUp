using System.Text.RegularExpressions;

namespace Boucher_DoubleModel.Models.Entitys
{
    /// <summary>
    /// Represent a client of a given store chain, extends the <see cref="Person"/> class which hold the other personal infos
    /// </summary>
    public class Client : Person
    {

        private string surname = "";

        /// <summary>
        /// Not nullable
        /// </summary>
        public string Surname
        {
            get { return surname; }
            set
            {
                surname = value.Trim();
            }
        }
    }
}
