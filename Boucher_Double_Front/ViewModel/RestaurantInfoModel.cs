using Boucher_DoubleModel.Models.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boucher_Double_Front.ViewModel
{
    public class RestaurantInfoModel
    {
        public ImageSource Image { get; set; }
        public string PicturePath { get; set; }
        public FileResult FileResult { get; set; }

        public Store Store
        {
            get
            {
                App app = Application.Current as App;
                return app.User.Store;
            }
        }


    }
}
