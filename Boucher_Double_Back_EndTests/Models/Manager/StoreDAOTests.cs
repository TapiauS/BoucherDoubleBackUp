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
    public class StoreDAOTests : TestSetups
    {
        [TestMethod()]
        public async Task CreateTestAsync()
        {
            User user = await new UserDAO().Connect("Simon", "Test");
            StoreDAO dao = new()
            {
                User = user,
                Store = user.Store,
            };
            Store store = new() { Adress = "Store", LogoPath = "Store", Mail = "iamastore@live.fr", Name = "Store", Town = "StoreCity" };
            Assert.IsTrue(await dao.CreateAsync(store));
        }


        [TestMethod()]
        public async Task DeleteTestAsync()
        {
            User user = await new UserDAO().Connect("Simon", "Test");
            StoreDAO dao = new()
            {
                User = user,
                Store = user.Store,
            };
            Assert.IsTrue(await dao.DeleteAsync(19));
        }

        [TestMethod()]
        public async Task GetAllTestAsync()
        {
            User user = await new UserDAO().Connect("Simon", "Test");
            StoreDAO dao = new()
            {
                User = user,
                Store = user.Store,
            };
            Assert.AreEqual(1, (await dao.GetAllAsync()).Count);
        }

        [TestMethod()]
        public async Task GetByIdTestAsync()
        {
            User user = await new UserDAO().Connect("Simon", "Test");
            StoreDAO dao = new()
            {
                User = user,
                Store = user.Store,
            };
            Assert.AreEqual("Nancy", (await dao.GetByIdAsync(8)).Town);
            Assert.AreEqual("placeholder", (await dao.GetByIdAsync(8)).Adress);
            Assert.AreEqual("test", (await dao.GetByIdAsync(8)).LogoPath);
        }

        [TestMethod()]
        public async Task UpdateTestAsync()
        {
            User user = await new UserDAO().Connect("Simon", "Test");
            StoreDAO dao = new()
            {
                User = user,
                Store = user.Store,
            };
            Store store = new() { Adress = "Store", LogoPath = "Store", Mail = "iamastore@live.fr", Name = "Store", Town = "StoreCity",Id=19,IdStore=8 };
            await dao.UpdateAsync(store);
        }
    }
}