using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberPark.Domain.Core
{
    public partial class Service
    {
        public class Statuses
        {
            private Statuses()
            {

            }
            //init - prepaid
            public const string Applied = "Applied";
            public const string Prepaid = "Prepaid";
            public const string Cancelled = "Cancelled";
            //prepaid - RFSD
            public const string PrepaymentConfirmed = "Prepayment Confirmed";
            public const string InProcess = "In Process";
            public const string RFSDayGiven = "RFS Day Given";
            public const string RefundApplied = "Refund Applied";
            public const string Rejected = "Rejected";
            public const string Refunded = "Refunded";
            //after RFSD
            public const string InService = "In Service";
            //upgrade
            public const string UpgradeApplied = "Upgrade Applied";
            public const string UpgradeConfirmed = "Upgrade Confirmed";
            public const string UpgradeSubmitted = "Upgrade Submitted";
            public const string Upgraded = "Upgraded";
            //suspend
            public const string SuspensionApplied = "Suspension Applied";
            public const string Suspended = "Suspended";
            //recover
            public const string RecoveryApplied = "Recovery Applied";
            //termination
            public const string TerminationApplied = "Termination Applied";
            public const string TerminationConfirmed = "Termination Confirmed";
            public const string TerminationSubmitted = "Termination Submitted";
            public const string Terminated = "Terminated";

            public static IList<string> GetNextPossibleState(IList<string> roles, string status)
            {
                if (roles.Contains(User.RoleTypes.Administrator))
                {
                    return Status;
                }

                List<string> possible = new List<string> { status };
                switch (status)
                {
                    #region init - prepaid
                    case Applied:
                        if (roles.Contains(User.RoleTypes.Accountant) || 
                            roles.Contains(User.RoleTypes.Customer) || roles.Contains(User.RoleTypes.CustomerService))
                        {
                            possible.Add(Prepaid);
                            possible.Add(Cancelled);
                        }
                        break;
                    case Prepaid:
                        if (roles.Contains(User.RoleTypes.Accountant))
                        {
                            possible.Add(Cancelled);
                            possible.Add(PrepaymentConfirmed);
                        }

                        if (roles.Contains(User.RoleTypes.Customer) || roles.Contains(User.RoleTypes.CustomerService))
                        {
                            possible.Add(Cancelled);
                        }
                        break;
                    #endregion
                    #region prepaid - RFSD
                    case PrepaymentConfirmed:
                        if (roles.Contains(User.RoleTypes.Provision))
                        {
                            possible.Add(InProcess);
                        }

                        if (roles.Contains(User.RoleTypes.Customer) || roles.Contains(User.RoleTypes.CustomerService))
                        {
                            possible.Add(RefundApplied);
                        }
                        break;

                    case InProcess:
                        if (roles.Contains(User.RoleTypes.Provision))
                        {
                            possible.Add(RFSDayGiven);
                            possible.Add(Rejected);
                        }
                        break;
                    case RFSDayGiven:
                        if (roles.Contains(User.RoleTypes.Provision))
                        {
                            possible.Add(InService);
                        }

                        if (roles.Contains(User.RoleTypes.Customer) || roles.Contains(User.RoleTypes.CustomerService))
                        {
                            possible.Add(TerminationApplied);
                        }
                        break;
                    case RefundApplied:
                        if (roles.Contains(User.RoleTypes.Accountant))
                        {
                            possible.Add(Refunded);
                        }
                        break;
                    case Rejected:
                        if (roles.Contains(User.RoleTypes.Accountant))
                        {
                            possible.Add(Refunded);
                        }
                        break;
                    #endregion
                    #region after RFSD
                    case InService:
                        if (roles.Contains(User.RoleTypes.Accountant))
                        {
                            possible.Add(UpgradeConfirmed);
                            possible.Add(SuspensionApplied);
                            possible.Add(TerminationConfirmed);
                        }

                        if (roles.Contains(User.RoleTypes.Customer) || roles.Contains(User.RoleTypes.CustomerService))
                        {
                            possible.Add(UpgradeApplied);
                            possible.Add(TerminationApplied);
                        }
                        break;
                    #endregion
                    #region upgrade
                    case UpgradeApplied:
                        if (roles.Contains(User.RoleTypes.Accountant))
                        {
                            possible.Add(UpgradeConfirmed);
                        }
                        break;
                    case UpgradeConfirmed:
                        if (roles.Contains(User.RoleTypes.Provision))
                        {
                            possible.Add(UpgradeSubmitted);
                        }
                        break;
                    case UpgradeSubmitted:
                        if (roles.Contains(User.RoleTypes.Provision))
                        {
                            possible.Add(Upgraded);
                        }
                        break;
                    #endregion
                    #region suspend / recover
                    case SuspensionApplied:
                        if (roles.Contains(User.RoleTypes.Provision))
                        {
                            possible.Add(Suspended);
                        }
                        break;
                    case RecoveryApplied:
                        if (roles.Contains(User.RoleTypes.Provision))
                        {
                            possible.Add(InService);
                        }
                        break;
                    #endregion
                    #region termination
                    case TerminationApplied:
                        if (roles.Contains(User.RoleTypes.Accountant))
                        {
                            possible.Add(TerminationConfirmed);
                        }
                        break;
                    case TerminationConfirmed:
                        if (roles.Contains(User.RoleTypes.Provision))
                        {
                            possible.Add(TerminationSubmitted);
                        }
                        break;
                    case TerminationSubmitted:
                        if (roles.Contains(User.RoleTypes.Provision))
                        {
                            possible.Add(Terminated);
                        }
                        break;
                    #endregion
                }
                return possible;
            }

            public static IList<string> Status = new List<string>
            {
                Applied,Prepaid,Cancelled,PrepaymentConfirmed,InProcess,RFSDayGiven,RefundApplied,Rejected,
                Refunded,InService,UpgradeApplied,UpgradeConfirmed,UpgradeSubmitted,Upgraded,
                SuspensionApplied,Suspended,RecoveryApplied,TerminationApplied,TerminationConfirmed,TerminationSubmitted,Terminated

            };
        }

    }
}
