using Microsoft.VisualStudio.TestTools.UnitTesting;
using Boucher_Double_Back_End.Models.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Boucher_Double_Back_EndTests.Models.Manager;
using Boucher_DoubleModel.Models.Entitys;

namespace Boucher_Double_Back_End.Models.Manager.Tests
{
    [TestClass()]
    public class SelloutDAOTests : TestSetups
    {
        [TestMethod()]
        public async Task CreateTestAsync()
        {
            User user = await new UserDAO().Connect("Simon", "Test");
            SelloutDAO dao = new()
            {
                User = user,
                Store = user.Store,
            };
            Sellout sellout = new()
            {
                ReceiptDate = DateTime.Now,
                SelloutDate = new DateTime(2023, 06, 26),
                Client = await new ClientDAO() { User = user, Store = user.Store }.GetByIdAsync(139),
                BuyedProducts = new Dictionary<Product, int> { { await new ProductDAO() { User = user, Store = user.Store }.GetByIdAsync(7), 2 }, { await new ProductDAO() { User = user, Store = user.Store }.GetByIdAsync(16), 1 } },
                Store = user.Store,
            };
            Assert.IsTrue(await dao.CreateAsync(sellout));
        }



        [TestMethod()]
        public async Task DeleteTestAsync()
        {
            User user = await new UserDAO().Connect("Simon", "Test");
            SelloutDAO dao = new()
            {
                User = user,
                Store = user.Store,
            };
            Assert.IsTrue(await dao.DeleteAsync(7));
        }

        [TestMethod()]
        public async Task GetAllTestAsync()
        {
            User user = await new UserDAO().Connect("Simon", "Test");
            SelloutDAO dao = new()
            {
                User = user,
                Store = user.Store,
            };
            Assert.AreEqual(1,(await dao.GetAllAsync()).Count);
        }

        [TestMethod()]
        public async Task GetByIdTestAsync()
        {
            User user = await new UserDAO().Connect("Simon", "Test");
            SelloutDAO dao = new()
            {
                User = user,
                Store = user.Store,
            };
            Assert.AreEqual(new DateTime(2023, 06, 26), (await dao.GetByIdAsync(7)).ReceiptDate);
            Assert.AreEqual(new DateTime(2023, 08, 26), (await dao.GetByIdAsync(7)).SelloutDate);
        }

        [TestMethod()]
        public async Task UpdateTestAsync()
        {
            User user = await new UserDAO().Connect("Simon", "Test");
            SelloutDAO dao = new()
            {
                User = user,
                Store = user.Store,
            };
            Sellout sellout = new()
            {
                ReceiptDate = DateTime.Now,
                SelloutDate = new DateTime(2023, 06, 26),
                Client = await new ClientDAO() { User = user, Store = user.Store }.GetByIdAsync(139),
                BuyedProducts = new Dictionary<Product, int> { { await new ProductDAO() { User = user, Store = user.Store }.GetByIdAsync(7), 2 }, { await new ProductDAO() { User = user, Store = user.Store }.GetByIdAsync(16), 1 } },
                Store = user.Store,
                Id=7
            };
            await dao.UpdateAsync(sellout);
        }
    }
}