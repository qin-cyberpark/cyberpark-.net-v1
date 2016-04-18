namespace CyberPark.Domain.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;


    [Table("service_equipments")]
    public partial class ServiceEquipment
    {
        public ServiceEquipment()
        {
        }

        public string Id { get; set; }
        public string ServiceType { get; set; }
        public string ServiceSubType { get; set; }
        public string ModelId { get; set; }
        [Column(TypeName ="bit")]
        public bool IsDefault { get; set; }
        [ForeignKey("ModelId")]
        public virtual EquipmentModel Model { get; set; }
    }
}