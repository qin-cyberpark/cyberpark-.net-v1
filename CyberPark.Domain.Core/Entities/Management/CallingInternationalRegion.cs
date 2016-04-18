namespace CyberPark.Domain.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("calling_international_regions")]
    public partial class CallingInternationalRegion
    {
        public CallingInternationalRegion()
        {
            Details = new HashSet<CallingInternationalRegionDetail>();
            CallingOffers = new HashSet<ServiceUsageOffer>();
        }

        [StringLength(20)]
        public string Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Description { get; set; }

        public virtual ICollection<CallingInternationalRegionDetail> Details { get; set; }
        public virtual ICollection<ServiceUsageOffer> CallingOffers { get; set; }
    }

    [Table("calling_international_region_details")]
    public partial class CallingInternationalRegionDetail
    {
        public string Id { get; set; }

        [Required]
        [StringLength(20)]
        public string RegionId { get; set; }

        [Required]
        [StringLength(100)]
        public string CountryName { get; set; }

        [Column(TypeName = "bit")]
        public bool IncludeMobile { get; set; }

        [ForeignKey("RegionId")]
        public virtual CallingInternationalRegion Region { get; set; }
    }
}
