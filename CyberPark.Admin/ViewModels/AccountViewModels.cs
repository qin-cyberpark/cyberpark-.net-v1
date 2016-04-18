using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CyberPark.Website.ViewModels
{
    public class AccountViewModels
    {
        public class LoginViewModel
        {
            public string PhoneOrEmail { get; set; }
            public string Password { get; set; }
        }

        public class AccountSearchCondition
        {
            //customerId: "", accountId: "", name: "", address: "",
            //asid: "", pstn: "", voip: ""
            public string customerId { get; set; } = "";
            public string accountId { get; set; } = "";
            public string name { get; set; } = "";
            public string address { get; set; } = "";
            public string asid { get; set; } = "";
            public string pstn { get; set; } = "";
            public string voip { get; set; } = "";
            public string status { get; set; } = "";
        }
    }
}