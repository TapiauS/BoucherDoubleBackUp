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
    public class BillParameterDAOTests:TestSetups
    {
        [TestMethod()]
        public async Task CreateTestAsync()
        {
            User user = await new UserDAO().Connect("Simon", "Test");
            BillParameterDAO dao = new()
            {
                User = user,
                Store = user.Store,
            };
            BillParameter bill = new()
            {
                IdStore = user.Store.IdStore,
                Foot = "test",
                Name = "test",
                SpecialMention = "test",
                Mention = "test",
            };
            Assert.IsTrue(await dao.CreateAsync(bill));
        }

        [TestMethod()]
        public async Task DeleteTestAsync()
        {
            User user = await new UserDAO().Connect("Simon", "Test");
            BillParameterDAO dao = new()
            {
                User = user,
                Store = user.Store,
            };
            Assert.IsTrue(await dao.DeleteAsync(9));
        }

        [TestMethod()]
        public async Task GetAllTestAsync()
        {
            User user = await new UserDAO().Connect("Simon", "Test");
            BillParameterDAO dao = new()
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
            BillParameterDAO dao = new()
            {
                User = user,
                Store = user.Store,
            };
            Assert.AreEqual("ref", (await dao.GetByIdAsync(9)).Name);
        }

        [TestMethod()]
        public async Task UpdateTestAsync()
        {

            User user = await new UserDAO().Connect("Simon", "Test");
            BillParameterDAO dao = new()
            {
                User = user,
                Store = user.Store,
            };
            BillParameter bill = new()
            {
                IdStore = user.Store.IdStore,
                Foot = "test",
                Name = "test",
                SpecialMention = "test",
                Mention = "test",
                Id=9
            };
            await dao.UpdateAsync(bill);
        }
    }
}