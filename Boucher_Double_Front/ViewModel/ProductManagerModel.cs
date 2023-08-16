using Boucher_DoubleModel.Models.Entitys;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boucher_Double_Front.ViewModel
{
    public class ProductManagerModel
    {
        public string imageSource;
        public string ImageSource
        {
            get
            {
                if (fileResult != null)
                {
                    return imageSource;
                }
                else
                {
                    App app = Application.Current as App;
                    return $"{app.BaseUrl}Product/image/{imageSource}";
                }
            }
            set => imageSource = value;
        }
        private FileResult fileResult;
        public FileResult FileResult { get => fileResult; set { fileResult = value; ImageSource = fileResult.FullPath; } }

        public Category Category { get; set; }

        public Product Product { get; set; } = new Product() { Price = new Price(0.1m) };

        public async Task LoadProductAsync(string idProduct)
        {
            int id = int.Parse(idProduct);
            App app = Application.Current as App;
            HttpClient client = await app.PrepareQuery();
            HttpResponseMessage response = await client.GetAsync($"Product/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                Product = JsonConvert.DeserializeObject<Product>(jsonString);
                ImageSource = Product.PicturePath;
            }
            else
            {
                throw new Exception("Server error");
            }
        }

        public async Task GetOneCategoryAsync(int idCategory)
        {
            App app = Application.Current as App;
            HttpClient client = await app.PrepareQuery();
            HttpResponseMessage response = await client.GetAsync($"Category/{idCategory}");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                Category = JsonConvert.DeserializeObject<Category>(jsonString);
            }
            else
            {
                throw new Exception("Server error");
            }
        }

        public async Task<bool> SendPictureAsync(HttpClient client)
        {
            if (FileResult != null)
            {
                var stream = await FileResult.OpenReadAsync();
                HttpContent httpContent = new StreamContent(stream);
                var formData = new MultipartFormDataContent
                {
                        { httpContent, "file",FileResult.FileName }
                };
                var response = await client.PostAsync($"Product/upload/{Product.Id}", formData);
                return response.IsSuccessStatusCode;
            }
            else
                return true;
        }


    }
}
