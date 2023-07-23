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
    public class ProductDAOTests : TestSetups
    {
        [TestMethod()]
        public async Task CreateTestAsync()
        {
            User user = await new UserDAO().Connect("Simon", "Test");
            ProductDAO dao = new()
            {
                User = user,
                Store = user.Store,
            };
            Product product = new()
            {
                Name = "Test",
                Category = new()
                {
                    Id = 8
                },
                Price = 10
            };
            Assert.IsTrue(await dao.CreateAsync(product));
        }

        [TestMethod()]
        public async Task DeleteTestAsync()
        {
            User user = await new UserDAO().Connect("Simon", "Test");
            ProductDAO dao = new()
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
            ProductDAO dao = new()
            {
                User = user,
                Store = user.Store,
            };
            Assert.AreEqual(2,(await  dao.GetAllAsync()).Count);
        }

        [TestMethod()]
        public async Task GetByIdTestAsync()
        {
            User user = await new UserDAO().Connect("Simon", "Test");
            ProductDAO dao = new()
            {
                User = user,
                Store = user.Store,
            };
            Assert.AreEqual("steack", (await dao.GetByIdAsync(7)).Name);
            Assert.AreEqual(5, (await dao.GetByIdAsync(7)).Price);
        }

        [TestMethod()]
        public async Task UpdateTestAsync()
        {
            User user = await new UserDAO().Connect("Simon", "Test");
            ProductDAO dao = new()
            {
                User = user,
                Store = user.Store,
            };
            Product product = new()
            {
                Name = "Test",
                Category = new()
                {
                    Id = 8
                },
                Price = 10,
                Id = 8
            };
            await dao.UpdateAsync(product);
        }
    }
}