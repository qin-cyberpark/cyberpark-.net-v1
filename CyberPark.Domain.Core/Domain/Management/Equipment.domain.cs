using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberPark.Domain.Core
{
    public partial class Equipment
    {
        public class EquipmentTypes
        {
            private EquipmentTypes()
            {

            }
            public const string Router = "Router";
        }
        public class Statuses
        {
            private Statuses()
            {

            }
            public const string Warehouse = "Warehouse";
            public const string Dispatched = "Dispatched";
            public const string Discarded = "Discarded";
        }
    }
}
