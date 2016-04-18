namespace CyberPark.Domain.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Linq;
    using System.Runtime.Serialization;
    [Table("customers")]
    public partial class Customer
    {
        public class IdentityTypes
        {
            public const string DriverLience = "Driver Licence";
            public const string Passport = "Passport";
            public const string OrganizationRegNo = "Org. Reg. No.";
        }
        public Customer()
        {
            Accounts = new HashSet<Account>();
        }

        [Key, ForeignKey("User")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(20)]
        public string Title { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(20)]
        public string Mobile { get; set; }

        [StringLength(20)]
        public string IdentityType { get; set; }

        [StringLength(50)]
        public string IdentityNumber { get; set; }

        public DateTime SignUpDate { get; set; }

        public virtual User User { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }

    }
}
