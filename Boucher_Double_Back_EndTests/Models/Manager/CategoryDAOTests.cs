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
    public class CategoryDAOTests:TestSetups
    {
        [TestMethod()]
        public async Task CreateTestAsync()
        {
            User user = await new UserDAO().Connect("Simon", "Test");
            CategoryDAO dao = new()
            {
                User = user,
                Store = user.Store,
            };
            Category category = new()
            {
                Name = "Test",
            };
            Assert.IsTrue(await dao.CreateAsync(category));
        }

        [TestMethod()]
        public async Task DeleteTestAsync()
        {
            User user = await new UserDAO().Connect("Simon", "Test");
            CategoryDAO dao = new()
            {
                User = user,
                Store = user.Store,
            };
            Assert.IsTrue(await dao.DeleteAsync(8));
        }

        [TestMethod()]
        public async Task GetAllTestAsync()
        {
            User user = await new UserDAO().Connect("Simon", "Test");
            CategoryDAO dao = new()
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
            CategoryDAO dao = new()
            {
                User = user,
                Store = user.Store,
            };
            Assert.AreEqual("test1", (await dao.GetByIdAsync(8)).Name);
        }

        [TestMethod()]
        public async Task UpdateTest()
        {
            User user = await new UserDAO().Connect("Simon", "Test");
            CategoryDAO dao = new()
            {
                User = user,
                Store = user.Store,
            };
            Category category = new()
            {
                Name = "Test",
                Id = 8
            };
            await dao.UpdateAsync(category);
        }
    }
}