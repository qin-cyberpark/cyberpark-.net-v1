namespace CyberPark.Domain.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Linq;

    [Table("addon_charge_records")]
    public partial class AddonCharge
    {
        public string Id { get; set; }

        [Required]
        [StringLength(128)]
        public string FileId { get; set; }

        [Required]
        [StringLength(50)]
        public string OriNumber { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public double Cost { get; set; }

        public double Charge { get; set; }
        public DateTime ChargeDate { get; set; }

        [StringLength(128)]
        public string ServiceId { get; set; }
        public string PhoneType { get; set; }

        public int? AccountId { get; set; }

        public int? InvoiceId { get; set; }

        [Column(TypeName = "bit")]
        public bool Ignored { get; set; }

        public int? IgnoredBy { get; set; }

        public string Description { get; set; }
        public string DisplayDescription { get; set; }
        public string Warning { get; set; }
        public string Memo { get; set; }

        [ForeignKey("FileId")]
        public virtual ExternalBill OriginalBill { get; set; }
        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; }
        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }
        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoice { get; set; }
    }
}
