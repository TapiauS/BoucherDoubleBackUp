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
    public class OneCategoryListModel
    {
        public bool IsMenuDefined
        {
            get
            {
                App app = Application.Current as App;
                return app.ActivMenu != null;
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
                await Shell.Current.DisplayAlert("Erreur", "Erreur Inconnue", "OK");
                return null;
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
                await Shell.Current.DisplayAlert("Erreur", "Erreur Inconnue", "OK");
                return default;
            };
        }
    }
}
