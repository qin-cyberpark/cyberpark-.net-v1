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
    
    public partial class tm_ticket
    {
        public int id { get; set; }
        public Nullable<int> customer_id { get; set; }
        public Nullable<int> user_id { get; set; }
        public string cellphone { get; set; }
        public string email { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string ticket_type { get; set; }
        public string publish_type { get; set; }
        public string description { get; set; }
        public string protected_viewer { get; set; }
        public string not_yet_viewer { get; set; }
        public string viewed_viewer { get; set; }
        public string not_yet_review_comment_viewer { get; set; }
        public Nullable<System.DateTime> create_date { get; set; }
        public Nullable<bool> existing_customer { get; set; }
        public Nullable<bool> commented { get; set; }
    }
}