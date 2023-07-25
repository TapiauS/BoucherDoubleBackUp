using static System.Collections.Specialized.BitVector32;
using System.Xml.Linq;

namespace Boucher_DoubleModel.Models.Entitys
{
    /// <summary>
    /// Represent a given sellout
    /// </summary>
    public class Sellout
    {
        public int Id { get; set; }

        public Event? Event { get; set; } 
        public Store Store { get; set; }
        public DateTime ReceiptDate { get; set; }=DateTime.Now;
        public DateTime SelloutDate { get; set; }=DateTime.Now;
        public Client Client { get; set; }
        /// <summary>
        /// The type and quantity of product sold associated through a Dictionnary
        /// </summary>
        public List<SelloutLine> BuyedProducts { get; set; } = new ();

        public void AddProduct(Product product)
        {
            List<SelloutLine> correspondingproduct = BuyedProducts.Where(element => element.SoldProduct.Id == product.Id).ToList();
            if (correspondingproduct.Count > 0)
            {
                correspondingproduct[0].Quantity += 1;
            }
            else
            {
                BuyedProducts.Add(new SelloutLine() { Quantity=1,SoldProduct=product});
            }
        }


        public void RemoveProduct(Product product) 
        {
            List<SelloutLine> correspondingproduct = BuyedProducts.Where(element => element.SoldProduct.Id == product.Id).ToList();
            if (correspondingproduct.Count > 0)
            {
                if (correspondingproduct[0].Quantity > 1)
                    correspondingproduct[0].Quantity -= 1;
                else
                    BuyedProducts.Remove(correspondingproduct[0]);
            }
            else
            {
                throw new BoucherDoubleModelException("Can't remove an product thats not here");
            }
        }
    }


}
