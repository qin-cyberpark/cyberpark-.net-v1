namespace CyberPark.Domain.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Linq;
    using System.Runtime.Serialization;
    using Newtonsoft.Json.Serialization;

    [Table("tickets")]
    public partial class Ticket
    {
        public Ticket()
        {
            Comments = new HashSet<TicketComment>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Type { get; set; }
        public string SubType { get; set; }
        public string Title { get; set; }

        public int CustomerId { get; set; }
        public int AccountId { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }

        public DateTime? StaffLastViewedDate { get; set; }
        public DateTime? CustomerLastViewedDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime CloseDate { get; set; }
        public int? CloseBy { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }

        public virtual ICollection<TicketComment> Comments { get; set; }
    }

    [Table("ticket_comments")]
    public partial class TicketComment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int TicketId { get; set; }
        [Column(TypeName ="text")]
        public string Comment { get; set; }
        public DateTime CommentDate { get; set; }
        public string Name { get; set; }
        public int? CustomerId { get; set; }
        public int? StaffId { get; set; }

        [ForeignKey("TicketId")]
        public virtual Ticket Ticket { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
        [ForeignKey("StaffId")]
        public virtual Staff Staff { get; set; }

    }
}
