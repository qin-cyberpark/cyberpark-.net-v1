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
    
    public partial class tm_manual_defray_log
    {
        public int id { get; set; }
        public Nullable<int> user_id { get; set; }
        public Nullable<int> customer_id { get; set; }
        public Nullable<int> order_id { get; set; }
        public Nullable<int> invoice_id { get; set; }
        public Nullable<double> eliminate_amount { get; set; }
        public string eliminate_way { get; set; }
        public Nullable<System.DateTime> eliminate_time { get; set; }
    }
}
