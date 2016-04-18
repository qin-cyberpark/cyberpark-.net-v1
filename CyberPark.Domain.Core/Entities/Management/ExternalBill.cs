using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CyberPark.Domain.Core
{
    public class ExternalBillWarnings
    {
        private ExternalBillWarnings()
        {

        }
        public const string ServiceNotInUsing = "Not In Using";
        public const string Unmatched = "Unmatched";
        public const string Error = "Error";

    }
    public class ExternalBillSources
    {
        private ExternalBillSources()
        {

        }
        public const string Unknown = "Unknown";
        public const string VOS = "VOS";
        public const string CallPlus = "CallPlus";
        public const string Chorus = "Chorus";
    }

    [Table("external_bill_files")]
    public partial class ExternalBill
    {
        private ExternalBill()
        {
            AddonCharges = new HashSet<AddonCharge>();
            CallingRecords = new HashSet<CallingRecord>();
        }

        public string Id { get; private set; }

        [Required]
        [StringLength(50)]
        public string FileName { get; private set; }

        [Required]
        [StringLength(10)]
        public string Source { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        [Column("CallOriNumberCount")]
        public int CallOriginalNumberCount { get; set; }
        public int CallRecordCount { get; set; }
        public double? CallTotalCost { get; set; }
        public DateTime? CallDateFrom { get; set; }
        public DateTime? CallDateTo { get; set; }
        [Column("AddonOriNumberCount")]
        public int? AddonOriginalNumberCount { get; set; }
        public int? AddonRecordCount { get; set; }
        public double? AddonTotalCost { get; set; }
        public DateTime? AddonDateFrom { get; set; }
        public DateTime? AddonDateTo { get; set; }
        public double Size { get; set; }
        public int OperatedBy { get; set; }
        public DateTime OperatedDate { get; set; }

        public virtual ICollection<AddonCharge> AddonCharges { get; private set; }
        public virtual ICollection<CallingRecord> CallingRecords { get; private set; }
    }
}
