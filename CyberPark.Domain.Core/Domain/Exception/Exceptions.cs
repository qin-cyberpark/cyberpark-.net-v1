using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberPark.Domain.Core
{
    public class ProductChargeException : Exception
    {
        public int AccountId { get; set; }
        public ProductChargeException(string message, int accountId, Exception innerException = null) : base(message, innerException)
        {
            AccountId = accountId;
        }
    }

    public class ExternalBillException : Exception
    {
        public ExternalBillException(string message, Exception innerException = null) : base(message, innerException)
        {
        }
    }
}
