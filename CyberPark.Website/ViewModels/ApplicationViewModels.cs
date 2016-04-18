using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CyberPark.Domain.Core;
namespace CyberPark.Website.ViewModels
{
    public class ApplicationViewModels
    {
        public class BroadbandAvailabilityViewModel
        {
            public string Address { get; set; }
            public string Active { get; set; }
            public bool ADSL { get; set; }
            public bool VDSL { get; set; }
            public bool UFB { get; set; }
            public IList<Plan> ADSLs { get; set; }
            public IList<Plan> VDSLs { get; set; }
            public IList<Plan> UFBs { get; set; }
        }

        public class Summary
        {
            public Account Account { get; set; }
            public Plan Plan { get; set; }
            public bool PayMonthly { get; set; }
        }
    }
}