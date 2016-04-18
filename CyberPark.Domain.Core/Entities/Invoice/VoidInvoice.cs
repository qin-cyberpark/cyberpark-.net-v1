using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace CyberPark.Domain.Core
{
    [Table("void_invoices")]
    public partial class VoidInvoice
    {

        public string Id { get; set; }
        public int InvoiceId { get; set; }
        public int VoidBy { get; set; }
        public DateTime VoidDate { get; set; }
        public string JsonData { get; set; }

        private VoidInvoice()
        {

        }

        [NotMapped]
        public Invoice OriginalInvoice
        {
            get
            {
                return Utility.ToObject<Invoice>(JsonData);
            }
            set
            {
                JsonData = Utility.ToJson(value);
            }
        }
    }
}
