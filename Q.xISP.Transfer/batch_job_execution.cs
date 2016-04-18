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
    
    public partial class batch_job_execution
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public batch_job_execution()
        {
            this.batch_job_execution_params = new HashSet<batch_job_execution_params>();
            this.batch_step_execution = new HashSet<batch_step_execution>();
        }
    
        public long JOB_EXECUTION_ID { get; set; }
        public Nullable<long> VERSION { get; set; }
        public long JOB_INSTANCE_ID { get; set; }
        public System.DateTime CREATE_TIME { get; set; }
        public Nullable<System.DateTime> START_TIME { get; set; }
        public Nullable<System.DateTime> END_TIME { get; set; }
        public string STATUS { get; set; }
        public string EXIT_CODE { get; set; }
        public string EXIT_MESSAGE { get; set; }
        public Nullable<System.DateTime> LAST_UPDATED { get; set; }
    
        public virtual batch_job_execution_context batch_job_execution_context { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<batch_job_execution_params> batch_job_execution_params { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<batch_step_execution> batch_step_execution { get; set; }
        public virtual batch_job_instance batch_job_instance { get; set; }
    }
}
