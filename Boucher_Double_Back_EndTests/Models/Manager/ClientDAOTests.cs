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
    public class ClientDAOTests:TestSetups
    {
        [TestMethod()]
        public async Task CreateTestAsync()
        {
            User user = await new UserDAO().Connect("Simon", "Test");
            ClientDAO dao = new()
            {
                User = user,
                Store = user.Store,
            };
            Client client=new()
            {
                Name = "Test",
                Surname = "Test",
                Mail="simtapiau@live.fr"
            };
            Assert.IsTrue(await dao.CreateAsync(client));
        }

        [TestMethod()]
        public async Task DeleteTestAsync()
        {
            User user = await new UserDAO().Connect("Simon", "Test");
            ClientDAO dao = new()
            {
                User = user,
                Store = user.Store,
            };
            Assert.IsTrue(await dao.DeleteAsync(139));
        }

        [TestMethod()]
        public async Task GetAllTestAsync()
        {
            User user = await new UserDAO().Connect("Simon", "Test");
            ClientDAO dao = new()
            {
                User = user,
                Store = user.Store,
            };
            Assert.AreEqual(2,(await dao.GetAllAsync()).Count);
        }

        [TestMethod()]
        public async Task GetByIdTestAsync()
        {
            User user = await new UserDAO().Connect("Simon", "Test");
            ClientDAO dao = new()
            {
                User = user,
                Store = user.Store,
            };
            Assert.AreEqual("client", (await dao.GetByIdAsync(139)).Name);
            Assert.AreEqual("client", (await dao.GetByIdAsync(139)).Surname);
        }

        [TestMethod()]
        public async Task UpdateTestAsync()
        {
            User user = await new UserDAO().Connect("Simon", "Test");
            ClientDAO dao = new()
            {
                User = user,
                Store = user.Store,
            };
            Client client = new()
            {
                Name = "Test",
                Surname = "Test",
                Mail = "simtapiau@live.fr",
                Id=139
            };
            await dao.UpdateAsync(client);
        }
    }
}