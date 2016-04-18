namespace CyberPark.Domain.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public partial class Branch
    {
        public static IList<Branch> Get(xISPContext db)
        {
            return db.Branches.ToList();
        }
    }
}