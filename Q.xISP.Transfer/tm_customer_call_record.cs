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
    
    public partial class tm_customer_call_record
    {
        public int id { get; set; }
        public Nullable<System.DateTime> statement_date { get; set; }
        public string record_type { get; set; }
        public string clear_service_id { get; set; }
        public string line_description { get; set; }
        public Nullable<System.DateTime> date_from { get; set; }
        public Nullable<System.DateTime> date_to { get; set; }
        public Nullable<System.DateTime> charge_date_time { get; set; }
        public Nullable<int> duration { get; set; }
        public string oot_id { get; set; }
        public string billing_description { get; set; }
        public Nullable<double> amount_excl { get; set; }
        public Nullable<double> amount_incl { get; set; }
        public string phone_called { get; set; }
        public Nullable<System.DateTime> upload_date { get; set; }
        public string juristiction { get; set; }
        public Nullable<bool> used { get; set; }
        public Nullable<int> invoice_id { get; set; }
    }
}
