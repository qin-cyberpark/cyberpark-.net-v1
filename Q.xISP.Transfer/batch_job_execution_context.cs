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
    
    public partial class batch_job_execution_context
    {
        public long JOB_EXECUTION_ID { get; set; }
        public string SHORT_CONTEXT { get; set; }
        public string SERIALIZED_CONTEXT { get; set; }
    
        public virtual batch_job_execution batch_job_execution { get; set; }
    }
}
