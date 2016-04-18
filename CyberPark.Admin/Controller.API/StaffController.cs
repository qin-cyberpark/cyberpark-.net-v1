using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CyberPark.Domain.Core;
using CyberPark.Website.ViewModels;
using System.Web;
using Microsoft.AspNet.Identity.Owin;

namespace CyberPark.Website.Controllers.API
{
    [Authorize(Roles = Domain.Core.User.RoleTypes.Staff)]
    public class StaffController : ApiController
    {
        private xISPContext _db;
        private xISPUserManager _userManager;
        public xISPUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<xISPUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public StaffController()
        {
            _db = new xISPContext();
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }


        //GET: /api/staff
        public ApiResult<IList<StaffViewModels.NewStaffViewModel>> Get()
        {
            var result = new List<StaffViewModels.NewStaffViewModel>();
            var staffs = Staff.Get(_db);
            foreach(var staff in staffs)
            {
                var model = new StaffViewModels.NewStaffViewModel
                {
                    Id = staff.Id,
                    BranchId = staff.BranchId,
                    Branch = staff.Branch.Name,
                    Name = staff.Name,
                    UserName = staff.User.UserName,
                    Email = staff.User.Email,
                    Mobile = staff.User.PhoneNumber
                };
                var roles = UserManager.GetRolesAsync(staff.Id).Result;
                foreach(var r in roles)
                {
                    if (!Domain.Core.User.RoleTypes.Staff.Equals(r))
                    {
                        model.OriRole = r;
                        model.Role = r;
                    }
                }
                result.Add(model);
            }
            var rslt = new ApiResult<IList<StaffViewModels.NewStaffViewModel>> {
                Data = result
            };

            return rslt;
        }

        //PUT: /api/staff
        [HttpPut]
        public ApiResult<StaffViewModels.NewStaffViewModel> Update(StaffViewModels.NewStaffViewModel model)
        {
            //save staff
            var staff = Staff.GetById(_db, model.Id);
            staff.BranchId = model.BranchId;
            staff.Email = model.Email;
            staff.Mobile = model.Mobile;
            staff.Name = model.Name;
            staff.Save(_db);

            //save user
            var user = UserManager.FindByIdAsync(model.Id).Result;
            user.UserName = model.UserName;
            user.Email = model.Email;
            user.PhoneNumber = model.Mobile;

            //update password
            if (!string.IsNullOrEmpty(model.Password) && model.Password.Equals(model.ConfirmPassword))
            {
                user.PasswordHash = UserManager.PasswordHasher.HashPassword(model.Password);
            }
            UserManager.UpdateAsync(user);

            //update role
            if (!model.Role.Equals(model.OriRole))
            {
                if (!string.IsNullOrEmpty(model.OriRole))
                {
                    var delRslt = UserManager.RemoveFromRoleAsync(model.Id, model.OriRole).Result;
                }
                var result = UserManager.AddToRoleAsync(model.Id, model.Role).Result;
            }
            return new ApiResult<StaffViewModels.NewStaffViewModel>
            {
                Data = model
            };
        }

        //POST: /api/staff
        [HttpPost]
        public ApiResult<Staff> Create(Staff staff)
        {
            return new ApiResult<Staff>
            {
                Data = staff.Create(_db)
            };
        }

        //DELETE: /api/Staff/id
        [HttpDelete]
        public ApiResult<bool> Remove(int id)
        {
            return new ApiResult<bool>
            {
                Data = Staff.Delete(_db, id)
            };
        }
    }
}
