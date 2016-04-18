namespace CyberPark.Domain.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Linq;
    using System.Runtime.Serialization;
    using Newtonsoft.Json.Serialization;


    [Table("accounts")]
    public partial class Account
    {
      
        public Account()
        {
            Products = new HashSet<Product>();
            ProductCharges = new HashSet<ProductCharge>();
            AddonCharges = new HashSet<AddonCharge>();
            CallingCharges = new HashSet<CallingCharge>();
            Adjustments = new HashSet<Adjustment>();
            Transactions = new HashSet<Transaction>();
            Invoices = new HashSet<Invoice>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string RegisterBranchId { get; set; }

        [Required]
        public string ChargeBranchId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        [StringLength(10)]
        public string Type { get; set; }
        public string InvoicePeriodType { get; set; }
        public DateTime? NextInvoiceIssueDate { get; set; }
        [Required]
        [StringLength(250)]
        public string Address { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Title { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }
        public string OrganizationName { get; set; }


        public string IdentityType { get; set; }

        public string IdentityNumber { get; set; }
        public double Balance { get; set; }
        [Column(TypeName ="bit")]
        public bool IsActive { get; set; }


        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        [ForeignKey("RegisterBranchId")]
        [InverseProperty("RegisterAccounts")]
        public virtual Branch RegisterBranch { get; set; }

        [ForeignKey("ChargeBranchId")]
        [InverseProperty("ChargeAccounts")]
        public virtual Branch ChargeBranch { get; set; }

        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
        public virtual ICollection<ProductCharge> ProductCharges { get; set; }
        public virtual ICollection<AddonCharge> AddonCharges { get; set; }
        public virtual ICollection<CallingCharge> CallingCharges { get; set; }
        public virtual ICollection<Adjustment> Adjustments { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
