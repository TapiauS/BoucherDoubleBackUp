
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boucher_DoubleModel.Models.Entitys
{
    public class MailContentParameter
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string MailObject { get; set; }
        public string MailFoot { get; set; }
        public string MailHead { get; set; }
        public int IdStore { get; set; }
    }
}
