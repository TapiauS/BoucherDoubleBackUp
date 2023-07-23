using Microsoft.VisualStudio.TestTools.UnitTesting;
using Boucher_Double_Back_End.Models.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Boucher_Double_Back_EndTests.Models.Manager;
using Boucher_DoubleModel.Models.Entitys;
using Boucher_DoubleModel.Models;

namespace Boucher_Double_Back_End.Models.Manager.Tests
{
    [TestClass()]
    public class UserDAOTests:TestSetups
    {
        [TestMethod()]
        public async Task CreateTest()
        {
            User user = await new UserDAO().Connect("Simon", "Test");
            UserDAO dao = new()
            {
                User = user,
                Store = user.Store,
            };
            User newuser = new()
            {
                Name = "Test",
                Login = "Christophe",
                Password = "Test2",
                Mail = "t@1",
                Store = user.Store,
                Surname = "Test",
                Role =Role.NORMAL
            };
            Assert.IsTrue(await dao.CreateAsync(newuser));
            
        }

        [TestMethod()]
        public async Task DeleteTest()
        {
            User user = await new UserDAO().Connect("Simon", "Test");
            UserDAO dao = new()
            {
                User = user,
                Store = user.Store,
            };
            User deleteUser = new()
            {
                IdUser =109 ,
            };
            Assert.IsTrue(await dao.DeleteAsync(deleteUser.IdUser));
        }

        [TestMethod()]
        public async Task GetAllTest()
        {
            User user = await new UserDAO().Connect("Simon", "Test");
            UserDAO dao = new()
            {
                User = user,
                Store = user.Store,
            };
            Assert.AreEqual(2,(await dao.GetAllAsync()).Count);
        }

        [TestMethod()]
        public async Task GetByIdTest()
        {
            User user = await new UserDAO().Connect("Simon", "Test");
            UserDAO dao = new()
            {
                User = user,
                Store = user.Store,
            };
            Assert.AreEqual("sacrifice", (await dao.GetByIdAsync(109)).Login);
        }

        [TestMethod()]
        public void UpdateTest()
        {

        }

        [TestMethod()]
        public async Task ConnectTest()
        {
            Assert.AreEqual("Simon",(await new UserDAO().Connect("Simon", "Test")).Login);
        }
    }
}