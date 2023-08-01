using Boucher_DoubleModel.Models.Entitys;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boucher_Double_Front.ViewModel
{
    public class UserManagerModel
    {
        public List<User> Users { get; set; } = new();
        public List<Store> Stores { get; set; } = new();



    }
}
