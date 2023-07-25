using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boucher_DoubleModel.Models.Entitys
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Begin { get; set; } = DateTime.Now;
        public DateTime End { get; set; }= DateTime.Now;
        public Store Store { get; set; }
    }
}
