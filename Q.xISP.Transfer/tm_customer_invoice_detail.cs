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
    
    public partial class tm_customer_invoice_detail
    {
        public int id { get; set; }
        public Nullable<int> invoice_id { get; set; }
        public string invoice_detail_name { get; set; }
        public string invoice_detail_desc { get; set; }
        public Nullable<double> invoice_detail_price { get; set; }
        public Nullable<int> invoice_detail_unit { get; set; }
        public Nullable<System.DateTime> invoice_detail_date { get; set; }
        public Nullable<double> invoice_detail_discount { get; set; }
        public string invoice_detail_type { get; set; }
    }
}
