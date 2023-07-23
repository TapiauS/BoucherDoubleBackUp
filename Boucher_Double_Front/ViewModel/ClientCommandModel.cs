using Boucher_DoubleModel.Models.Entitys;
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
    }
}
