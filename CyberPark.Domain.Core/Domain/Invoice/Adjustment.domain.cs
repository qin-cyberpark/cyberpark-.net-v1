using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberPark.Domain.Core
{
    public partial class Adjustment
    {
        public static IList<Adjustment> GetRecentAdjustmentsByAccountId(xISPContext db, int accountId, int rows = 10)
        {
            return db.Adjustments.Where(x => x.AccountId == accountId && !x.IsDeleted).OrderByDescending(x => x.OperatedDate).Take(rows).ToList();
        }

        public static Adjustment GetById(xISPContext db, string id)
        {
            return db.Adjustments.Where(x => x.Id.Equals(id)).SingleOrDefault();
        }

        /// <summary>
        /// create
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public Adjustment Create(xISPContext db, int userId)
        {
            using (var trans = db.Database.BeginTransaction())
            {
                //add adjustmen
                Id = Guid.NewGuid().ToString();
                OperatedBy = userId;
                OperatedDate = DateTime.Now;
                db.Adjustments.Add(this);

                //update balance
                db.Entry(this).Reference(x => x.Account).Load();
                Account.Balance += Amount;
                db.SaveChanges();
                trans.Commit();
                return db.Adjustments.SingleOrDefault(x => x.Id.Equals(Id));
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
                //delete adjustment
                IsDeleted = true;
                db.Entry(this).State = EntityState.Modified;
                OperatedBy = userId;
                OperatedDate = DateTime.Now;

                //update balance
                db.Entry(this).Reference(x => x.Account).Load();
                Account.Balance -= Amount;

                db.SaveChanges();
                trans.Commit();
                return db.Adjustments.SingleOrDefault(x => x.Id.Equals(Id) && !x.IsDeleted) == null;
            }
        }
    }
}
