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
    
    public partial class calling_international_region_details
    {
        public string Id { get; set; }
        public string RegionId { get; set; }
        public string CountryName { get; set; }
        public bool IncludeMobile { get; set; }
    
        public virtual calling_international_regions calling_international_regions { get; set; }
    }
}