using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CyberPark.Domain.Core;
using CyberPark.Website.Models;
namespace CyberPark.Website.ViewModels
{
    public class StaffViewModels
    {
        public class NewStaffViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string BranchId { get; set; }
            public string Branch { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public string Mobile { get; set; }
            public string OriRole { get; set; }
            public string Role { get; set; }
            public string Password { get; set; }
            public string ConfirmPassword { get; set; }
        }

       
    }
}