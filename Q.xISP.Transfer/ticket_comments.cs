//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Q.xISP.Transfer
{
    using System;
    using System.Collections.Generic;
    
    public partial class ticket_comments
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string Comment { get; set; }
        public System.DateTime CommentDate { get; set; }
        public string Name { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public Nullable<int> StaffId { get; set; }
    
        public virtual customer customer { get; set; }
        public virtual staff staff { get; set; }
        public virtual ticket ticket { get; set; }
    }
}