using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberPark.Domain.Core
{
    internal class CallPlusBillParser : BaseExternalBillParser
    {

        public CallPlusBillParser(string header) : base(header)
        {
            // TODO Auto-generated constructor stub
            BillSource = ExternalBillSources.CallPlus;
            fields = new SortedList<string, int>();
            fields.Add("Origin Number", -1);
            fields.Add("Destination Number", -1);
            fields.Add("Description", -1);
            fields.Add("Date", -1);
            fields.Add("Time", -1);
            fields.Add("Call Length (seconds)", -1);
            fields.Add("Call Cost (NZD)", -1);
            fields.Add("Type", -1);
        }

        public override BaseExternalBillParser NextParser()
        {
            return new ChorusBillParser(_header);
        }

        public override CallingRecord ParseCallingRecord(string line)
        {
            try
            {
                CallingRecord cr = new CallingRecord()
                {
                    Id = Guid.NewGuid().ToString()
                };

                string[] arr = line.Split(',');
                int arrLen = arr.Length;
                string tmp = null;
                string tmp2 = null;

                // ori_number
                int idx = fields["Origin Number"];
                if (arrLen > idx)
                {
                    cr.OriNumber = arr[idx].Trim();
                }
                else {
                    return null;
                }

                // call_start
                idx = fields["Date"];
                int idx2 = fields["Time"];
                if (arrLen > idx)
                {
                    tmp = arr[idx].Trim();
                    tmp2 = tmp + " " + arr[idx2].Trim();
                    cr.CallStart = DateTime.Parse(tmp2, new CultureInfo("en-NZ"));
                }
                else {
                    return null;
                }

                // duration
                idx = fields["Call Length (seconds)"];
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
                idx = fields["Call Cost (NZD)"];
                if (arrLen > idx)
                {
                    tmp = arr[idx].Trim().Replace("$", "");
                    if (!"".Equals(tmp))
                    {
                        cr.Cost = double.Parse(tmp);
                    }
                }
                else {
                    return null;
                }

                // des_number
                idx = fields["Destination Number"];
                if (arrLen > idx)
                {
                    cr.DesNumber = arr[idx].Trim();
                    if (cr.DesNumber.Equals("unknown"))
                    {
                        return null;
                    }
                }
                else {
                    return null;
                }

                // type
                idx = fields["Type"];
                if (arrLen > idx)
                {
                    switch (arr[idx].Trim().ToUpper())
                    {
                        case "I":   /* International */
                            cr.Type = CallingRecord.Types.International;
                            break;
                        case "S":   /* National */
                            cr.Type = CallingRecord.Types.National;
                            cr.AreaPrefix = "3";
                            break;
                        case "L":   /* Local */
                            cr.Type = CallingRecord.Types.Local;
                            cr.AreaPrefix = "0";
                            break;
                        case "M":   /* Mobile */
                        case "MG":  /* Mobile Gsm */
                            cr.Type = CallingRecord.Types.Mobile;
                            cr.AreaPrefix = "2";
                            cr.IsMobile = true;
                            break;
                        default:
                            return null;
                    }
                }
                else {
                    return null;
                }

                // description
                idx = fields["Description"];
                if (arrLen > idx)
                {
                    tmp = arr[idx].Trim();
                    cr.Description = tmp.Length <= 100 ? tmp:tmp.Substring(0,100);
                }
                else {
                    return null;
                }

                if (cr.Type.Equals(CallingRecord.Types.International))
                {
                    //IsMobile
                    cr.IsMobile = cr.Description.ToLower().Contains("mobile");
                    //AreaName
                    cr.AreaName = PstnCallingRate.GetAreaNameByDesc(cr.Description);
                }

                //charge
                cr.RatePerMinute = PstnCallingRate.GetRate(cr.AreaPrefix, cr.AreaName, cr.Type.Equals(CallingRecord.Types.International));
                cr.ChargeMinute = CallingRecord.ConvertSecondDurationToMinute(cr.Duration);
                cr.Charge = cr.RatePerMinute * cr.ChargeMinute;

                return cr;
            }
            catch
            {
                return null;
            }
        }
    }
}
