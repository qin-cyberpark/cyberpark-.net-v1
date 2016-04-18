namespace CyberPark.Domain.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("service_package_terms")]
    public partial class ServicePackageTerm
    {
        public ServicePackageTerm()
        {
        }

        public string Id { get; set; }

        [Required]
        [StringLength(128)]
        public string PackageId { get; set; }

        public DateTime? StartDate { get; set; }

        public int Months { get; set; }

        public DateTime? EndDate { get; set; }

        [ForeignKey("PackageId")]
        public virtual ServicePackage ServicePackage { get; set; }

        [NotMapped]
        public string Description{
            get{
                return string.Format("{0} months - {1:ddMMM yyyy} to {2:ddMMM yyyy}", Months, StartDate, EndDate);
            }
        }
    }
}
