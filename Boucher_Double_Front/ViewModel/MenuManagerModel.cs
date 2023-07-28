using Boucher_DoubleModel.Models.Entitys;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boucher_Double_Front.ViewModel
{
    public class MenuManagerModel
    {
        public Menu Menu
        {
            get
            {
                App app = Application.Current as App;
                return app.ActivMenu;
            }
        }
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
                    return $"{app.BaseUrl}Product/image/{app.ActivMenu.PicturePath}";
                }
            }
            set => imageSource = value;
        }
        private FileResult fileResult;
        public FileResult FileResult { get => fileResult; set { fileResult = value; ImageSource = fileResult.FullPath; } }
        public List<SelloutLine> Lines
        {
            get
            {
                App appInstance = Application.Current as App;
                return appInstance.ActivMenu.Content;
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
                var response = await client.PostAsync($"Menu/upload/{Menu.Id}", formData);
                return response.IsSuccessStatusCode;
            }
            else
                return true;
        }

        public async Task GetOneCategoryAsync(int IdCategory)
        {
            App app = Application.Current as App;
            HttpClient client = await app.PrepareQuery();
            HttpResponseMessage response = await client.GetAsync($"Category/{IdCategory}");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                Menu.Category = JsonConvert.DeserializeObject<Category>(jsonString);
            }
            else
            {
                await Shell.Current.DisplayAlert("Erreur", "Erreur Inconnue", "OK");
                return;
            }
        }

        public async Task LoadOneMenuAsync(int IdMenu)
        {
            App app = Application.Current as App;
            HttpClient client = await app.PrepareQuery();
            HttpResponseMessage httpResponseMessage = await client.GetAsync($"Menu/{IdMenu}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string jsoncontent = await httpResponseMessage.Content.ReadAsStringAsync();
                app.ActivMenu = JsonConvert.DeserializeObject<Boucher_DoubleModel.Models.Entitys.Menu>(jsoncontent);
            }
            else
                Shell.Current.DisplayAlert("Erreur", "Erreur d'accés au serveur", "Ok");
        }
    }
}
