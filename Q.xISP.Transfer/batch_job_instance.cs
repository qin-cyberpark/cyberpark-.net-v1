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
    
    public partial class batch_job_instance
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public batch_job_instance()
        {
            this.batch_job_execution = new HashSet<batch_job_execution>();
        }
    
        public long JOB_INSTANCE_ID { get; set; }
        public Nullable<long> VERSION { get; set; }
        public string JOB_NAME { get; set; }
        public string JOB_KEY { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<batch_job_execution> batch_job_execution { get; set; }
    }
}
