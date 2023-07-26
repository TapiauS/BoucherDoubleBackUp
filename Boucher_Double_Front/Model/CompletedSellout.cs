using Boucher_DoubleModel.Models.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boucher_Double_Front.Model
{
    public class CompletedSellout
    {
        public Sellout Sellout { get; set; }
        public List<SelloutLine> Lines { get; set; } = new();
        private Dictionary<int, int> keyValuePairs= new();

        public CompletedSellout(List<Product> allProducts,Sellout sellout) 
        {
            foreach (var product in allProducts) 
            {
                keyValuePairs.Add(product.Id, 0);
            }

            Sellout = sellout;
            foreach (var line in Sellout.BuyedProducts)
            {
                Product product = line.SoldProduct;
                if (line.SoldProduct.GetType() != typeof(Menu))
                {
                    keyValuePairs[product.Id] = line.Quantity;
                }
                else
                {
                    keyValuePairs[product.Id] = line.Quantity;
                    Menu menu=product as Menu;
                    foreach(var menuproductline in menu.Content)
                    {
                        keyValuePairs[menuproductline.SoldProduct.Id] += menuproductline.Quantity;
                    }
                }
            }

            foreach(var product in allProducts)
            {
                Lines.Add(new() { SoldProduct = product, Quantity = keyValuePairs[product.Id] });
            }
        }
    }
}
