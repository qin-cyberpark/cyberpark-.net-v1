using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberPark.Domain.Core
{
    internal class VOSBillParser : BaseExternalBillParser
    {
        public VOSBillParser(string header) : base(header)
        {
            // TODO Auto-generated constructor stub
            BillSource = ExternalBillSources.VOS;
            fields = new SortedList<string, int>();
            fields.Add("主叫号码", -1);
            fields.Add("被叫号码", -1);
            fields.Add("起始时间", -1);
            fields.Add("通话时长", -1);
            fields.Add("计费时长", -1);
            fields.Add("通话费用", -1);
            fields.Add("话费成本", -1);
            fields.Add("通话类型", -1);
            fields.Add("地区前缀", -1);
        }

        public override BaseExternalBillParser NextParser()
        {
            return new CallPlusBillParser(_header);
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

            // ori_number
            int idx = fields["主叫号码"];
            if (arrLen > idx)
            {
                cr.OriNumber = arr[idx].Trim();
            }
            else {
                return null;
            }

            // call_start
            idx = fields["起始时间"];
            if (arrLen > idx)
            {
                tmp = arr[idx].Trim();
                if (!"".Equals(tmp))
                {
                    cr.CallStart = DateTime.Parse(tmp);
                }
            }
            else {
                return null;
            }

            // duration
            idx = fields["计费时长"];
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

            // cost / charge
            idx = fields["通话费用"];
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
            idx = fields["被叫号码"];
            if (arrLen > idx)
            {
                cr.DesNumber = arr[idx].Trim();
            }
            else {
                return null;
            }

            // type
            idx = fields["通话类型"];
            if (arrLen > idx)
            {
                switch (arr[idx].Trim().ToLower())
                {
                    // international and national only
                    case "国际长途":
                    case "international":
                        cr.Type = CallingRecord.Types.International;
                        break;
                    case "国内长途":
                    case "national":
                        cr.Type = CallingRecord.Types.National;
                        break;
                    default:
                        return null;
                }
            }
            else {
                return null;
            }

            // area_prefix
            idx = fields["地区前缀"];
            if (arrLen > idx)
            {
                cr.AreaPrefix = arr[idx].Trim();
            }
            else {
                return null;
            }

            //hard coding for 0064-internationl case
            if (!string.IsNullOrEmpty(cr.AreaPrefix) && cr.AreaPrefix.StartsWith("0064"))
            {
                cr.Type = CallingRecord.Types.National;
            }

            //AreaName
            cr.AreaName = VoipCallingRate.GetAreaNameByPrefix(cr.AreaPrefix, cr.Type.Equals(CallingRecord.Types.International));
            //IsMobile
            if (!string.IsNullOrEmpty(cr.AreaName) && cr.AreaName.ToLower().Contains("mobile"))
            {
                cr.IsMobile = true;
                if (cr.Type.Equals(CallingRecord.Types.National))
                {
                    cr.Type = CallingRecord.Types.Mobile;
                }
            }

            //charge
            cr.RatePerMinute = VoipCallingRate.GetRate(cr.AreaPrefix, cr.AreaName, cr.Type.Equals(CallingRecord.Types.International));
            cr.ChargeMinute = CallingRecord.ConvertSecondDurationToMinute(cr.Duration);
            cr.Charge = cr.RatePerMinute * cr.ChargeMinute;

            //actual charge
            return cr;
        }
    }
}
