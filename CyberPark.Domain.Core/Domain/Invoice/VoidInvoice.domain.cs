using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberPark.Domain.Core
{
    public partial class VoidInvoice
    {
        public VoidInvoice(Invoice invoice, int userId)
        {
            Id = Guid.NewGuid().ToString();
            InvoiceId = invoice.Id;
            JsonData = Utility.ToJson(invoice,true);       
            VoidBy = userId;
            VoidDate = DateTime.Now;
        }
    }
}
