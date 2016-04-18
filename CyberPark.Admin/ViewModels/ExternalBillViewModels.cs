using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CyberPark.Domain.Core;
using CyberPark.Website.Models;

namespace CyberPark.Website.ViewModels
{
    public class ExternalBillViewModels
    {
        public class RecordCounter
        {
            public RecordCounter(string number, int count){
                Number = number;
                Count = count;
            }
            public string Number { get; set; }
            public int Count { get; set; }
        }

        public class BillViewModel
        {
            public BillViewModel(ExternalBill bill)
            {
                Bill = bill;

                //unmatched calls
                UnmatchedCalls = new List<RecordCounter>();
                bill.CallingRecords.Where(x => x.ServiceId == null && !x.Ignored).GroupBy(x => x.OriNumber)
                    .ToList().ForEach(r => UnmatchedCalls.Add(new RecordCounter(r.Key, r.Count())));

                //unmatched servics
                UnmatchedServices = new List<RecordCounter>();
                bill.AddonCharges.Where(x => x.ServiceId == null && !x.Ignored).GroupBy(x => x.OriNumber)
                    .ToList().ForEach(r => UnmatchedServices.Add(new RecordCounter(r.Key, r.Count())));

                //ignored calls
                //IgnoredCalls = new SortedList<string, IList<CallingRecord>>();
                //bill.CallingRecords.Where(x => x.Ignored).GroupBy(x => x.OriNumber)
                //    .ToList().ForEach(r => IgnoredCalls.Add(r.Key, r.ToList()));

                //ignored services
                //IgnoredServices = new SortedList<string, IList<AddonCharge>>();
                //bill.AddonCharges.Where(x => x.Ignored).GroupBy(x => x.OriNumber)
                //    .ToList().ForEach(r => IgnoredServices.Add(r.Key, r.ToList()));
            }

            public ExternalBill Bill { get; private set; }
            public List<RecordCounter> UnmatchedCalls { get; private set; }
            public List<RecordCounter> UnmatchedServices { get; private set; }

            /*
            public SortedList<string, IList<CallingRecord>> IgnoredCalls { get; private set; }
            public SortedList<string, IList<AddonCharge>> IgnoredServices { get; private set; }
            */
        }

        public class BillMatchIgnoreModel
        {
            public string BillId { get; set; }
            public string Number { get; set; }
            public bool isCall { get; set; }
        }
    }
}