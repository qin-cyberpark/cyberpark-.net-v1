namespace CyberPark.Domain.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Data.Entity;

    [Table("equipment_models")]
    public partial class EquipmentModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public double Price { get; set; }
    }

    [Table("equipments")]
    public partial class Equipment
    {
        [Key]
        [Column("Id")]
        public string SerialNumber { get; set; }
        public string ModelId { get; set; }
        public DateTime StoredDate { get; set; }
        public string ProductId { get; set; }
        public DateTime DispatchedDate { get; set; }
        public string Status { get; set; }

        [ForeignKey("ModelId")]
        public virtual EquipmentModel Model { get;set;}
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}
