

using Microsoft.VisualStudio.TestTools.UnitTesting;
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
