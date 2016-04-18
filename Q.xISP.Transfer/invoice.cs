//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Q.xISP.Transfer
{
    using System;
    using System.Collections.Generic;
    
    public partial class invoice
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public invoice()
        {
            this.addon_charge_records = new HashSet<addon_charge_records>();
            this.adjustments = new HashSet<adjustment>();
            this.calling_charge_records = new HashSet<calling_charge_records>();
            this.product_charge_records = new HashSet<product_charge_records>();
            this.transactions = new HashSet<transaction>();
        }
    
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public System.DateTime DateFrom { get; set; }
        public System.DateTime DateTo { get; set; }
        public double PreviousBalance { get; set; }
        public double ChargeAmountExcludeGST { get; set; }
        public double GST { get; set; }
        public double ProductAmount { get; set; }
        public double AddonAmount { get; set; }
        public double CallingAmount { get; set; }
        public double AdjustAmount { get; set; }
        public double TransactionAmount { get; set; }
        public string Status { get; set; }
        public int IssuedBy { get; set; }
        public System.DateTime IssuedDate { get; set; }
        public System.DateTime DisplayIssuedDate { get; set; }
        public Nullable<bool> AutoDeliver { get; set; }
        public Nullable<System.DateTime> DeliveredDate { get; set; }
    
        public virtual account account { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<addon_charge_records> addon_charge_records { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<adjustment> adjustments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<calling_charge_records> calling_charge_records { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<product_charge_records> product_charge_records { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<transaction> transactions { get; set; }
    }
}