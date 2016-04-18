using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyberPark.Website;
using CyberPark.Website.Models;
using AspNet.Identity.MySQL;
using Rhino.Mocks;
using Microsoft.Owin.Security;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using CyberPark.Domain.Core;
namespace Q.xISP.Transfer
{
    public class DataTranfer
    {
        private static xISPDbContext _dbContext;
        private static xISPUserManager _userManager;
        private static xISPRoleManager _roleManager;
        private static xISPSignInManager _signInManager;

        static DataTranfer()
        {
            var IAuthMng = MockRepository.Mock<IAuthenticationManager>();
            _dbContext = xISPDbContext.Create();
            _userManager = new xISPUserManager(new UserStore<xISPUser>(_dbContext));
            _userManager.UserValidator = new UserValidator<xISPUser, int>(_userManager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            _roleManager = new xISPRoleManager(new RoleStore<IdentityRole>(_dbContext));
            _signInManager = new xISPSignInManager(_userManager, IAuthMng);
        }

        public static List<OriCustomer> LoadCustomer()
        {
            //load original customer
            List<tm_customer> customers;
            using (OriEntities db = new OriEntities())
            {
                var query = from c in db.tm_customer
                            join o in db.tm_customer_order
                            on c.id equals o.customer_id
                            //where o.order_status.Equals("using")
                            orderby o.customer_id ascending
                            select c;

                customers = query.Distinct().ToList();
            }

            //load order
            List<OriCustomer> oriCustomers = new List<OriCustomer>();
            foreach (tm_customer c in customers)
            {
                oriCustomers.Add(OriCustomer.Load(c));
                Console.WriteLine(string.Format("{0} - DONE", c.id));
            }

            return oriCustomers;
        }
        public static void InitRole()
        {
            _roleManager.Create(new IdentityRole("Customer"));
            _roleManager.Create(new IdentityRole("Staff"));
        }
        public static void ConvertOperator()
        {
            //load original user
            List<tm_user> oriUsers;
            using (OriEntities db = new OriEntities())
            {
                oriUsers = db.tm_user.ToList();
            }

            using (var db = new NewEntities())
            {
                foreach (var u in oriUsers)
                {
                    //login
                    xISPUser user = new xISPUser
                    {
                        Id = u.id,
                        UserName = u.cellphone ?? u.id.ToString(),
                        Email = u.email,
                        PhoneNumber = u.cellphone
                    };
                    if (string.IsNullOrEmpty(user.Email))
                    {
                        continue;
                    }
                    var rslt = _userManager.CreateAsync(user, u.password).Result;
                    if (!rslt.Succeeded)
                    {
                        Console.WriteLine(string.Format("{0}:{1}", rslt.Errors.FirstOrDefault(), u.id));
                        continue;
                    }

                    //set role
                    _userManager.AddToRole(user.Id, "Staff");

                    //operator
                    staff stf = new staff
                    {
                        Id = u.id,
                        Name = u.user_name
                    };
                    db.staffs.Add(stf);
                }
                db.SaveChanges();
            }
        }
        public static void ConvertTransactionAfter(DateTime date)
        {
            //load original
            List<tm_customer_transaction> oriTrans;
            using (OriEntities db = new OriEntities())
            {
                oriTrans = db.tm_customer_transaction.Where(x => x.transaction_date >= date).ToList();
            }

            //
            using (var db = new xISPContext())
            {
                foreach (var t in oriTrans)
                {
                    if (db.Accounts.Any(a => a.Id == t.order_id))
                    {
                        Transaction newT = new Transaction
                        {
                            Id = Guid.NewGuid().ToString(),
                            AccountId = t.order_id ?? 0,
                            Amount = t.amount ?? 0,
                            Date = t.transaction_date ?? DateTime.MinValue,
                            Type = t.card_name ?? "",
                            CardHolder = t.cardholder_name,
                            CardNumber = t.card_number,
                            DpsTxnRef = t.dps_txn_ref,
                            DpsResponse = t.response_text,
                            TxnMac = t.txnMac,

                            OperatedBy = t.executor ?? 0,
                            OperatedDate = DateTime.Now
                        };

                        //db.transactions.Add(newT);
                        newT.Create(db, 9999);
                    }
                }
                db.SaveChanges();
            }
        }

        #region order
        public static void ConvertCustomerAndOrder(NewEntities db, SortedList<int, string> asids, OriCustomer oriCustomer)
        {
            using (var trans = db.Database.BeginTransaction())
            {
                //login
                tm_customer ori = oriCustomer.Customer;

                //customer
                customer customer = new customer
                {
                    Id = ori.id,
                    FirstName = ori.first_name ?? "",
                    LastName = ori.last_name,
                    Title = ori.title ?? "",
                    Mobile = ori.cellphone,
                    Email = ori.email,
                    IdentityNumber = ori.identity_number,
                    SignUpDate = ori.register_date ?? DateTime.MinValue
                };

                //identity type
                switch (ori.identity_type?.ToLower() ?? "")
                {
                    case "driver licence":
                        customer.IdentityType = Customer.IdentityTypes.DriverLience;
                        break;
                    case "passport":
                        customer.IdentityType = Customer.IdentityTypes.Passport;
                        break;
                    default:
                        customer.IdentityType = ori.identity_type;
                        break;
                }

                db.customers.Add(customer);
                Console.WriteLine(string.Format("customer {0}", customer.Id));
                db.SaveChanges();

                xISPUser user = new xISPUser
                {
                    Id = customer.Id,
                    UserName = ori.id.ToString(),
                    Email = ori.email,
                    PhoneNumber = ori.cellphone
                };
                var rslt = _userManager.CreateAsync(user, string.IsNullOrEmpty(ori.password) ? "password" : ori.password).Result;
                if (!rslt.Succeeded)
                {
                    Console.WriteLine(string.Format("{0}:{1}", rslt.Errors.FirstOrDefault(), ori.id));
                    return;
                }
                Console.WriteLine(string.Format("user {0}", user.Id));

                //set role
                _userManager.AddToRole(user.Id, "Customer");

                //order=>address / servicePackage / service
                foreach (OriOrder oriO in oriCustomer.Orders)
                {
                    #region order->account
                    if(!oriO.Order.order_status.Equals("using") &&
                        !oriO.Order.order_status.Equals("rfs") &&
                        !(oriO.Order.order_status.Equals("disconnected") && oriO.Order.disconnected_date != null 
                            && oriO.Order.disconnected_date > DateTime.Today.AddMonths(-1)))
                    {
                        continue;
                    }

                    //Address
                    account acc = new account
                    {
                        Id = oriO.Order.id,
                        CustomerId = oriCustomer.Customer.id,
                        Type = oriO.Order.customer_type.ToUpper().Equals("BUSINESS") ? Account.Types.Business
                                            : (oriO.Order.customer_type.ToUpper().Equals("PERSONAL") ? Account.Types.Personal : "Unknown"),
                        Address = oriO.Order.address,
                        IsActive = true,
                        RegisterBranchId = "b0b1ee67-dbcb-11e5-b23c-d4bed972f15a",
                        ChargeBranchId = "b0b1ee67-dbcb-11e5-b23c-d4bed972f15a",

                        FirstName = oriO.Order.first_name,
                        LastName = oriO.Order.last_name,
                        Title = oriO.Order.title,
                        Mobile = oriO.Order.mobile,
                        Email = oriO.Order.email,

                        NextInvoiceIssueDate = oriO.Order.next_invoice_create_date

                    };

                    //diff identity
                    if (acc.Type.Equals(Account.Types.Business))
                    {
                        acc.OrganizationName = oriO.Order.org_name;
                        acc.IdentityType = Customer.IdentityTypes.OrganizationRegNo;
                        acc.IdentityNumber = oriO.Order.org_register_no;
                        acc.InvoicePeriodType = Account.InvoicePeriodTypes.CalendarMonth;
                    }
                    else
                    {
                        acc.IdentityType = customer.IdentityType;
                        acc.IdentityNumber = customer.IdentityNumber;
                        if ("old".Equals(oriO.Order.order_serial))
                        {
                            acc.InvoicePeriodType = Account.InvoicePeriodTypes.CalendarMonth;
                        }
                        else
                        {
                            acc.InvoicePeriodType = Account.InvoicePeriodTypes.ServiceGivenDay;
                        }
                    }

                    customer.accounts.Add(acc);
                    Console.WriteLine(string.Format("account {0}", acc.Id));
                    db.SaveChanges();

                    //order detail=>product / service
                    //main product
                    product mainPdct = new product
                    {
                        Id = Guid.NewGuid().ToString(),
                        AccountId = acc.Id,
                        DiscountRate = 0.0D,
                        AppliedDate = oriO.Order.order_create_date==null? oriO.Order.order_create_date .Value: DateTime.Today
                    };

                    string productStatus = null;
                    switch (oriO.Order.order_status)
                    {
                        case "using": productStatus = Service.Statuses.InService; break;
                        case "rfs": productStatus = Service.Statuses.RFSDayGiven; break;
                        case "disconnected": productStatus = Service.Statuses.Terminated; break;
                        default: break;
                    }

                    foreach (tm_customer_order_detail oriOdetail in oriO.OrderDetails.OrderByDescending(o => o.detail_price))
                    {
                        if ((oriOdetail.detail_expired ?? DateTime.MaxValue) < DateTime.Now
                            //|| oriOdetail.detail_type.ToLower().Equals("12 month")
                            //|| oriOdetail.detail_type.ToLower().Equals("hardware")
                            //|| oriOdetail.detail_type.ToLower().Equals("hardware-router")
                            || oriOdetail.detail_type.ToLower().Equals("debit")
                            || oriOdetail.detail_type.ToLower().Equals("jackpot")
                            || oriOdetail.detail_type.ToLower().Equals("discount")
                            || oriOdetail.detail_type.ToLower().Equals("transition")
                            || oriOdetail.detail_type.ToLower().Equals("new-connection"))
                        {
                            continue;
                        }

                        //
                        service srvTmp = OrderDetailToService(asids,oriOdetail, oriO.Order);
                        srvTmp.Status = productStatus;

                        if (oriOdetail.detail_type.ToLower().Equals("plan-term") ||
                            oriOdetail.detail_type.ToLower().Equals("plan-no-term") ||
                            oriOdetail.detail_type.ToLower().Equals("12 month"))
                        {
                            //broadband=>default package
                            mainPdct.Name = oriOdetail.detail_name;
                            mainPdct.BasePriceGSTExclusive = (oriOdetail.detail_price ?? 0) / (acc.Type.Equals(Account.Types.Personal) ? 1.15D : 1D);
                            mainPdct.ServiceGivenDate = oriO.Order.order_using_start;
                            if (Service.Statuses.InService.Equals(productStatus))
                            {
                                DateTime chargeDt = oriO.Order.next_invoice_create_date.Value.AddMonths(-1);
                                if (acc.InvoicePeriodType.Equals(Account.InvoicePeriodTypes.CalendarMonth))
                                {
                                    //last month end
                                    mainPdct.ChargedToDate = new DateTime(chargeDt.Year, chargeDt.Month, DateTime.DaysInMonth(chargeDt.Year, chargeDt.Month));
                                }
                                else
                                {
                                    //start using day - 1
                                    var day = oriO.Order.order_using_start.Value.Day;
                                    int dayDiff = -1;
                                    if (DateTime.DaysInMonth(chargeDt.Year, chargeDt.Month) < day)
                                    {
                                        day = DateTime.DaysInMonth(chargeDt.Year, chargeDt.Month);
                                        dayDiff = 0;
                                    }
                                    mainPdct.ChargedToDate = new DateTime(chargeDt.Year, chargeDt.Month, day).AddDays(dayDiff);
                                }
                            }
                          

                            mainPdct.NumberOfMonthPerCharge = 1;
                            mainPdct.IsOneOff = false;
                            //save main package
                            db.products.Add(mainPdct);
                            db.SaveChanges();

                            if (oriOdetail.detail_type.ToLower().Equals("plan-term"))
                            {
                                //terms
                                //service_package_terms term = OrderDetialToTerms(oriOdetail, oriO.Order);
                                //term.Months = oriOdetail.detail_term_period ?? 0;
                                //if (oriO.Order.order_using_start?.AddMonths(term.Months) > DateTime.Now)
                                //{
                                //    term.StartDate = oriO.Order.order_using_start ?? DateTime.MinValue;
                                //    term.EndDate = term.StartDate?.AddMonths(term.Months).AddDays(-1);
                                //}
                                mainPdct.TermStartDate = oriO.Order.order_using_start;
                                mainPdct.MonthsOfTerm = oriOdetail.detail_term_period;
                                mainPdct.IsTermed = true;
                                //mainPkg.service_package_terms.Add(term);
                                //db.service_package_terms.Add(term);
                                db.SaveChanges();
                            }

                            mainPdct.services.Add(srvTmp);
                            db.SaveChanges();
                        }
                        else if (oriOdetail.detail_type.ToLower().Equals("pstn") ||
                         oriOdetail.detail_type.ToLower().Equals("voip"))
                        {
                            if (oriOdetail.detail_price == 0)
                            {
                                //inclue in main package
                                mainPdct.services.Add(srvTmp);
                            }
                            else
                            {
                                //new package
                                product pdctTmp = new product
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    AccountId = acc.Id,
                                    BasePriceGSTExclusive = (oriOdetail.detail_price ?? 0) / (acc.Type.Equals(Account.Types.Personal) ? 1.15D : 1D),
                                    ServiceGivenDate = mainPdct.ServiceGivenDate,
                                    ChargedToDate = mainPdct.ChargedToDate,
                                    NumberOfMonthPerCharge = 1,
                                    IsOneOff = false,
                                    Name = srvTmp.SubType
                                };
                                db.products.Add(pdctTmp);
                                pdctTmp.services.Add(srvTmp);
                                db.SaveChanges();

                            }
                        }
                        else if (oriOdetail.detail_type.ToLower().Equals("super-free-calling") ||
                           oriOdetail.detail_type.ToLower().Equals("present-calling-minutes"))
                        {
                            service_usage_offers offerTmp = OrderDetailToUsageOffer(oriOdetail, oriO.Order);
                            if (oriOdetail.detail_price == null || oriOdetail.detail_price <= 0)
                            {
                                //inclue in main package
                                mainPdct.service_usage_offers.Add(offerTmp);
                            }
                            else
                            {
                                //new package
                                product pdctTmp = new product
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    AccountId = acc.Id,
                                    BasePriceGSTExclusive = (oriOdetail.detail_price ?? 0) / (acc.Type.Equals(Account.Types.Personal) ? 1.15D : 1D),
                                    ServiceGivenDate = mainPdct.ServiceGivenDate,
                                    ChargedToDate = mainPdct.ChargedToDate,
                                    NumberOfMonthPerCharge = 1,
                                    IsOneOff = false,
                                    Name = offerTmp.Name
                                };
                                db.products.Add(pdctTmp);
                                pdctTmp.service_usage_offers.Add(offerTmp);
                                db.SaveChanges();
                            }
                        }
                        else if (oriOdetail.detail_type.ToLower().Equals("hardware") ||
                           oriOdetail.detail_type.ToLower().Equals("hardware-router"))
                        {
                            continue;
                        }
                    }
                    #endregion
                }
                if (customer.accounts.Count == 0)
                {
                    trans.Rollback();
                    return;
                }

                db.SaveChanges();
                trans.Commit();
            }
        }

        private static service OrderDetailToService(SortedList<int, string> asids, tm_customer_order_detail od, tm_customer_order or)
        {
            service srv = new service
            {
                Id = Guid.NewGuid().ToString(),
                Status = Service.Statuses.InService
            };
            string pstnNumber = od.pstn_number?.Trim().Replace(" ", "").Replace("-", "");
            if (pstnNumber != null)
            {
                if (pstnNumber.StartsWith("0") && !pstnNumber.StartsWith("0800"))
                {
                    //remove first Zero;
                    //except 0800
                    pstnNumber = pstnNumber.Remove(0, 1);
                }
                if (pstnNumber.StartsWith("64") && pstnNumber.Length > 8)
                {
                    //remove 64 prefix
                    pstnNumber = pstnNumber.Remove(0, 2);
                }
            }
            switch (od.detail_type.ToLower())
            {
                case "plan-term":
                case "plan-no-term":
                case "12 month":
                    //
                    srv.Type = Service.Types.BroadBand;
                    srv.SubType = od.detail_plan_type;
                    srv.IdentityNumber = asids.ContainsKey(or.id)?asids[or.id]: null;
                    srv.BroadbandCVLAN = or.cvlan;
                    srv.BroadbandSVLAN = or.svlan;
                    srv.BroadbandPPPoeLoginName = or.pppoe_loginname;
                    srv.BroadbandPPPoePassword = or.pppoe_password;
                    srv.ReadyForServiceDate = or.rfs_date;
                    break;
                case "pstn":
                    //pstn
                    srv.Type = Service.Types.Phone;
                    srv.SubType = Service.PhoneSubTypes.PSTN;
                    srv.IdentityNumber = pstnNumber;
                    srv.ReadyForServiceDate = or.rfs_date;
                    break;
                case "voip":
                    //voip
                    srv.Type = Service.Types.Phone;
                    srv.SubType = Service.PhoneSubTypes.VoIP;
                    srv.IdentityNumber = od.pstn_number;
                    srv.VoipAssignedDate = od.voip_assign_date;
                    srv.VoipPassword = od.voip_password;
                    srv.ReadyForServiceDate = or.rfs_date;
                    break;
                default:
                    srv.Type = od.detail_type.Length > 20 ? od.detail_type.Substring(0, 20) : od.detail_type;
                    break;
            }
            return srv;
        }

        private static service_usage_offers OrderDetailToUsageOffer(tm_customer_order_detail od, tm_customer_order or)
        {
            service_usage_offers offer = new service_usage_offers
            {
                Id = Guid.NewGuid().ToString(),
                ServiceType = Service.Types.Phone,
                Minutes = od.detail_calling_minute ?? 0,
                Local = false,
                National = false,
                Mobile = false,
                Name = od.detail_name
            };

            if (offer.Minutes == 99999)
            {
                offer.Minutes = -1;
            }

            if (od.detail_desc.ToLower().Contains("voip"))
            {
                offer.ServiceSubType = Service.PhoneSubTypes.VoIP;
            }
            else
            {
                offer.ServiceSubType = Service.PhoneSubTypes.PSTN;
            }

            string desc = od.detail_desc.ToLower();
            if (desc.Contains("super-mobile"))
            {
                offer.Mobile = true;
            }

            if (desc.Contains("super-local"))
            {
                offer.Local = true;
            }

            if (desc.Equals("national") || desc.Contains("super-national"))
            {
                offer.National = true;
            }

            if (desc.ToLower().Equals("international") || desc.Contains("40"))
            {
                offer.CallingRegionId = "40COUNTRIES";
            }

            if (desc.Contains("china"))
            {
                offer.CallingRegionId = "CHN_FIX_MOB";
            }

            if (desc.Contains("india"))
            {
                offer.CallingRegionId = "IND_FIX_MOB";
            }



            return offer;
        }
        #endregion

        public static void ConvertPlan()
        {
            List<tm_plan> oriPlans;
            using (var db = new OriEntities())
            {
                oriPlans = db.tm_plan.Where(x => x.plan_status.Equals("selling")).ToList();
            }

            using (var db = new NewEntities())
            {
                foreach (var ori in oriPlans)
                {
                    var newPlan = new plan
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = ori.plan_name,
                        MonthlyPrice = ori.plan_price ?? 0D,
                        MonthsOfContract = ori.term_period ?? 0,
                        BroadbandType = ori.plan_type.ToUpper(),
                        PstnCount = ori.pstn_count ?? 0,
                        VoipCount = ori.voip_count ?? 0,
                        IsPromotion = ori.promotion ?? false,
                        DisplayPriority = ori.place_sort ?? 0,
                        Memo = ori.memo,
                        IsActive = true
                    };
                    db.plans.Add(newPlan);
                }
                db.SaveChanges();
            }
        }
    }
}