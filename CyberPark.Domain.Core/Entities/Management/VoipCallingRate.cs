namespace CyberPark.Domain.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Linq;

    [Table("calling_rates_voip")]
    public partial class VoipCallingRate
    {
        [NotMapped]
        private static List<VoipCallingRate> _rates;

        static VoipCallingRate()
        {
            using (xISPContext db = new xISPContext())
            {
                _rates = db.VoipCallingRates.ToList();
            }
        }


        private VoipCallingRate()
        {

        }

        [Key]
        [Column(Order = 0)]
        [StringLength(10)]
        public string Prefix { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string Type { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(50)]
        public string AreaName { get; set; }

        [Key]
        [Column(Order = 3)]
        public double RatePerMinute { get; set; }

        public static double GetRate(string prefix, string area, bool isInternational)
        {
            prefix = ForamtPrefix(prefix);
            IList<VoipCallingRate> rates;
            if (isInternational)
            {
                rates = _rates.Where(x => x.Type.Equals("International")).ToList();
            }
            else
            {
                rates = _rates.Where(x => !x.Type.Equals("International")).ToList();
            }
            while (prefix.Length > 0)
            {
                var rate = rates.FirstOrDefault(x => x.Prefix.Equals(prefix));
                if (rate != null)
                {
                    return rate.RatePerMinute;
                }
                prefix = prefix.Substring(0, prefix.Length - 1);
            }

            return -1;
        }

        public static string GetAreaNameByPrefix(string prefix, bool isInternational)
        {
            prefix = ForamtPrefix(prefix);
            IList<VoipCallingRate> rates;
            if (isInternational)
            {
                rates = _rates.Where(x => x.Type.Equals("International")).ToList();
            }
            else
            {
                rates = _rates.Where(x => !x.Type.Equals("International")).ToList();
            }
            while (prefix.Length > 0)
            {
                var rate = rates.FirstOrDefault(x => x.Prefix.Equals(prefix));
                if (rate != null)
                {
                    return rate.AreaName;
                }
                prefix = prefix.Substring(0, prefix.Length - 1);
            }
            return null;
        }

        public static string ForamtPrefix(string num)
        {
            string result = num;
            while (result.StartsWith("0"))
            {
                result = result.Remove(0, 1);
            }
            return result;
        }

    }
}
