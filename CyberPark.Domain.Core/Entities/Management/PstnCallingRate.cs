namespace CyberPark.Domain.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Data.Entity;

    [Table("calling_rates_pstn")]
    public partial class PstnCallingRate
    {
        [NotMapped]
        private static List<PstnCallingRate> _rates;
        static PstnCallingRate()
        {
            using (xISPContext db = new xISPContext())
            {
                _rates = db.PstnCallingRates.ToList();
            }
        }

        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string Prefix { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Type { get; set; }

        [Required]
        [StringLength(50)]
        public string AreaName { get; set; }

        public double RatePerMinute { get; set; }


        public static double GetRate(string prefix, string area, bool isInternational)
        {
            PstnCallingRate rate;
            if (isInternational)
            {
                rate = _rates.FirstOrDefault(x => x.Type.Equals("International") &&
                                   (x.Prefix.Equals(prefix) || x.AreaName.Equals(area)));
            }
            else
            {
                rate = _rates.FirstOrDefault(x => !x.Type.Equals("International") &&
                                   (x.Prefix.Equals(prefix) || x.AreaName.Equals(area)));
            }
            if (rate != null)
            {
                return rate.RatePerMinute;
            }

            return -1;
        }

        public static string GetAreaNameByDesc(string desc)
        {
            var rate = _rates.FirstOrDefault(x => x.AreaName.Equals(desc));
            if(rate != null)
            {
                return rate.AreaName;
            }
            
            return _rates.FirstOrDefault(x => desc.Contains(x.AreaName) && !string.IsNullOrEmpty(x.AreaName))?.AreaName;
        }

        public static string GetAreaNameByPrefix(string prefix)
        {
            return _rates.FirstOrDefault(x =>x.Prefix.Equals(prefix))?.AreaName;
        }
    }
}
