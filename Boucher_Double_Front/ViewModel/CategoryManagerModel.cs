using Boucher_DoubleModel.Models.Entitys;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boucher_Double_Front.ViewModel
{
    public class CategoryManagerModel
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
                    return $"{app.BaseUrl}Category/image/{imageSource}";
                }
            }
            set => imageSource = value;
        }
        private FileResult fileResult;
        public FileResult FileResult { get => fileResult; set { fileResult = value; ImageSource = fileResult.FullPath; } }
        public string IdContainer { get; set; } = "0";
        public Category Category { get; set; } = new();


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
                var response = await client.PostAsync($"Category/upload/{Category.Id}", formData);
                return response.IsSuccessStatusCode;
            }
            else
                return true;
        }

        public async Task GetOneCategoryAsync(int categoryId)
        {
            if (categoryId != 0)
            {
                App app = Application.Current as App;
                HttpClient client = await app.PrepareQuery();
                HttpResponseMessage response = await client.GetAsync($"Category/{categoryId}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    Category = JsonConvert.DeserializeObject<Category>(jsonString);
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erreur", "Erreur Inconnue", "OK");
                    return;
                }
            }
        }
    }
}
