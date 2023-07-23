using System.Text.RegularExpressions;

namespace Boucher_DoubleModel.Models.Entitys
{
    /// <summary>
    /// Represent a given store, extend the <see cref="Person"/> class wich contain the name of the store
    /// </summary>
    public class Store : Person
    {
        private string town="";
        public string Town { get 
            {
                return town;
            }
            set 
            {
                town = value.Trim();
            } }
        public string Adress { get; set; } = "";
        public int IdStore { get; set; }
        public string ?LogoPath { get; set; } = "";
    }
}
