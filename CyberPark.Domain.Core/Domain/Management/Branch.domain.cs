namespace CyberPark.Domain.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("branches")]
    public partial class Branch
    {
        public Branch()
        {
            //RegisterAccounts = new HashSet<Account>();
            ChargeAccounts  = new HashSet<Account> ();
        }

        public string Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public virtual ICollection<Account> RegisterAccounts { get; set; }
        public virtual ICollection<Account> ChargeAccounts { get; set; }
    }
}