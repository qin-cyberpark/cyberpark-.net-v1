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
    
    public partial class tm_vos_voip_call_record
    {
        public int id { get; set; }
        public Nullable<System.DateTime> call_start { get; set; }
        public string ori_number { get; set; }
        public string dest_number { get; set; }
        public Nullable<int> call_length { get; set; }
        public Nullable<int> charge_length { get; set; }
        public Nullable<double> call_fee { get; set; }
        public Nullable<double> call_cost { get; set; }
        public string call_type { get; set; }
        public Nullable<int> area_prefix { get; set; }
        public Nullable<int> invoice_id { get; set; }
    }
}
