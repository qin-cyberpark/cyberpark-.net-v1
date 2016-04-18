namespace CyberPark.Domain.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("service_usage_offers")]
    public partial class ServiceUsageOffer
    {
        public string Id { get; set; }

        [Required]
        [StringLength(128)]
        public string ProductId { get; set; }

        [Required]
        [StringLength(20)]
        public string ServiceType { get; set; }

        [StringLength(20)]
        public string ServiceSubType { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public int Minutes { get; set; }
        public int DataFlow { get; set; }

        [Column(TypeName = "bit")]
        public bool Local { get; set; }

        [Column(TypeName = "bit")]
        public bool National { get; set; }

        [Column(TypeName = "bit")]
        public bool Mobile { get; set; }

        [StringLength(20)]
        public string CallingRegionId { get; set; }

        [Column(TypeName = "bit")]
        public bool IsDeleted { get; set; }

        public int OperatorId { get; set; } 

        [ForeignKey("CallingRegionId")]
        public virtual CallingInternationalRegion CallingRegion { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}
