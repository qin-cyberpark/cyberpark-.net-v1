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
    
    public partial class equipment
    {
        public string Id { get; set; }
        public string ModelId { get; set; }
        public System.DateTime StoredDate { get; set; }
        public string ProductId { get; set; }
        public Nullable<System.DateTime> DispatchedDate { get; set; }
        public string Status { get; set; }
    
        public virtual equipment_models equipment_models { get; set; }
        public virtual product product { get; set; }
    }
}