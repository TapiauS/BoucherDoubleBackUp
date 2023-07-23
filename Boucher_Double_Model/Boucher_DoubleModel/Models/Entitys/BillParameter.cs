
using System.Text.RegularExpressions;

namespace Boucher_DoubleModel.Models.Entitys
{        
    /// <summary>
    /// Class that reprensent one of the bill parameter of a single shop
    /// </summary>
    public class BillParameter
    {

        private string name;
        public int Id { get; set; }
        public string? Name { 
            get 
            { 
                return name; 
            } 
            set 
            { 
               name = value.Trim();
            } }
        public string? Foot { get; set; }
        public string? SpecialMention { get; set; }
        public string? Mention { get; set; }
        public int IdStore { get; set; }
    }
}
