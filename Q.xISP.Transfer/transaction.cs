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
    
    public partial class transaction
    {
        public string Id { get; set; }
        public Nullable<int> AccountId { get; set; }
        public Nullable<int> InvoiceId { get; set; }
        public System.DateTime Date { get; set; }
        public double Amount { get; set; }
        public string Type { get; set; }
        public string CardHolder { get; set; }
        public string CardNumber { get; set; }
        public string DpsTxnRef { get; set; }
        public string DpsResponse { get; set; }
        public string TxnMac { get; set; }
        public int OperatedBy { get; set; }
        public System.DateTime OperatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public string Memo { get; set; }
    
        public virtual account account { get; set; }
        public virtual invoice invoice { get; set; }
    }
}