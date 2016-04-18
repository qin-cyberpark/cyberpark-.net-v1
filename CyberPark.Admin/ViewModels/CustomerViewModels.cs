using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CyberPark.Domain.Core;
using System.Data.Entity;

namespace CyberPark.Website.ViewModels
{
    public class CustomerViewModels
    {
        public class CustomerListItem {
            private Customer _customer;
            private List<Account> _accounts = new List<Account>();
            public CustomerListItem(Customer customer)
            {
                _customer = customer;
                foreach(var acc in _customer.Accounts)
                {
                    _accounts.Add(
                        new Account
                        {
                            Id = acc.Id,
                            Address = acc.Address,
                            Balance = acc.Balance,
                            Type = acc.Type
                        }
                    );
                }
                
            }
            public int Id{get { return _customer.Id; }}
            public string Name { get { return _customer.Name; } }
            public string Email { get { return _customer.User.Email; } }
            public string Mobile { get { return _customer.User.PhoneNumber; } }
            public IList<Account> Accounts { get { return _accounts; } }

            public static IList<CustomerListItem> Convert(IList<Customer> customers)
            {
                IList<CustomerListItem>  result = new List<CustomerListItem>();
                if (customers == null) {
                    return result;
                }
                foreach(Customer c in customers)
                {
                    result.Add(new CustomerListItem(c));
                }
                return result;
            }
        }
    }
}