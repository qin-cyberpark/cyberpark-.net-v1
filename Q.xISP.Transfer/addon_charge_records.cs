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
    
    public partial class addon_charge_records
    {
        public string Id { get; set; }
        public string FileId { get; set; }
        public System.DateTime ChargeDate { get; set; }
        public string OriNumber { get; set; }
        public System.DateTime DateFrom { get; set; }
        public Nullable<System.DateTime> DateTo { get; set; }
        public double Cost { get; set; }
        public double Charge { get; set; }
        public string ServiceId { get; set; }
        public string PhoneType { get; set; }
        public Nullable<int> AccountId { get; set; }
        public Nullable<int> InvoiceId { get; set; }
        public bool Ignored { get; set; }
        public Nullable<int> IgnoredBy { get; set; }
        public string Description { get; set; }
        public string DisplayDescription { get; set; }
        public string Warning { get; set; }
        public string Memo { get; set; }
    
        public virtual account account { get; set; }
        public virtual external_bill_files external_bill_files { get; set; }
        public virtual service service { get; set; }
        public virtual invoice invoice { get; set; }
    }
}
