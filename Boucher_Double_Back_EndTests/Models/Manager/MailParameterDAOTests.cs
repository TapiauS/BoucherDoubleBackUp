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
    public class MailParameterDAOTests:TestSetups
    {
        [TestMethod()]
        public async Task CreateTestAsync()
        {
            User user = await new UserDAO().Connect("Simon", "Test");
            MailParameterDAO dao = new()
            {
                User = user,
                Store = user.Store,
            };
            MailParameter mail = new()
            {
                Server = "test",
                IdStore = user.Store.IdStore,
                Name = "test",
                ConnexionType = "test",
                Port=0,
                Login="test",
                Password="test123a,",
            };
            Assert.IsTrue(await dao.CreateAsync(mail));
        }

        [TestMethod()]
        public async Task DeleteTestAsync()
        {
            User user = await new UserDAO().Connect("Simon", "Test");
            MailParameterDAO dao = new()
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
            MailParameterDAO dao = new()
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
            MailParameterDAO dao = new()
            {
                User = user,
                Store = user.Store,
            };

            Assert.AreEqual(10, (await dao.GetByIdAsync(9)).Port);
            Assert.AreEqual("testserveur", (await dao.GetByIdAsync(9)).Login);
            Assert.AreEqual("test1", (await dao.GetByIdAsync(9)).Name);
            Assert.AreEqual("simon", (await dao.GetByIdAsync(9)).Password);
            Assert.AreEqual("type", (await dao.GetByIdAsync(9)).ConnexionType);
        }

        [TestMethod()]
        public async Task UpdateTestAsync()
        {
            User user = await new UserDAO().Connect("Simon", "Test");
            MailParameterDAO dao = new()
            {
                User = user,
                Store = user.Store,
            };
            MailParameter mail = new()
            {
                Server = "test",
                IdStore = user.Store.IdStore,
                Name = "test",
                ConnexionType = "test",
                Port = 0,
                Login = "test",
                Password = "test123a,",
                Id=9
            };
            await dao.UpdateAsync(mail);
        }
    }
}