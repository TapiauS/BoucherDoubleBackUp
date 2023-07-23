using System.Text.RegularExpressions;

namespace Boucher_DoubleModel.Models.Entitys
{
    /// <summary>
    /// Represent a product selled by a store, the store selling it is determind by the category field
    /// </summary>
    public class Product
    {
        private string name;
        public int Id { get; set; }
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value.Trim();
            }
        }


        public Price Price
        {
            get;
            set;
        }
        public Category Category { get; set; }


        public string? PicturePath { get; set; }
    }
}
