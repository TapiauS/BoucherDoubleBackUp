namespace Boucher_DoubleModel.Models.Entitys
{
    /// <summary>
    /// Represent an aglomeration of product sold as a single entity
    /// </summary>
    public class Menu : Product
    {
        /// <summary>
        /// Associate each product with its quantity inside the menu
        /// </summary>
        public List<SelloutLine> Content { get; set; } = new();

        public void AddProduct(Product product)
        {
            List<SelloutLine> correspondingproduct = Content.Where(element => element.SoldProduct.Id == product.Id).ToList();
            if (correspondingproduct.Count > 0)
            {
                correspondingproduct[0].Quantity += 1;
            }
            else
            {
                Content.Add(new SelloutLine() { Quantity = 1, SoldProduct = product });
            }
        }


        public void RemoveProduct(Product product)
        {
            List<SelloutLine> correspondingproduct = Content.Where(element => element.SoldProduct.Id == product.Id).ToList();
            if (correspondingproduct.Count > 0)
            {
                if (correspondingproduct[0].Quantity > 1)
                    correspondingproduct[0].Quantity -= 1;
                else
                    Content.Remove(correspondingproduct[0]);
            }
            else
            {
                throw new BoucherDoubleModelException("Can't remove an product thats not here");
            }
        }
    }
}
