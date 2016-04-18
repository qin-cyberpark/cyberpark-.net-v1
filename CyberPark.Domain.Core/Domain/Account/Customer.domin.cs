namespace CyberPark.Domain.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Linq;
    using System.Runtime.Serialization;
    public partial class Customer
    {
        [NotMapped]
        public string Name
        {
            get
            {
                return string.Format("{0} {1}", FirstName, LastName);
            }
        }

        public static IList<Customer> Get(xISPContext db, Func<Customer, bool> query)
        {
            return db.Customers.Include(x => x.User)
                        .Include(x => x.Accounts)
                        .Where(query).OrderBy(x => x.Id)
                        .ToList();
        }

        public static Customer GetById(xISPContext db, int customerId)
        {
            return db.Customers.Include(x => x.User)
                        .Include(x => x.Accounts)
                        .SingleOrDefault(x => x.Id == customerId);
        }

        public Customer Save(xISPContext db)
        {
            db.Entry(this).State = EntityState.Modified;
            db.SaveChanges();
            return db.Customers.SingleOrDefault(x => x.Id.Equals(this.Id));
        }

        public static Customer Create(xISPContext db, Customer customer)
        {
            //get max id
            if(customer.Id == 0) {
                customer.Id = db.Customers.Max(x => x.Id) + 1;
            }

            db.Customers.Add(customer);
            db.SaveChanges();

            return customer;
        }
    }
}
