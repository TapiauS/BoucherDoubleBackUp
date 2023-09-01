using System.Text.RegularExpressions;

namespace Boucher_DoubleModel.Models.Entitys
{
    /// <summary>
    /// Category of product, access is limited by store requiring it to have a field representing that
    /// </summary>
    public class Category
    {
        private string name;
        public int Id { get; set; }
        public int IdContainer { get; set; }
        public string Name { get { return name; }
            set
            {
                string nameRegex = @"^[A-Za-z0-9]+$";
                if (Regex.IsMatch(value.Trim(), nameRegex))
                {
                    name = value.Trim();
                }
                else
                    throw new BoucherDoubleModelException("Invalid category name");
            }
        }
        public Event? Event { get; set; }
        public Store Store { get; set; }

        public string? PicturePath { get; set; }
        public List<Category> SubCategory { get; set; } = new();
    }
}
