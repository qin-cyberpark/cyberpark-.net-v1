using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using AspNet.Identity.MySQL;
using CyberPark.Website.Models;

namespace CyberPark.Website
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }

    //public class xISPUserValidator : UserValidator<xISPUser, int>
    //{

    //}

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class xISPUserManager : UserManager<xISPUser, int>
    {
        public xISPUserManager(UserStore<xISPUser> store)
            : base(store)
        {
            UserStore = store;
        }

        protected UserStore<xISPUser> UserStore
        {
            get; private set;
        }

        public static xISPUserManager Create(IdentityFactoryOptions<xISPUserManager> options, IOwinContext context)
        {
            var manager = new xISPUserManager(
                new UserStore<xISPUser>(
                context.Get<xISPDbContext>() as MySQLDatabase));
            
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<xISPUser, int>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<xISPUser, int>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<xISPUser, int>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<xISPUser, int>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;

        }

        public Task<xISPUser> FindByNameOrEmailAsync(string nameOrEmail)
        {
            return UserStore.FindByNameOrEmailAsync(nameOrEmail);
        }

        public Task<xISPUser> FindByPhoneOrEmailAsync(string phoneOrEmail)
        {
            return UserStore.FindByPhoneOrEmailAsync(phoneOrEmail);
        }
        
    }

    // Configure the application sign-in manager which is used in this application.
    public class xISPSignInManager : SignInManager<xISPUser, int>
    {
        public xISPSignInManager(xISPUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
            xISPUserManager = userManager;
        }

        protected xISPUserManager xISPUserManager { get; private set; }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(xISPUser user)
        {
            return user.GenerateUserIdentityAsync((xISPUserManager)UserManager);
        }

        public static xISPSignInManager Create(IdentityFactoryOptions<xISPSignInManager> options, IOwinContext context)
        {
            return new xISPSignInManager(context.GetUserManager<xISPUserManager>(), context.Authentication);
        }

        public Task<SignInStatus> PasswordSignInPhoneOrEmail(string phoneOrEmail, string password, bool isPersistent, bool shouldLockout)
        {
            var user = xISPUserManager.FindByPhoneOrEmailAsync(phoneOrEmail).Result;
            if(user == null)
            {
                return Task<SignInStatus>.Factory.StartNew(() => SignInStatus.Failure);
            }

            return PasswordSignInAsync(user.UserName, password, isPersistent, shouldLockout);
        }
    }

    public class xISPRoleManager : RoleManager<IdentityRole>
    {
        public xISPRoleManager(IRoleStore<IdentityRole, string> roleStore)
            : base(roleStore)
        {
        }

        public static xISPRoleManager Create(IdentityFactoryOptions<xISPRoleManager> options, IOwinContext context)
        {
            var appRoleManager = new xISPRoleManager(new RoleStore<IdentityRole>(context.Get<xISPDbContext>()));

            return appRoleManager;
        }
    }
}
