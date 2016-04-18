namespace CyberPark.Domain.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("calling_charge_records")]
    public partial class CallingCharge
    {
        private CallingCharge()
        {

        }

        public CallingCharge(int accountId, string serviceId, DateTime from, DateTime to)
        {
            AccountId = accountId;
            ServiceId = serviceId;
            DateFrom = from;
            DateTo = to;
            Id = Guid.NewGuid().ToString();
            CallingRecords = new HashSet<CallingRecord>();
            OperatedDate = DateTime.Now;
        }
        public string Id { get; set; }
        public string ServiceId { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public double Cost { get; set; }
        public double Charge { get; set; }
        public int AccountId { get; set; }
        public int? InvoiceId { get; set; }
        public DateTime OperatedDate { get; set; }
        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }
        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; }
        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoicd { get; set; }

        public virtual ICollection<CallingRecord> CallingRecords { get; set; }
    }
}
