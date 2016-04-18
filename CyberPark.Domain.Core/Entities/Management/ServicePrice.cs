namespace CyberPark.Domain.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Data.Entity;

    [Table("service_prices")]
    public partial class ServicePrice
    {
        public string Id { get; set; }

        public string ServiceType { get; set; }
        public string ServiceSubType { get; set; }
        public string Name { get; set; }

        public double PriceGSTExclusive { get; set; }
        [Column(TypeName = "bit")]
        public bool IsBusiness { get; set; }
        [Column(TypeName ="bit")]
        public bool IsMonthly{ get; set; }
        public int MinContractMonths { get; set; }
        [Column(TypeName = "bit")]
        public bool IsActive { get; set; }


        [Column(TypeName = "text")]
        [StringLength(255)]
        public string Description { get; set; }
    }
}
