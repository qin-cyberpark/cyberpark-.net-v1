using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;

namespace Q.xISP.Transfer
{
    public class OriCustomer
    {
        public tm_customer Customer { get; set; }
        public List<OriOrder> Orders { get; private set; }
        public OriCustomer(tm_customer c)
        {
            Customer = c;
        }
        public static OriCustomer Load(tm_customer c)
        {
            OriCustomer ori = new OriCustomer(c);
            using(OriEntities db = new OriEntities())
            {
                //load order info
                ori.Orders = new List<OriOrder>();
                var orders = db.tm_customer_order.Where(x => x.customer_id == c.id).ToList();
                foreach (tm_customer_order o in orders)
                {
                    ori.Orders.Add(OriOrder.Load(o));
                }
            }

            return ori;
        }
    }

    public class OriOrder
    {
        public tm_customer_order Order { get; private set; }
        public List<tm_customer_order_detail> OrderDetails { get; private set;}
        public OriOrder(tm_customer_order ord)
        {
            Order = ord;
        }
        public static OriOrder Load(tm_customer_order ord)
        {
            OriOrder ori = new OriOrder(ord);
            using (OriEntities db = new OriEntities())
            {
                ori.OrderDetails = db.tm_customer_order_detail.Where(x => x.order_id == ord.id).ToList();
            }
            return ori;
        }
    }
}
