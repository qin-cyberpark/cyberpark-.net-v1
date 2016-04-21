namespace CyberPark.Domain.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Linq;
    using System.Data.Entity;
    using Newtonsoft.Json;
    [Table("products")]
    public partial class Product
    {
        public Product()
        {
            Services = new HashSet<Service>();
            UsageOffers = new HashSet<ServiceUsageOffer>();
            ProductCharges = new HashSet<ProductCharge>();
            Equipments = new HashSet<Equipment>();
        }

        public string Id { get; set; }

        public int AccountId { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        public double BasePriceGSTExclusive { get; set; }
        public double DiscountRate { get; set; }
        public int NumberOfMonthPerCharge { get; set; }
        public DateTime? ServiceGivenDate { get; set; }
        public DateTime? ChargedToDate { get; set; }

        //term case
        [Column(TypeName = "bit")]
        public bool IsTermed { get; set; }
        public DateTime? TermStartDate { get; set; }
        public int? MonthsOfTerm { get; set; }

        //monthly or one-off
        [Column(TypeName = "bit")]
        public bool IsOneOff { get; set; }
        public DateTime? OneOffChargeDate { get; set; }
        [Column(TypeName = "bit")]
        public bool? HasOneOffCharged { get; set; }

        [Required]
        [StringLength(30)]
        public string Status { get; set; }

        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }

        
        public int PrepayMonths { get; set; }
        public DateTime AppliedDate { get; set; }

        //public virtual ICollection<ServicePackageTerm> Terms { get; set; }
        public virtual ICollection<Service> Services { get; set; }
        public virtual ICollection<ServiceUsageOffer> UsageOffers { get; set; }
        public virtual ICollection<ProductCharge> ProductCharges { get; set; }
        public virtual ICollection<Equipment> Equipments { get; set; }

    }
}
