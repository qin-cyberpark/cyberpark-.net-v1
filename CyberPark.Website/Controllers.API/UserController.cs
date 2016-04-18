using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Web;
using System.Web.Http;
using CyberPark.Domain.Core;
using CyberPark.Website.ViewModels;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using CyberPark.Website.Models;
using System.Linq;
using System;

namespace CyberPark.Website.Controllers.API
{
    [Authorize]
    public class UserController : ApiController
    {
        private xISPContext _db;
        public UserController()
        {
            _db = new xISPContext();

        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

        #region managers
        private xISPSignInManager _signInManager;
        private xISPUserManager _userManager;

        public UserController(xISPUserManager userManager, xISPSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public xISPSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.Current.GetOwinContext().Get<xISPSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

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

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().Authentication;
            }
        }
        #endregion

        // POST: /api/admin/login
        [AllowAnonymous]
        [Route("api/login")]
        [HttpPost]
        public async Task<ApiResult<bool>> Login(AccountViewModels.LoginViewModel model)
        {
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInPhoneOrEmail(model.PhoneOrEmail, model.Password, isPersistent: false, shouldLockout: false);
            if (result == SignInStatus.Success)
            {
                //get user
                //var id = int.Parse(HttpContext.Current.User.Identity.GetUserId());
                var id = UserManager.FindByNameOrEmailAsync (model.PhoneOrEmail).Result.Id;
                var user = Domain.Core.User.Get(_db, id);
                if(user != null && user.IsCustomer)
                {
                    return new ApiResult<bool> { Success = true };
                }
                else { 
                    return new ApiResult<bool> { Success = false};
                }
            }
            else {
                return new ApiResult<bool> { Success = false};
            }
        }

        [HttpPost]
        [Route("api/logout")]
        public void Logout()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        }

        [AllowAnonymous]
        [Route("api/signup")]
        [HttpPost]
        public async Task<ApiResult<bool>> SignUp(AccountViewModels.SignUpViewModel model)
        {
            try {
                //check email or phone not used
                var existCustomer = Customer.Get(_db, x => x.Email.Equals(model.Email) || x.Mobile.Equals(model.Mobile));
                if (existCustomer.Count > 0)
                {
                    return new ApiResult<bool> { Success = false, Message = "Email or Mobile has been registered" };
                }
                var existUser = await UserManager.FindByPhoneOrEmailAsync(model.Email);
                if (existUser != null)
                {
                    return new ApiResult<bool> { Success = false, Message = "Email has been registered" };
                }
                existUser = await UserManager.FindByPhoneOrEmailAsync(model.Mobile);
                if (existUser != null)
                {
                    return new ApiResult<bool> { Success = false, Message = "Mobile has been registered" };
                }

                //create customer

                var customer = new Customer()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Mobile = model.Mobile,
                    Email = model.Email,
                    SignUpDate = DateTime.Now
                };

                Customer.Create(_db, customer);
                if (customer.Id == 0)
                {
                    return new ApiResult<bool> { Success = false };
                }

                //create user
                xISPUser user = new xISPUser
                {
                    Id = customer.Id,
                    UserName = model.Email,
                    Email = model.Email,
                    PhoneNumber = model.Mobile
                };
                var rslt = UserManager.CreateAsync(user, model.Password).Result;
                if (!rslt.Succeeded)
                {
                    return new ApiResult<bool> { Success = false, Message = rslt.Errors.FirstOrDefault() };
                }

                //get user
                user = await UserManager.FindByPhoneOrEmailAsync(model.Email);
                if (user == null)
                {
                    return new ApiResult<bool> { Success = false };
                }

                //set role
                UserManager.AddToRole(user.Id, "Customer");

                //sign in
                var result = await SignInManager.PasswordSignInPhoneOrEmail(model.Email, model.Password, isPersistent: false, shouldLockout: false);
                if (result != SignInStatus.Success)
                {
                    return new ApiResult<bool> { Success = false };
                }

                return new ApiResult<bool> { Success = true };
            }
            catch(Exception ex)
            {
                return new ApiResult<bool> { Success = false };
            }
        }

        // POST: /api/admin/login
        [AllowAnonymous]
        [Route("api/user/current")]
        public ApiResult<User> GetCurrentUser()
        {
            //get user
            if(xISPUser.CurrentUser == null)
            {
                return new ApiResult<User> { Success = false, Data = null};
            }
            else
            { 
                return new ApiResult<User> { Success = true, Data = xISPUser.CurrentUser};
            }
        }
    }


}
