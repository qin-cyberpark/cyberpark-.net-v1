using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberPark.Domain.Core
{
    internal class BaseExternalBillParser
    {
        protected string _header;
        protected SortedList<string, int> fields = null;
        public string BillSource = ExternalBillSources.Unknown;

        public BaseExternalBillParser(string header)
        {
            _header = header;
        }

        public bool isValid()
        {
            if (fields == null)
            {
                return false;
            }

            string[] arr = _header.Split(',');
            int len = arr.Length;
            for (int i = 0; i < len; i++)
            {
                if (fields.ContainsKey(arr[i]))
                {
                    fields[arr[i]] = i;
                }
            }

            foreach (int idx in fields.Values)
            {
                if (idx < 0)
                {
                    // not include all the necessary fields
                    return false;
                }
            }

            return true;
        }

        public bool isInclude()
        {
            if (fields == null)
            {
                return false;
            }

            return true;
        }

        public virtual CallingRecord ParseCallingRecord(string line)
        {
            return null;
        }

        public virtual AddonCharge ParseAddonRecord(string line)
        {
            return null;
        }

        public virtual BaseExternalBillParser NextParser()
        {
            return null;
        }
    }
}
