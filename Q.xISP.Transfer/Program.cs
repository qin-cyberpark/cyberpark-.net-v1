using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Q.xISP.Transfer
{
    public class Program
    {
        static void Main(string[] args)
        {
            //init role
            //DataTranfer.InitRole();

            //Plan
            //DataTranfer.ConvertPlan();

            //Staff
            //DataTranfer.ConvertOperator();

            //orders
            var oriCustomers = DataTranfer.LoadCustomer();
            var asids = new SortedList<int, string>();
            using (var db = new OriEntities())
            {
                db.tm_customer_order_broadband_asid.Distinct().ToList().ForEach(x =>
                {
                    if (!asids.ContainsKey(x.order_id.Value))
                    {
                        asids.Add(x.order_id.Value, x.broadband_asid);
                    }
                });
            }
            using (var db = new NewEntities())
            {
                foreach (OriCustomer oc in oriCustomers)
                {
                    DataTranfer.ConvertCustomerAndOrder(db, asids, oc);
                }
            }

            //transaction
            //DataTranfer.ConvertTransactionAfter(new DateTime(2015, 12, 31, 23, 59, 59));
            
            Console.WriteLine("finished");
            Console.ReadLine();
        }
    }
}
