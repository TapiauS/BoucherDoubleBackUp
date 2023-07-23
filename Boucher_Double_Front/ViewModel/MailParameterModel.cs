using Boucher_DoubleModel.Models.Entitys;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boucher_Double_Front.ViewModel
{
    public class MailParameterModel
    {
        public MailContentParameter CurrentMailParameter { get; set; }
        public List<MailContentParameter> MailParameters { get; set; }
        public async Task LoadMailContentParameterAsync()
        {
            try
            {
                App app = Application.Current as App;
                HttpClient client = await app.PrepareQuery();
                HttpResponseMessage response = await client.GetAsync("MailContentParameter");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    MailParameters = JsonConvert.DeserializeObject<List<MailContentParameter>>(json);
                    MailParameters.Insert(0, new MailContentParameter() { Name = "Aucune de ces options" });
                }
                else
                    throw new Exception("Serveur error");
            }
            catch
            {
            }
        }
    }
}
