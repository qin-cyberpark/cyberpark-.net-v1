namespace CyberPark.Domain.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Data.Entity;

    public partial class User
    {
        public class RoleTypes
        {
            private RoleTypes()
            {

            }
            public const string Customer = "Customer";
            public const string Staff = "Staff";
            public const string Administrator = "Administrator";
            public const string Accountant = "Accountant";
            public const string Provision = "Provision";
            public const string CustomerService = "CustomerService";
            public const string Manager = "Manager";
            public const string Sales = "Sales";
        }

        public bool IsStaff
        {
            get
            {
                return Staff != null;
            }
        }

        public bool IsCustomer
        {
            get
            {
                return Customer != null;
            }
        }

        public static User Get(xISPContext db, int id)
        {
            var user = db.Users.Include(x => x.Customer)
                                .Include(x => x.Staff)
                                .SingleOrDefault(x => x.Id == id);
            return user;
        }

    }
}
