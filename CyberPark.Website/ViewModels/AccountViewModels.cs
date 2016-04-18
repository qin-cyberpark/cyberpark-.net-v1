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

        public class SignUpViewModel
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Mobile { get; set; }
            public string Password { get; set; }
        }
    }
}