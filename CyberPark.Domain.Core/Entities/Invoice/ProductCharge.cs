namespace CyberPark.Domain.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("product_charge_records")]
    public partial class ProductCharge
    {
        private ProductCharge()
        {

        }

        public ProductCharge(int accountId, string packgeId)
        {
            Id = Guid.NewGuid().ToString();
            AccountId = accountId;
            ProductId = packgeId;
            ChargedDate = DateTime.Now;
        }
        public string Id { get; private set; }

        [Required]
        [StringLength(128)]
        public string ProductId { get; private set; }

        public int AccountId { get; set; }

        [Column(name: "PreviousProductChargedToDate")]
        public DateTime PreviousProductChargedToDate { get; set; }
        [Column(name: "CurrentProductChargedToDate")]
        public DateTime CurrentProductChargedToDate { get; set; }

        public double AmountGSTExclusive { get; set; }

        public DateTime ChargedDate { get; set; }

        public int? InvoiceId { get; set; }

        [StringLength(255)]
        public string Memo { get; set; }

        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoice { get; set; }

        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

    }
}
