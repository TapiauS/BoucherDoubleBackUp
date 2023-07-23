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
    public class SelloutTests
    {

        [TestMethod()]
        public void AddProducTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void RemoveProducTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GenerateBillTest()
        {
            Product steack = new() { Price = 2.5m, Name = "Steack", Id = 1, Category = new() };
            List<SelloutLine> products = new();
            products.Add(new() { SoldProduct = steack,Quantity=5 });
            Sellout sell = new Sellout() { Client = new(), SelloutDate = DateTime.Now, BuyedProducts = products };
            BillParameter billParameter = new() { Foot="placeholder",Mention="placeholder",SpecialMention="Placeholder"};
            Assert.AreEqual("rien", sell.GenerateBill(billParameter));
        }
    }
}