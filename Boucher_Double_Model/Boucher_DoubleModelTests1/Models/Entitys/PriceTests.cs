using Microsoft.VisualStudio.TestTools.UnitTesting;
using Boucher_DoubleModel.Models.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boucher_DoubleModel.Models.Entitys.Tests
{
    [TestClass()]
    public class PriceTests
    {

        [TestMethod()]
        public void GetPriceTest()
        {
            Price onlyfullmonnaie = new() { FullMonnaie = 3, Cent = 0 };
            Price onlyCent = new() { Cent = 5, FullMonnaie = 0 };
            Price combinaison = new() { Cent = 4, FullMonnaie = 9 };

            Assert.AreEqual(3.0m, onlyfullmonnaie.GetPrice());
            Assert.AreEqual(0.05m, onlyCent.GetPrice());
            Assert.AreEqual(9.04m, combinaison.GetPrice());
        }

        [TestMethod()]
        public void PriceTest()
        {
            Price onlyfullmonnaie = new(3.00m);
            Price onlyCent = new(0.05m) ;
            Price combinaison = new(9.04m) ;
            Assert.AreEqual(0, onlyfullmonnaie.Cent);
            Assert.AreEqual(3, onlyfullmonnaie.FullMonnaie);
            Assert.AreEqual(0,onlyCent.FullMonnaie);
            Assert.AreEqual(5, onlyCent.Cent);
            Assert.AreEqual(9, combinaison.FullMonnaie);
            Assert.AreEqual(4, combinaison.Cent);
        }
    }
}