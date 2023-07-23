using Boucher_Double_Front.Model;
using Boucher_DoubleModel.Models.Entitys;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Boucher_Double_Front.Database
{
    public class DatabaseHelper
    {
        SQLiteConnection database;

        public DatabaseHelper(string databasePath)
        {
            database = new (databasePath);
            database.CreateTable<UserAccess>();
            database.CreateTable<MailContentParameter>();
            database.CreateTable<BillParameter>();
        }

        public UserAccess GetKnowUser()
        {
            return database.Table<UserAccess>().FirstOrDefault();
        }

        public void DeleteKnowUser()
        {
            database.DeleteAll<UserAccess>();
        }

        public void SaveUserInfo(UserAccess item)
        {
            database.DeleteAll<UserAccess>();
            database.InsertOrReplace(item);
        }

        public MailContentParameter GetKnowMailContentParameter()
        {
            return database.Table<MailContentParameter>().FirstOrDefault();
        }

        public void SaveMailContentParameter(MailContentParameter item)
        {
            database.DeleteAll<MailContentParameter>();
            database.InsertOrReplace(item);
        }

        public BillParameter GetKnowBillParameter()
        {
            return database.Table<BillParameter>().FirstOrDefault();
        }

        public void SaveBillParameter(BillParameter item)
        {
            database.DeleteAll<BillParameter>();
            database.InsertOrReplace(item);
        }
    }
}
