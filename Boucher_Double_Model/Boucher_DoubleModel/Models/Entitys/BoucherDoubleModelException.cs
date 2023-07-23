using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boucher_DoubleModel.Models.Entitys
{
    public class BoucherDoubleModelException:Exception
    {
        public BoucherDoubleModelException():base() { }

        public BoucherDoubleModelException(string message):base(message) { }
    }
}
