using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberPark.Domain.Core
{
    public partial class Transaction
    { 
        public static IList<Transaction> GetRecentTransactionsByAccountId(xISPContext db, int accountId, int rows = 10)
        {
            return db.Transactions.Where(x => x.AccountId == accountId && !x.IsDeleted).ToList();
        }


        public static Transaction GetById(xISPContext db, string id)
        {
            return db.Transactions.Where(x => x.Id.Equals(id)).SingleOrDefault();
        }

        /// <summary>
        /// create
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public Transaction Create(xISPContext db, int userId)
        {
            using (var trans = db.Database.BeginTransaction())
            {
                //add tranasction
                Id = Guid.NewGuid().ToString();
                OperatedBy = userId;
                OperatedDate = DateTime.Now;
                db.Transactions.Add(this);

                //update balance
                db.Entry(this).Reference(x => x.Account).Load();
                Account.Balance -= Amount;
              
                db.SaveChanges();
                trans.Commit();
                return db.Transactions.SingleOrDefault(x => x.Id.Equals(Id));
            }
        }

        public bool Delete(xISPContext db, int userId)
        {
            if (InvoiceId != null)
            {
                return false;
            }

            using (var trans = db.Database.BeginTransaction())
            {
                //delete transaction
                IsDeleted = true;
                db.Entry(this).State = EntityState.Modified;
                OperatedBy = userId;
                OperatedDate = DateTime.Now;

                //update balance
                db.Entry(this).Reference(x => x.Account).Load();
                Account.Balance += Amount;

                db.SaveChanges();
                trans.Commit();
                return db.Transactions.SingleOrDefault(x => x.Id.Equals(Id) && !x.IsDeleted) == null;
            }
        }
    }
}
