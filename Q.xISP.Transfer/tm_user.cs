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
    
    public partial class tm_user
    {
        public int id { get; set; }
        public string login_name { get; set; }
        public string password { get; set; }
        public string user_name { get; set; }
        public string user_role { get; set; }
        public string memo { get; set; }
        public string auth { get; set; }
        public string cellphone { get; set; }
        public string email { get; set; }
        public Nullable<bool> is_provision { get; set; }
        public Nullable<double> agent_commission_rates { get; set; }
        public Nullable<double> invite_commission { get; set; }
    }
}