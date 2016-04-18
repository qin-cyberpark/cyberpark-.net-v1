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


    public partial class BalanceRecord
    {

        public BalanceRecord()
        {
            
        }

        public string Id { get; set; }

        public int AccountId { get; set; }

        public double PreviousBalance { get; set; }
        public string TransactionId { get; set; }
        public string AdjustmentId { get; set; }
        public int InvoiceId { get; set; }
        public string Operate { get; set; }
        public double Amount { get; set; }
        public double CurrentBalance { get; set; }
        public DateTime OperateDate { get; set; }
        public string Memo { get; set; }

        public virtual Account Account { get; set; }
    }
}
