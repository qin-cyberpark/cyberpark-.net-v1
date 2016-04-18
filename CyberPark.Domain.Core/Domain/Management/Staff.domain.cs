using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace CyberPark.Domain.Core
{
    public partial class Staff
    {

        public static IList<Staff> Get(xISPContext db)
        {
            return db.Staffs.Include(x=>x.User).Include(x=>x.Branch).ToList();
        }

        public static Staff GetById(xISPContext db, int id)
        {
            return db.Staffs.Include(x => x.User).Include(x => x.Branch).Where(x=>x.Id == id).SingleOrDefault();
        }

        public Staff Create(xISPContext db)
        {
            db.Staffs.Add(this);
            db.SaveChanges();
            return db.Staffs.Include(x => x.User).SingleOrDefault(x => x.Id.Equals(this.Id));
        }

        public static bool Delete(xISPContext db, int id)
        {
            var s = db.Staffs.SingleOrDefault(x => x.Id.Equals(id));
            if (s == null)
            {
                return false;
            }
            db.Staffs.Remove(s);
            db.SaveChanges();
            s = db.Staffs.SingleOrDefault(x => x.Id.Equals(id));
            return s == null;
        }

        public Staff Save(xISPContext db)
        {
            User.Email = Email;
            User.PhoneNumber = Mobile;
            db.Entry(this).State = EntityState.Modified;
            db.SaveChanges();
            return db.Staffs.SingleOrDefault(x => x.Id.Equals(this.Id));
        }
    }
}
