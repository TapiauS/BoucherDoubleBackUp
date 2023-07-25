using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boucher_Double_Front.Services
{
    public interface IPrintService
    {
        Task<IList<string>> GetDeviceList();
        Task Print(string deviceName, string text);
    }
}
