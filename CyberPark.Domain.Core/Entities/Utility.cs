using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberPark.Domain.Core
{
    public partial class Utility 
    {
        public class BroadbandAvailability
        {
            public BroadbandAvailability(string address)
            {
                Address = address;
            }
            public string Address { get; set; }
            public bool ADSL { get; set; } = false;
            public bool VDSL { get; set; } = false;
            public bool UFB { get; set; } = false;
            public string Active { get; set; } = null;
        }
    }
}
