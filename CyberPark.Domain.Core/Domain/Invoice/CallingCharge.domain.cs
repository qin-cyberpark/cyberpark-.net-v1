using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace CyberPark.Domain.Core
{
    public partial class CallingCharge
    {
        private void Add(CallingRecord rec)
        {
            CallingRecords.Add(rec);
            Cost += rec.Charge;
            Charge += rec.ActualCharge ?? 0;
        }

        public static bool CalcCallingCharge(xISPContext db, int accountId,  ref string msg)
        {
            try
            {

                #region load data
                //load account
                var account = db.Accounts.SingleOrDefault(x => x.Id == accountId);
                if (account == null)
                {
                    msg = string.Format("Account {0} is not existing", accountId);
                    return false;
                }

                //load calling offer
                var offers = db.ServiceUsageOffers.Include(x => x.CallingRegion.Details)
                                                    .Where(x => x.Product.Status.Equals(Service.Statuses.InService)
                                                        && x.Product.AccountId == accountId).AsNoTracking().ToList();
                //add calling record
                //var callRecs = db.CallingRecords.Include(x => x.Service)
                //                                .Where(x => x.Service.ServicePackage.AccountId == accountId
                //                                      && x.ChargeRecordId == null && !x.Ignored
                //                                      && x.CallStart >= dtFrom && x.CallStart <= dtTo).OrderBy(x => x.ServiceId).ToList();

                //load phone services
                var callSrvs = db.Services.Where(x => x.Product.AccountId == accountId && x.Type.Equals(Service.Types.Phone)
                                                 && x.Status.Equals(Service.Statuses.InService)).ToList();

                #endregion

                foreach (var srv in callSrvs)
                {
                    //load records
                    db.Entry(srv).Collection(x => x.CallingRecords).Query()
                                .Where(x => x.ChargeRecordId == null && !x.Ignored)
                                .OrderByDescending(x => x.CallStart).Load();
                    if(srv.CallingRecords.Count == 0)
                    {
                        continue;
                    }

                    var dtFrom = srv.CallingRecords.Min(x => x.CallStart);
                    var dtTo = srv.CallingRecords.Max(x => x.CallStart);

                    var callChrg = new CallingCharge(accountId, srv.Id, dtFrom, dtTo);
                    foreach (var call in srv.CallingRecords)
                    {
                        //global free
                        if (account.Type.Equals(Account.Types.Personal)
                           && ((call.PhoneType.Equals(Service.PhoneSubTypes.VoIP)
                               && (call.Type.Equals(CallingRecord.Types.Local) || call.Type.Equals(CallingRecord.Types.National)))
                           || (call.PhoneType.Equals(Service.PhoneSubTypes.PSTN)
                               && call.Type.Equals(CallingRecord.Types.Local))))
                        {
                            //person free: VoIP-Local, National; PSTN - Local
                            call.ActualChargeMiute = 0;
                            call.ActualCharge = 0;
                            call.Memo = string.Format("{0} free to {1}-{2}", account.Type, call.PhoneType, call.Type);
                            call.Ignored = true;
                            call.IgnoredBy = SysConfig.Instance.AutoOperatorId;
                            continue;
                        }
                        else if (account.Type.Equals(Account.Types.Business)
                                 && call.PhoneType.Equals(Service.PhoneSubTypes.VoIP)
                                 && call.Type.Equals(CallingRecord.Types.Local))
                        {
                            //business free: VoIP-Local
                            call.ActualChargeMiute = 0;
                            call.ActualCharge = 0;
                            call.Memo = string.Format("{0} free to {1}-{2}", account.Type, call.PhoneType, call.Type);
                            call.Ignored = true;
                            call.IgnoredBy = SysConfig.Instance.AutoOperatorId;
                            continue;
                        }

                        #region match offer
                        bool offerApplied = false;
                        foreach (var ofr in offers)
                        {
                            if (!ofr.Unlimited && ofr.Minutes == 0)
                            {
                                continue;
                            }

                            if (ofr.IsApplicable(call))
                            {
                                if (ofr.Unlimited)
                                {
                                    call.ActualChargeMiute = 0;
                                    call.ActualCharge = 0;
                                }
                                else if (ofr.Minutes > call.ChargeMinute)
                                {
                                    ofr.Minutes -= call.ChargeMinute;
                                    call.ActualChargeMiute = 0;
                                    call.ActualCharge = 0;
                                }
                                else
                                {
                                    ofr.Minutes = 0;
                                    call.ActualChargeMiute = call.ChargeMinute - ofr.Minutes;
                                    call.ActualCharge = call.RatePerMinute * call.ActualChargeMiute;
                                }
                                offerApplied = true;
                                call.OfferId = ofr.Id;
                                break;
                            }
                        }

                        if (!offerApplied)
                        {
                            call.ActualChargeMiute = call.ChargeMinute;
                            call.ActualCharge = call.RatePerMinute * call.ActualChargeMiute;
                        }
                        #endregion

                        callChrg.Add(call);
                    }
                    db.CallingCharges.Add(callChrg);
                }

                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return false;
            }
        }
    }
}
