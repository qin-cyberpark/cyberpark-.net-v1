using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberPark.Domain.Core
{
    public partial class ExternalAddOnConverter
    {
        public ExternalAddOnConverter(string keywords) {
            Id = Guid.NewGuid().ToString();
            Keywords = keywords;
        }

        private static IList<ExternalAddOnConverter> _data;
        static ExternalAddOnConverter()
        {
            if(_data == null)
            {
                Reload();
            }
        }

        public static void Reload()
        {
            using (var db = new xISPContext())
            {
                _data = db.ExternalAddOnConverters.ToList();
            }
        }

        public static ExternalAddOnConverter Match(string description)
        {
            foreach(var converter in _data)
            {
                if (description.ToLower().Contains(converter.Keywords.ToLower()))
                {   
                    //known description
                    if (converter.IsDisplay??false)
                    {
                        return converter;
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            //new description
            Add(description);
            return null;
        }

        public static void Add(string keywords) {
            using (var db = new xISPContext())
            {
                db.ExternalAddOnConverters.Add(new ExternalAddOnConverter(keywords));
                db.SaveChanges();
            }
            Reload();
        }
    }
}
