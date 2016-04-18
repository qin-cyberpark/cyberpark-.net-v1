using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CyberPark.Domain.Core
{
    [Table("external_bill_addon_converters")]
    public partial class ExternalAddOnConverter
    {
        private ExternalAddOnConverter()
        {

        }
        public string Id { get; set; }
        [Required]
        public string Keywords { get; set; }
        public string DisplayAs { get; set; }
        public double? Price { get; set; }
        [Column(TypeName ="bit")]
        public bool? IsDisplay { get; set; }
    }
}
