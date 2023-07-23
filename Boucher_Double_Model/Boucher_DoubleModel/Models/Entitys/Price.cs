using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Boucher_DoubleModel.Models.Entitys
{
    public class Price
    {
        private int _cent;
        public int FullMonnaie { get; set; }
        public int Cent 
        { 
            get=>_cent; 
            set 
            { 
                _cent = value%10^2;
            }
        }

        public Price()
        {
        }

        [DataMember(Name = "fullMonnaie")]
        private int FullMonnaieDeserialized
        {
            set => FullMonnaie = value;
        }

        [DataMember(Name = "cent")]
        private int CentDeserialized
        {
            set => Cent = value;
        }
        public Price(decimal fullvalue)
        {
            FullMonnaie= (int)fullvalue;
            Cent =(int)(fullvalue - (int)fullvalue)*100;
        }

        public decimal GetPrice()
        {
            return (decimal)(FullMonnaie + Cent * 0.01);
        }
    }
}
