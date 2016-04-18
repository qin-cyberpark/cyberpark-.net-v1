namespace CyberPark.Domain.Core
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Linq;

    [Table("invoices")]
    public partial class Invoice
    {
        public class Statuses
        {
            /*
            Valid--(auto check)-->WaitForCheck--(confirm by accountant)->Valid
            Valid/WaitForCheck--(cancel by accountant)->Void
            Valid--(new invoice issued)-->sealed
            */
            private Statuses()
            {

            }
            public const string Valid = "Valid";
            public const string WaitForCheck = "WaitForCheck";
            public const string Void = "Void";
            public const string Sealed = "Sealed";
        }

        public Invoice()
        {
            AddonCharges = new HashSet<AddonCharge>();
            CallingCharges = new HashSet<CallingCharge>();
            Adjustments = new HashSet<Adjustment>();
            Transactions = new HashSet<Transaction>();
            ProductCharges = new HashSet<ProductCharge>();
        }

        #region properties
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int AccountId { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public double PreviousBalance { get; set; }

        public double ChargeAmountExcludeGST { get; set; }
        public double GST { get; set; }

        public double ProductAmount { get; set; }

        public double AddonAmount { get; set; }

        public double CallingAmount { get; set; }

        public double AdjustAmount { get; set; }

        public double TransactionAmount { get; set; }

        [Required]
        [StringLength(10)]
        public string Status { get; set; }

        public int IssuedBy { get; set; }

        public DateTime IssuedDate { get; set; }
        public DateTime DisplayIssuedDate { get; set; }
        public bool AutoDeliver { get; set; }
        public DateTime? DeliveredDate { get; set; }

        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }

        public virtual ICollection<AddonCharge> AddonCharges { get; set; }
        public virtual ICollection<Adjustment> Adjustments { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<ProductCharge> ProductCharges { get; set; }
        public virtual ICollection<CallingCharge> CallingCharges { get; set; }
        #endregion
    }
}
