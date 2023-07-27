using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boucher_Double_Front.Model
{
    public class Theme
    {
        public string BackgroundColorHexCode { get; set; }= Colors.Blue.ToRgbaHex();
        public string ButtonColorHexCode { get; set; }= Colors.Red.ToRgbaHex();
        public string ShellColorHexCode { get; set; }= Colors.Yellow.ToRgbaHex();

        public string TextColorHexCode { get; set; }=Colors.Green.ToRgbaHex();
    }
}
