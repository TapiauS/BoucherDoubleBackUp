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
    public class MenuDAOTests : TestSetups
    {
        [TestMethod()]
        public async Task CreateTestAsync()
        {
            User user = await new UserDAO().Connect("Simon", "Test");
            MenuDAO dao = new()
            {
                User = user,
                Store = user.Store,
            };
            Dictionary<Product, int> content = new()
            {
                { await new ProductDAO() { User = user, Store = user.Store }.GetByIdAsync(7), 2 }
            };
            Menu menu = new()
            {
                Category = await new CategoryDAO() {User=user,Store=user.Store }.GetByIdAsync(16),
                Name = "test",
                Content = content
            };
            Assert.IsTrue(await dao.CreateAsync(menu));

        }

        [TestMethod()]
        public async Task DeleteTest()
        {
            User user = await new UserDAO().Connect("Simon", "Test");
            MenuDAO dao = new()
            {
                User = user,
                Store = user.Store,
            };
            Assert.IsTrue(await dao.DeleteAsync(16));
        }

        [TestMethod()]
        public async Task GetAllTestAsync()
        {
            User user = await new UserDAO().Connect("Simon", "Test");
            MenuDAO dao = new()
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
            MenuDAO dao = new()
            {
                User = user,
                Store = user.Store,
            };
            Assert.AreEqual("menu", (await dao.GetByIdAsync(16)).Name);
            Assert.AreEqual(10, (await dao.GetByIdAsync(16)).Price);
        }

        [TestMethod()]
        public async Task UpdateTestAsync()
        {
            User user = await new UserDAO().Connect("Simon", "Test");
            MenuDAO dao = new()
            {
                User = user,
                Store = user.Store,
            };
            Dictionary<Product, int> content = new()
            {
                { await new ProductDAO() { User = user, Store = user.Store }.GetByIdAsync(7), 2 }
            };
            Menu menu = new()
            {
                Category = await new CategoryDAO() { User = user, Store = user.Store }.GetByIdAsync(16),
                Name = "test",
                Content = content,
                Id=16
            };
            dao.UpdateAsync(menu);
        }
    }
}