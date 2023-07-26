using Boucher_Double_Front.Platforms.Windows;
using Boucher_Double_Front.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
[assembly: Dependency(typeof(PrinterServiceRenderer))]
namespace Boucher_Double_Front.Platforms.Windows
{ 
    public class PrinterServiceRenderer : IPrintService
    {
        public async Task<IList<string>> GetDeviceList()
        {
            return default;
        }

        public Task Print(string deviceName, string text)
        {
            return default;
            /*throw new NotImplementedException();*/
        }
    }
}
