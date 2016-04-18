namespace CyberPark.Domain.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("transactions")]
    public partial class Transaction
    {
        public string Id { get; set; }

        public int AccountId { get; set; }

        public int? InvoiceId { get; set; }

        public DateTime Date { get; set; }

        public double Amount { get; set; }

        [StringLength(30)]
        public string Type { get; set; }

        [StringLength(20)]
        public string CardHolder { get; set; }

        [StringLength(20)]
        public string CardNumber { get; set; }

        [StringLength(20)]
        public string DpsTxnRef { get; set; }

        [StringLength(20)]
        public string DpsResponse { get; set; }

        [StringLength(20)]
        public string TxnMac { get; set; }

        public int OperatedBy { get; set; }

        public DateTime OperatedDate { get; set; }

        [Column(TypeName = "bit")]
        public bool IsDeleted { get; set; }

        [Column(TypeName = "text")]
        [StringLength(65535)]
        public string Memo { get; set; }

        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }

        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoice { get; set; }
    }
}
