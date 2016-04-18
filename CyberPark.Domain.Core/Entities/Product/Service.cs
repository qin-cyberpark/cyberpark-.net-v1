namespace CyberPark.Domain.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;


    [Table("services")]
    public partial class Service
    {
        public Service()
        {
            AddonCharges = new HashSet<AddonCharge>();
            CallingRecords = new HashSet<CallingRecord>();
        }

        public string Id { get; set; }

        [Required]
        [StringLength(128)]
        public string ProductId { get; set; }

        [Required]
        [StringLength(20)]
        public string Type { get; set; }

        [StringLength(20)]
        public string SubType { get; set; }

        [StringLength(20)]
        public string IdentityNumber { get; set; }

        [StringLength(10)]
        public string BroadbandSVLAN { get; set; }

        [StringLength(10)]
        public string BroadbandCVLAN { get; set; }

        [StringLength(50)]
        public string BroadbandPPPoeLoginName { get; set; }

        [StringLength(50)]
        public string BroadbandPPPoePassword { get; set; }

        [StringLength(20)]
        public string VoipPassword { get; set; }

        public DateTime? VoipAssignedDate { get; set; }

        public DateTime? ReadyForServiceDate { get; set; }

        [Required]
        [StringLength(10)]
        public string Status { get; set; }

        public bool IsDeleted { get; set; }

        public int OperatorId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        public virtual ICollection<AddonCharge> AddonCharges { get; set; }
        public virtual ICollection<CallingRecord> CallingRecords { get; set; }
        public virtual ICollection<CallingCharge> CallingCharges { get; set; }
    }
}