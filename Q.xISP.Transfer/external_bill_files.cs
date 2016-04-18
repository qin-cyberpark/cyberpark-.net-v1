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
    
    public partial class external_bill_files
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public external_bill_files()
        {
            this.addon_charge_records = new HashSet<addon_charge_records>();
            this.calling_records = new HashSet<calling_records>();
        }
    
        public string Id { get; set; }
        public string FileName { get; set; }
        public string Source { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int CallOriNumberCount { get; set; }
        public int CallRecordCount { get; set; }
        public Nullable<double> CallTotalCost { get; set; }
        public Nullable<System.DateTime> CallDateFrom { get; set; }
        public Nullable<System.DateTime> CallDateTo { get; set; }
        public Nullable<int> AddonOriNumberCount { get; set; }
        public Nullable<int> AddonRecordCount { get; set; }
        public Nullable<double> AddonTotalCost { get; set; }
        public Nullable<System.DateTime> AddonDateFrom { get; set; }
        public Nullable<System.DateTime> AddonDateTo { get; set; }
        public double Size { get; set; }
        public int OperatedBy { get; set; }
        public System.DateTime OperatedDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<addon_charge_records> addon_charge_records { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<calling_records> calling_records { get; set; }
    }
}
