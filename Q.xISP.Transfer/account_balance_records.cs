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
    
    public partial class account_balance_records
    {
        public string Id { get; set; }
        public int AccountId { get; set; }
        public double PreviousBalance { get; set; }
        public string TransactionId { get; set; }
        public string AdjustmentId { get; set; }
        public Nullable<int> InvoiceId { get; set; }
        public string Operate { get; set; }
        public double Amount { get; set; }
        public double CurrentBalance { get; set; }
        public System.DateTime OperateDate { get; set; }
        public string Memo { get; set; }
    
        public virtual account account { get; set; }
    }
}