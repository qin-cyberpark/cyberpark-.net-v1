using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;

namespace CyberPark.Domain.Core
{
    public class Application
    {
        //public class ApplicationPrice
        //{
        //    public string Address { get; set; }
        //    public bool IsBusiness { get; set; }
        //    public ServicePrice Broadband { get; set; }
        //    //public IList<ServicePrice> Phones { get; set; }
        //    public ServicePrice VoIP { get; set; }
        //    public ServicePrice PSTN { get; set; }
        //    public ServicePrice NewConnection { get; set; }
        //    public EquipmentModel Equipment { get; set; }
        //}

        //public static ApplicationPrice Get(xISPContext db, string address, string servicePriceId)
        //{
        //    ApplicationPrice price = new ApplicationPrice
        //    {
        //        Address = address
        //    };

        //    //get broadband price
        //    price.Broadband = ServicePrice.Get(db, x => x.Id.Equals(servicePriceId)).SingleOrDefault();
        //    price.IsBusiness = price.Broadband.IsBusiness;
        //    //get phones
        //    price.VoIP = ServicePrice.Get(db, x => x.IsActive && x.IsBusiness == price.IsBusiness 
        //                                    && x.ServiceSubType.Equals(Service.PhoneSubTypes.VoIP)).FirstOrDefault();
        //    price.PSTN = ServicePrice.Get(db, x => x.IsActive && x.IsBusiness == price.IsBusiness
        //                        && x.ServiceSubType.Equals(Service.PhoneSubTypes.PSTN)).FirstOrDefault();
        //    //get new connection
        //    price.NewConnection = ServicePrice.Get(db, x => x.IsActive && x.IsBusiness == price.IsBusiness
        //                            && x.ServiceSubType.Equals(Service.MisellaneousSubTypes.NewConnection)).FirstOrDefault();
        //    //get hardware
        //    price.Equipment = db.ServiceEquipments.Include(x => x.Model).Where(x => x.ServiceSubType.Equals(price.Broadband.ServiceSubType)
        //                              && x.IsDefault).FirstOrDefault()?.Model;
      
        //    return price;
        //}
    }
}
