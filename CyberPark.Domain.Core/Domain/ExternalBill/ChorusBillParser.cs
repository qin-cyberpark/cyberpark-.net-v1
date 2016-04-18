using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberPark.Domain.Core
{
    internal class ChorusBillParser: BaseExternalBillParser
    {
        public ChorusBillParser(string header) : base(header)
        {
            // TODO Auto-generated constructor stub
            BillSource = ExternalBillSources.Chorus;
            fields = new SortedList<string, int>();
            fields.Add("StatementDate", -1);
            fields.Add("RecordType", -1);
            fields.Add("ClearServiceID", -1);
            fields.Add("DateFrom", -1);
            fields.Add("DateTo", -1);
            fields.Add("ChargeDate", -1);
            fields.Add("ChargeTime", -1);
            fields.Add("Duration", -1);
            fields.Add("OOTID", -1);
            fields.Add("BillingDescription", -1);
            fields.Add("AmountExcl", -1);
            fields.Add("AmountIncl", -1);
            fields.Add("CallTypeCode", -1);
            fields.Add("Juristiction", -1);
            fields.Add("PhoneCalled", -1);
            fields.Add("ProductAmountChangeFlag", -1);
        }

        public override CallingRecord ParseCallingRecord(string line)
        {
            CallingRecord cr = new CallingRecord()
            {
                Id = Guid.NewGuid().ToString()
            };
            string[] arr = line.Split(',');
            int arrLen = arr.Length;
            string tmp = null;
            //no charge
            int idx = fields["ProductAmountChangeFlag"];
            if (arrLen > idx && arr[idx] != null)
            {
                tmp = arr[idx].Trim();
                if (tmp.Equals("No Change"))
                {
                    return null;
                }
            }

            // ori_number
            idx = fields["RecordType"];
            if (arrLen > idx)
            {
                tmp = arr[idx].Trim();
                if (!tmp.Equals("T1") && !tmp.Equals("T3"))
                {
                    return null;
                }
            }

            // ori_number
            idx = fields["ClearServiceID"];
            if (arrLen > idx)
            {
                string number = arr[idx].Trim();
                if (number.Length == 9 && number[8] == '0')
                {
                    number = number.Substring(0, 8);
                }
                cr.OriNumber = number;
            }
            else {
                return null;
            }

            // call_start
            if (arrLen > fields["ChargeDate"] && arrLen > fields["ChargeTime"])
            {
                tmp = arr[fields["ChargeDate"]].Trim() + " " + arr[fields["ChargeTime"]].Trim();
                if (!" ".Equals(tmp))
                {
                    cr.CallStart = DateTime.Parse(tmp, new CultureInfo("en-NZ"));
                }
            }
            else {
                return null;
            }

            // duration
            idx = fields["Duration"];
            if (arrLen > idx)
            {
                tmp = arr[idx].Trim();
                if (!"".Equals(tmp))
                {
                    cr.Duration = int.Parse(tmp);
                }
            }
            else {
                return null;
            }

            // cost & charge
            idx = fields["AmountIncl"];
            if (arrLen > idx)
            {
                tmp = arr[idx].Trim();
                if (!"".Equals(tmp))
                {
                    cr.Cost = double.Parse(tmp);
                }
            }
            else {
                return null;
            }

            // des_number
            idx = fields["PhoneCalled"];
            if (arrLen > idx)
            {
                cr.DesNumber = arr[idx].Trim();
            }

            // type
            idx = fields["Juristiction"];
            int idx2 = fields["CallTypeCode"];
            if (arrLen > idx)
            {
                switch (arr[idx].Trim().ToUpper())
                {
                    case "I":
                        cr.AreaPrefix = cr.DesNumber.Substring(0, cr.DesNumber.IndexOf("-"));
                        cr.Type = CallingRecord.Types.International;
                        break;
                    case "N":
                        cr.AreaPrefix = "3";
                        cr.Type = CallingRecord.Types.National;
                        break;
                    case "O":
                        cr.AreaPrefix = "2";
                        cr.Type = CallingRecord.Types.Mobile;
                        cr.IsMobile = true;
                        break;
                    case "L":
                        cr.AreaPrefix = "0";
                        cr.Type = CallingRecord.Types.Local;
                        break;
                    default:
                        return null;
                }
            }
            else {
                return null;
            }

            // description
            idx = fields["BillingDescription"];
            if (arrLen > idx)
            {
                tmp = arr[idx].Trim();
                cr.Description = tmp.Length <= 100 ? tmp : tmp.Substring(0, 100);
            }
            else {
                return null;
            }


            if (cr.Type.Equals("I"))
            {
                //AreaName
                cr.AreaName = PstnCallingRate.GetAreaNameByPrefix(cr.AreaPrefix);
                //IsMobile
                if (!cr.IsMobile)
                {
                    cr.IsMobile = cr.AreaName.ToLower().Contains("mobile");
                }
            }

            //charge
            cr.RatePerMinute = PstnCallingRate.GetRate(cr.AreaPrefix, cr.AreaName, cr.Type.Equals("I"));
            cr.ChargeMinute = CallingRecord.ConvertSecondDurationToMinute(cr.Duration);
            cr.Charge = cr.RatePerMinute * cr.ChargeMinute;

            return cr;
        }

        public override AddonCharge ParseAddonRecord(string line)
        {
            AddonCharge asr = new AddonCharge()
            {
                Id = Guid.NewGuid().ToString()
            };
            string[] arr = line.Split(',');
            int arrLen = arr.Length;
            String tmp = null;
            
            // charge date
            if (arrLen > fields["StatementDate"])
            {
                tmp = arr[fields["StatementDate"]].Trim();
                if (!string.IsNullOrEmpty(tmp))
                {
                    asr.ChargeDate = DateTime.Parse(tmp, new CultureInfo("en-NZ"));
                }
            }
            else {
                return null;
            }


            // record type
            if (arrLen > fields["RecordType"])
            {
                tmp = arr[fields["RecordType"]].Trim();
                if (string.IsNullOrEmpty(tmp) || !tmp.Equals("AC"))
                {
                    return null;
                }
            }
            else {
                return null;
            }


            // description
            int idx = fields["BillingDescription"];
            tmp = arr[idx].Trim();
            if (arrLen > idx)
            {
                asr.Description = tmp.Length <= 100 ? tmp : tmp.Substring(0, 100);
            }
            else {
                return null;
            }

            // ori_number
            idx = fields["ClearServiceID"];
            if (arrLen > idx)
            {
                string number = arr[idx].Trim();
                if (number.Length >= 10)
                {
                    //asid
                    number = number.Substring(0, 10);
                }else if(number.Length >= 8)
                {
                    number = number.Substring(0, 8);
                }
                asr.OriNumber = number;
            }
            else {
                return null;
            }

            // date from
            idx = fields["DateFrom"];
            if (arrLen > idx)
            {
                tmp = arr[idx].Trim();
                if (!"".Equals(tmp))
                {
                    asr.DateFrom = DateTime.Parse(tmp, new CultureInfo("en-NZ"));
                }
            }
            else {
                //return null;
            }

            // date to
            idx = fields["DateTo"];
            if (arrLen > idx)
            {
                tmp = arr[idx].Trim();
                if (!"".Equals(tmp))
                {
                    asr.DateTo = DateTime.Parse(tmp, new CultureInfo("en-NZ"));
                }
            }
            else {
                return null;
            }

            // cost & charge
            idx = fields["AmountIncl"];
            if (arrLen > idx)
            {
                tmp = arr[idx].Trim();
                if (!"".Equals(tmp))
                {
                    asr.Cost = double.Parse(tmp);
                    asr.Charge = asr.Cost;
                }
            }
            else {
                return null;
            }

            return asr;
        }
    }
}
