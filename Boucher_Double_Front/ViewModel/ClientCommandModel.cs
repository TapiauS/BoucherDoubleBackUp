using Boucher_Double_Front.Model;
using Boucher_DoubleModel.Models.Entitys;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boucher_Double_Front.ViewModel
{
    public class ClientCommandModel
    {
        public List<SelloutLine> Lines
        {
            get
            {
                App appInstance = Application.Current as App;
                if (appInstance.ActivCommand != null)
                    return appInstance.ActivCommand.BuyedProducts;
                return null;
            }
        }

        public List<ImageCategory> AllCategory { get; set; }

        public ImageCategory ActiveCategory { get; set; }

        public List<ImageCategory> ParentCategory { get; set; } = new();

        public List<Event> CompatibleEvents { get; set; }

        public async Task GetAllEventAsync()
        {
            App app = Application.Current as App;
            HttpClient client = await app.PrepareQuery();
            HttpResponseMessage response = await client.GetAsync("Event");
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                List<Event> _events = JsonConvert.DeserializeObject<List<Event>>(json);
                List<Event> filteredEvents = _events.Where(_event => _event.End > app.ActivCommand.SelloutDate && _event.Begin < app.ActivCommand.ReceiptDate).ToList();
                filteredEvents.Insert(0, new() { Name = "Aucune de ces options" });
                CompatibleEvents = filteredEvents;
            }
            else
                throw new Exception("Server access error");
        }


        public async Task<List<ImageCategory>> GetAllCategoryAsync()
        {
            App appInstance = Application.Current as App;
            HttpClient client = await appInstance.PrepareQuery();
            HttpResponseMessage response = await client.GetAsync("Category");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                List<Category> categorys = JsonConvert.DeserializeObject<List<Category>>(jsonString);
                return categorys.Select(category => new ImageCategory
                {
                    Category = category,
                    ImageSource = $"{(Application.Current as App).BaseUrl}Category/image/{category.PicturePath}"
                }).ToList();
            }
            else
            {
                throw new Exception("Servor access error");
            }
        }


        public async Task<List<ImageCategory>> GetSubCategoryAsync(int id)
        {
            App appInstance = Application.Current as App;
            HttpClient client = await appInstance.PrepareQuery();
            HttpResponseMessage response = await client.GetAsync($"Category/SubCategory/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Category>>(jsonString).Select(category => new ImageCategory() { Category = category, ImageSource = $"{(Application.Current as App).BaseUrl}Category/image/{category.PicturePath}" }).ToList();
            }
            else
            {
                throw new Exception("Servor access error");
            }
        }


        public async Task<List<ImageProduct>> GetAllProductAsync(int categoryId)
        {
            App appInstance = Application.Current as App;
            HttpClient client = await appInstance.PrepareQuery();
            HttpResponseMessage response = await client.GetAsync("Product");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                List<Product> products = JsonConvert.DeserializeObject<List<Product>>(jsonString);
                List<ImageProduct> images = products.Select(product => new ImageProduct
                {
                    Product = product,
                    ImageSource = $"{(Application.Current as App).BaseUrl}Product/image/{product.PicturePath}"
                }).ToList();
                return images.Where(imageproduct => imageproduct.Product.Category.Id == categoryId).ToList();
            }
            else
            {
                throw new Exception("Server error");
            }
        }

        public async Task<List<ImageProduct>> GetAllMenuAsync(int categoryId)
        {
            App app = Application.Current as App;
            HttpClient client = await app.PrepareQuery();
            HttpResponseMessage httpResponse = await client.GetAsync("Menu");
            if (httpResponse.IsSuccessStatusCode)
            {
                string jsoncontent = await httpResponse.Content.ReadAsStringAsync();
                List<Menu> menus = JsonConvert.DeserializeObject<List<Menu>>(jsoncontent);
                return menus.Where(menu => menu.Category.Id == categoryId).Select(menu => new ImageProduct() { ImageSource = $"{(Application.Current as App).BaseUrl}Product/image/{menu.PicturePath}", Product = menu }).ToList();
            }
            else
            {
                throw new Exception("Server error");
            };
        }
    }
}
