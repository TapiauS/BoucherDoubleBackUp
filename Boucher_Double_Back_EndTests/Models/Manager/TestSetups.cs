using Boucher_Double_Back_End.Controllers;
using Boucher_Double_Back_End.Models.Manager;
using Boucher_DoubleModel.Models;
using Boucher_DoubleModel.Models.Entitys;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Boucher_Double_Back_EndTests.Models.Manager
{
    [TestClass]
    public class TestSetups
    {
        private TransactionScope _transaction;
        [TestInitialize]
        public void Initialize()
        {

            _transaction = new TransactionScope();

            // Set up any necessary test data here
        }

        [TestCleanup]
        public void Cleanup()
        {
            _transaction.Dispose(); // Rolls back the transaction
        }
    }
}
