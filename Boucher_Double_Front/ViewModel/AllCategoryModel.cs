﻿using Boucher_Double_Front.Model;
using Boucher_DoubleModel.Models.Entitys;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boucher_Double_Front.ViewModel
{
    public class AllCategoryModel
    {
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
                return JsonConvert.DeserializeObject<List<Category>>(jsonString).Select(category=>new ImageCategory() { Category=category, ImageSource = $"{(Application.Current as App).BaseUrl}Category/image/{category.PicturePath}" }).ToList();
            }
            else
            {
                throw new Exception("Servor access error");
            }
        }
    }
}
