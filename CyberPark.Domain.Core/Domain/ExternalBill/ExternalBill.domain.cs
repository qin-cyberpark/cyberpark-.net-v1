using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace CyberPark.Domain.Core
{
    public partial class ExternalBill:IComparable   
    {
        private BaseExternalBillParser _parser;

        private ExternalBill(string header) : this()
        {
            Id = Guid.NewGuid().ToString();
            _parser = new VOSBillParser(header);
            while (_parser != null && !_parser.isValid())
            {
                _parser = _parser.NextParser();
            }
        }

        public int CompareTo(object obj)
        {
            var bill = obj as ExternalBill;
            return Id.CompareTo(bill?.Id);
        }

        #region import
        [NotMapped]
        public bool IsValid
        {
            get
            {
                return _parser != null;
            }
        }

        /// <summary>
        /// Parse Calling Record
        /// </summary>
        /// <param name="line"></param>
        private void AddCallingRecord(string line)
        {
            if (_parser == null)
            {
                return;
            }
            CallingRecord cr = _parser.ParseCallingRecord(FormatLine(line));
            if (cr != null && cr.Cost > 0)
            {
                if (cr.ChargeMinute == 0)
                {
                    cr.Warning += "duration is 0;";
                }
                if (cr.RatePerMinute < 0)
                {
                    cr.Warning += "no matched rate;";
                }

                cr.FileId = Id;
                CallingRecords.Add(cr);
            }
        }

        /// <summary>
        /// Add Addon Service Record
        /// </summary>
        /// <param name="line"></param>
        private void AddAddonServiceRecord(string line)
        {
            if (_parser == null)
            {
                return;
            }
            line = FormatLine(line);
            AddonCharge asr = _parser.ParseAddonRecord(line);
            if (asr != null && asr.Cost > 0)
            {
                asr.FileId = Id;
                AddonCharges.Add(asr);
            }
        }

        /// <summary>
        /// Save External object
        /// </summary>
        /// <param name="staffId"></param>
        /// <returns></returns>
        public bool Save(int staffId)
        {
            try
            {
                if (HasUploaded(Source, Year, Month))
                {
                    throw new ExternalBillException(string.Format("{0} {1}/{2} data has updated", Source, Year, Month));
                }

                //int unmatched = 0;
                using (xISPContext db = new xISPContext())
                {
                    using (var trans = db.Database.BeginTransaction())
                    {
                        //insert file
                        OperatedBy = staffId;
                        OperatedDate = DateTime.Now;

                        //insert record
                        foreach (AddonCharge asr in AddonCharges)
                        {
                            asr.MatchAccount(db);
                            AddonCharges.Add(asr);
                            //unmatched += asr.MatchAccount(db) ? 0 : 1;
                            //AddonCharges.Add(asr);
                        }

                        foreach (CallingRecord cr in CallingRecords)
                        {
                            cr.MatchAccount(db);
                            CallingRecords.Add(cr);
                            //unmatched += cr.MatchAccount(db) ? 0 : 1;
                            //CallingRecords.Add(cr);
                        }

                        db.ExternalBills.Add(this);
                        db.SaveChanges();
                        trans.Commit();
                    }
                }

                //if (unmatched > 0)
                //{
                //    Warning.WriteAsync(WarningModules.ExternalBill, "Import",
                //        string.Format("file {0} has {1} unmatched records", FileName, unmatched), userId: staffId);
                //}
            }
            catch (ExternalBillException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new ExternalBillException("Fail to save external bill", ex);
            }

            return true;
        }

        /// <summary>
        /// Format data line
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static string FormatLine(string line)
        {
            StringBuilder sb = new StringBuilder();
            int start = line.IndexOf("\"");
            int end = 0;
            while (start > 0)
            {
                end = line.IndexOf("\"", start + 1);
                if (end > 0)
                {
                    sb.Append(line.Substring(0, start));
                    sb.Append(line.Substring(start + 1, end - start - 1).Replace(",", " "));
                    sb.Append(line.Substring(end + 1));
                }
                else
                {
                    sb.Append(line.Substring(0, start));
                    sb.Append(line.Substring(start + 1));
                }
                line = sb.ToString();
                start = line.IndexOf("\"");
            }
            return line;
        }

        public static bool HasUploaded(string src, int year, int month)
        {
            
            using (var db = new xISPContext())
            {
                var bill = db.ExternalBills.Where(x => x.Source.Equals(src) && x.Year == year && x.Month == month).FirstOrDefault();
                return bill != null;
            }
        }

        /// <summary>
        /// Parse file to ExternalBill object
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static ExternalBill Parse(string folder, string fileName)
        {
            SortedList<string, List<CallingRecord>> tmpCallRecords = new SortedList<string, List<CallingRecord>>();
            SortedList<string, List<AddonCharge>> tmpSrvRecords = new SortedList<string, List<AddonCharge>>();
            string filePath = folder + fileName;
            string line = "";
            string header = "";
            bool firstLine = true;
            ExternalBill bill = null;

            using (var sr = new StreamReader(filePath, Encoding.GetEncoding("GBK")))
            {
                //try
                //{
                line = sr.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    if (firstLine)
                    {
                        header = line;
                        bill = new ExternalBill(header)
                        {
                            FileName = fileName
                        };
                        if (!bill.IsValid)
                        {
                            break;
                        }
                        else {
                            firstLine = false;
                        }
                    }
                    else {
                        bill.AddCallingRecord(line);
                        bill.AddAddonServiceRecord(line);
                    }
                    line = sr.ReadLine();
                }
                //}
                //catch (Exception e)
                //{
                //    throw e;
                //}
            }
            //            
           


            //set properties
            bill.Source = bill?._parser.BillSource ?? ExternalBillSources.Unknown;
            bill.CallOriginalNumberCount = bill.CallingRecords.GroupBy(x => x.OriNumber).Count();
            bill.CallRecordCount = bill.CallingRecords.Count;
            bill.CallTotalCost = bill.CallingRecords.Count > 0 ? bill.CallingRecords.Sum(x => x.Cost) : 0D;
            bill.CallDateFrom = bill.CallingRecords.Count > 0 ? bill.CallingRecords.Min(x => x.CallStart) : DateTime.MinValue;
            bill.CallDateTo = bill.CallingRecords.Count > 0 ? bill.CallingRecords.Max(x => x.CallStart) : DateTime.MinValue;
            bill.AddonOriginalNumberCount = bill.AddonCharges.GroupBy(x => x.OriNumber).Count();
            bill.AddonRecordCount = bill.AddonCharges.Count;
            bill.AddonTotalCost = bill.AddonCharges.Count > 0 ? bill.AddonCharges.Sum(x => x.Cost) : 0D;
            bill.AddonDateFrom = bill.AddonCharges.Count > 0 ? bill.AddonCharges.Where(x => x.DateFrom != DateTime.MinValue).Min(x => x.DateFrom) : DateTime.MinValue;
            bill.AddonDateTo = bill.AddonCharges.Count > 0 ? bill.AddonCharges.Max(x => x.DateTo) : DateTime.MinValue;
            bill.Size = new FileInfo(filePath).Length;


            //year / month
            DateTime dtFrom = bill.CallDateFrom ?? bill.AddonDateFrom.Value;
            DateTime dtTo = bill.CallDateTo ?? bill.AddonDateTo.Value;
            if ((dtFrom.Year == dtTo.Year && dtFrom.Month == dtTo.Month) || dtFrom.Day <= 15)
            {
                bill.Year = dtFrom.Year;
                bill.Month = dtFrom.Month;
            }
            else
            {
                bill.Year = dtTo.Year;
                bill.Month = dtTo.Month;
            }

            if (!bill.IsValid)
            {
                return null;
            }
            else {
                return bill;
            }
        }

        /// <summary>
        /// match account for call/service record 
        /// </summary>
        /// <param name="billId"></param>
        /// <param name="oriNumber"></param>
        /// <param name="isCall"></param>
        /// <returns>account id</returns>
        public static int MatchAccount(xISPContext db, string fileId, string oriNumber, bool isCall) {
            try
            {
                var srv = db.Services.Include(x => x.Product)
                                    .SingleOrDefault(s => s.IdentityNumber.Equals(oriNumber)
                                                         || oriNumber.Equals("0" + s.IdentityNumber)
                                                         || oriNumber.Equals("64" + s.IdentityNumber));
                if (srv == null)
                {
                    //unmatched
                    return 0;
                }

                //matched and update
                if (isCall)
                {
                    db.CallingRecords.Where(x => x.FileId.Equals(fileId) && x.OriNumber.Equals(oriNumber)
                                            && x.ServiceId == null && !x.Ignored).ToList()
                                            .ForEach(x => { x.ServiceId = srv.Id;x.PhoneType = srv.SubType; });
                }
                else
                {
                    db.AddonCharges.Where(x => x.FileId.Equals(fileId) && x.OriNumber.Equals(oriNumber)
                                           && x.ServiceId == null && !x.Ignored).ToList()
                                           .ForEach(x => { x.ServiceId = srv.Id; x.AccountId = srv.Product.AccountId; });
                }
                db.SaveChanges();
                return srv.Product.AccountId;

            }
            catch
            {
                return 0;
            }
        }

        public static bool Ignore(xISPContext db, string fileId, string oriNumber, bool isCall, int userId)
        {
            try
            {
                //ignore
                if (isCall)
                {
                    db.CallingRecords.Where(x => x.FileId.Equals(fileId) && x.OriNumber.Equals(oriNumber)
                                            && x.ServiceId == null && !x.Ignored).ToList()
                                            .ForEach(x => { x.Ignored = true; x.IgnoredBy = userId; });
                }
                else
                {
                    db.AddonCharges.Where(x => x.FileId.Equals(fileId) && x.OriNumber.Equals(oriNumber)
                                           && x.ServiceId == null && !x.Ignored).ToList()
                                           .ForEach(x => { x.Ignored = true; x.IgnoredBy = userId; });
                }
                db.SaveChanges();
                return true;

            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region operate
        public static IList<ExternalBill> Get()
        {
            using (xISPContext db = new xISPContext())
            {
                return db.ExternalBills.ToList();
            }
        }

        public static ExternalBill Get(string id)
        {
            using (xISPContext db = new xISPContext())
            {
                return db.ExternalBills.Include(x => x.CallingRecords)
                                .Include(x => x.AddonCharges)
                                .SingleOrDefault(x => x.Id.Equals(id));
            }

        }

        private static bool HasExternalBillImported(int year, int month, string src)
        {
            using (var db = new xISPContext())
            {
                //check data source
                var bill = db.ExternalBills.Where(x => x.Source.Equals(src) && x.Year == year && x.Month == month).FirstOrDefault();
                if (bill == null)
                {
                    return false;
                }

                var unmatchedCount = db.CallingRecords.Where(x => x.OriginalBill.Source.Equals(src) && x.ServiceId == null && !x.Ignored).Count();
                if (unmatchedCount > 0)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool HasExternalBillImported(int year, int month)
        {
            using (var db = new xISPContext())
            {
                //check data source
                if (!HasExternalBillImported(year, month, ExternalBillSources.CallPlus))
                {
                    return false;
                }
                if (!HasExternalBillImported(year, month, ExternalBillSources.Chorus))
                {
                    return false;
                }
                if (!HasExternalBillImported(year, month, ExternalBillSources.VOS))
                {
                    return false;
                }

                return true;
            }
        }

        public static bool HasExternalBillReadyForInvoice(DateTime issueDate)
        {
            //external bill year/month
            var billYearMonth = new DateTime(issueDate.Year, issueDate.Month, 1).AddMonths(-1);
            var billImportedDate = billYearMonth.AddMonths(1).AddDays(SysConfig.Instance.ExternalBillImportDay);
            if (issueDate < billImportedDate.Date)
            {
                //before external bill imported
                billYearMonth = billYearMonth.AddMonths(-1);
            }

            return ExternalBill.HasExternalBillImported(billYearMonth.Year, billYearMonth.Month);
        }

        #endregion

        #region statistics
        public static SortedList<ExternalBill, int> CountUnmatchedCall(xISPContext db)
        {
            var result = new SortedList<ExternalBill, int>();
            db.CallingRecords.Include(x => x.OriginalBill)
                   .Where(x => x.ServiceId == null && !x.Ignored)
                   .Select(x => new { x.OriginalBill, x.OriNumber }).Distinct()
                   .GroupBy(g => g.OriginalBill).Select(g => new { Bill = g.Key, Count = g.Count() })
                   .Where(g=>g.Count> 0).ToList()
                   .ForEach(x=>result.Add(x.Bill,x.Count));

            return result;
        }
        public static SortedList<ExternalBill, int> CountUnmatchedService(xISPContext db)
        {
            var result = new SortedList<ExternalBill, int>();
            db.AddonCharges.Include(x => x.OriginalBill)
                   .Where(x => x.ServiceId == null && !x.Ignored)
                   .Select(x => new { x.OriginalBill, x.OriNumber }).Distinct()
                   .GroupBy(g => g.OriginalBill).Select(g => new { Bill = g.Key, Count = g.Count() }).ToList()
                   .Where(g => g.Count > 0).ToList()
                   .ForEach(x => result.Add(x.Bill, x.Count));

            return result;
        }


        #endregion
    }
}
