namespace CyberPark.Domain.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Linq;

    [Table("calling_records")]
    public partial class CallingRecord
    {
        public class Types
        {
            private Types()
            {

            }
            public const string International = "I";
            public const string National = "N";
            public const string Mobile = "M";
            public const string Local = "L";
        }

        public string Id { get; set; }

        [Required]
        [StringLength(128)]
        public string FileId { get; set; }

        [Required]
        [StringLength(50)]
        public string OriNumber { get; set; }

        [StringLength(50)]
        public string DesNumber { get; set; }

        [StringLength(20)]
        public string AreaPrefix { get; set; }

        [StringLength(50)]
        public string AreaName { get; set; }

        [Required]
        [StringLength(20)]
        public string Type { get; set; }

        [Column(TypeName = "bit")]
        public bool IsMobile { get; set; }

        public DateTime CallStart { get; set; }

        public int Duration { get; set; }

        public double Cost { get; set; }

        public double RatePerMinute { get; set; }
        public int ChargeMinute { get; set; }
        public double Charge { get; set; }

        public int? ActualChargeMiute { get; set; }
        public double? ActualCharge { get; set; }



        [StringLength(128)]
        public string OfferId { get; set; }

        [StringLength(128)]
        public string ServiceId { get; set; }
        public string ChargeRecordId { get; set; }
        public string PhoneType { get; set; }



        [Column(TypeName = "bit")]
        public bool Ignored { get; set; }

        public int? IgnoredBy { get; set; }

        public string Description { get; set; }
        public string Warning { get; set; }
        public string Memo { get; set; }

        [ForeignKey("FileId")]
        public virtual ExternalBill OriginalBill { get; set; }
        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; }
        [ForeignKey("ChargeRecordId")]
        public virtual CallingCharge ChargeRecord { get; set; }
        [ForeignKey("OfferId")]
        public virtual ServiceUsageOffer UsageOffer { get; set; }
    }
}
