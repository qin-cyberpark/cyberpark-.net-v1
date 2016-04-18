using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyberPark.Domain;
namespace CyberPark.AutoJob
{
    class Program
    {
        static void Main(string[] args)
        {
            
            if(args.Length < 1)
            {
                return;
            }

            Console.WriteLine("running CyberPark - " + args[0]);

            switch (args[0].ToLower())
            {
                case "issueinv":
                    CyberPark.Domain.Core.Account.IssueAllInvoice(DateTime.Now, CyberPark.Domain.Core.SysConfig.Instance.AutoOperatorId);
                    break;
                case "deliverinv":
                    CyberPark.Domain.Core.Invoice.DeliverAll();
                    break;
                default:
                    break;
            }
        }
    }
}
