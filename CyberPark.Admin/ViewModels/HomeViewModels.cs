using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CyberPark.Domain.Core;
using CyberPark.Website.Models;

namespace CyberPark.Website.ViewModels
{
    public class HomeViewModels
    {
        public class AccountCountViewModel
        {
            private IList<Account> _accounts;
            public AccountCountViewModel(IList<Account> accounts)
            {
                _accounts = accounts;
            }

            public int Count(string status) {
                return _accounts.Where(x => x.Products.Any(p=>p.Status.Equals(status))).Count();
            }

            #region status
            public int Applied { get { return Count(Service.Statuses.Applied); } }
            public int Prepaid { get { return Count(Service.Statuses.Prepaid); } }
            public int Cancelled { get { return Count(Service.Statuses.Cancelled); } }
            public int PrepaymentConfirmed { get { return Count(Service.Statuses.PrepaymentConfirmed); } }
            public int InProcess { get { return Count(Service.Statuses.InProcess); } }
            public int RFSDayGiven { get { return Count(Service.Statuses.RFSDayGiven); } }
            public int RefundApplied { get { return Count(Service.Statuses.RefundApplied); } }
            public int Rejected { get { return Count(Service.Statuses.Rejected); } }
            public int Refunded { get { return Count(Service.Statuses.Refunded); } }
            public int InService { get { return Count(Service.Statuses.InService); } }
            public int UpgradeApplied { get { return Count(Service.Statuses.UpgradeApplied); } }
            public int UpgradeConfirmed { get { return Count(Service.Statuses.UpgradeConfirmed); } }
            public int UpgradeSubmitted { get { return Count(Service.Statuses.UpgradeSubmitted); } }
            public int Upgraded { get { return Count(Service.Statuses.Upgraded); } }
            public int SuspensionApplied { get { return Count(Service.Statuses.SuspensionApplied); } }
            public int Suspended { get { return Count(Service.Statuses.Suspended); } }
            public int RecoveryApplied { get { return Count(Service.Statuses.RecoveryApplied); } }
            public int TerminationApplied { get { return Count(Service.Statuses.TerminationApplied); } }
            public int TerminationConfirmed { get { return Count(Service.Statuses.TerminationConfirmed); } }
            public int TerminationSubmitted { get { return Count(Service.Statuses.TerminationSubmitted); } }
            public int Terminated { get { return Count(Service.Statuses.Terminated); } }
            #endregion
        }

        public class ExternalBillCountViewModel
        {
            public ExternalBillCountViewModel(SortedList<ExternalBill, int> calls, SortedList<ExternalBill, int> services)
            {
                UnmatchedCalls = calls;
                UnmatchedServices = services;
            }
            public SortedList<ExternalBill, int> UnmatchedCalls { get; private set; }
            public SortedList<ExternalBill, int> UnmatchedServices { get; private set; }
        }

        public class InfoCountViewModel
        {
            public InfoCountViewModel(int warning)
            {
                Warnings = warning;
            }
            public int Warnings { get; }
        }

        public class AccountantCountViewModel
        {
            public AccountantCountViewModel(int undeliveredInv)
            {
                UndeliveredInvoice = undeliveredInv;
            }
            public int UndeliveredInvoice { get; }
        }
    }
}