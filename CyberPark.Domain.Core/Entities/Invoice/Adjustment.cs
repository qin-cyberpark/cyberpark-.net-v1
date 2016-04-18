namespace CyberPark.Domain.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Linq;
    [Table("adjustments")]
    public partial class Adjustment
    {
        public string Id { get; set; }

        public int AccountId { get; set; }

        public int? InvoiceId { get; set; }

        public double Amount { get; set; }

        [Column(TypeName = "text")]
        [StringLength(65535)]
        public string Memo { get; set; }

        public int OperatedBy { get; set; }

        public DateTime OperatedDate { get; set; }

        [Column(TypeName = "bit")]
        public bool IsDeleted { get; set; }

        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }
        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoice { get; set; }
    }
}
