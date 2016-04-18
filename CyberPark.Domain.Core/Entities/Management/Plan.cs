namespace CyberPark.Domain.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Data.Entity;

    [Table("plans")]
    public partial class Plan
    {
        public string Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        public double MonthlyPrice { get; set; }
        public double ModemPrice { get; set; }
        public double NewConnectionCharge { get; set; }
        public int MonthsOfContract { get; set; }

        [Column(TypeName ="bit")]
        public bool IsBusiness { get; set; }

        [Required]
        [StringLength(20)]
        public string BroadbandType { get; set; }

        public int PstnCount { get; set; }

        public int VoipCount { get; set; }

        public bool IsPromotion { get; set; }

        public int DisplayPriority { get; set; }

        public bool IsActive { get; set; }
    }
}
