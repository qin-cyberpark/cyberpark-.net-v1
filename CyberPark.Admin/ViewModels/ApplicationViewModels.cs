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
            public string Address { get; set; }
            public string PlanId { get; set; }
            public bool NeedModem { get; set; }
            public bool NewConnection { get; set; }
            public int PrepayMonths { get; set; }
        }
    }
}