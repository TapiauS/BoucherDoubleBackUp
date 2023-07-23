using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Boucher_Double_Front.Model
{
    public class UserAccess
    {
        [PrimaryKey]
        public string Pseudo { get; set; }
        public string Password { get; set; }
    }
}
