using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Identity.MySQL;
using Microsoft.AspNet.Identity;
using System.Linq;
using CyberPark.Domain.Core;

namespace CyberPark.Website.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class xISPUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<xISPUser, int> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
        public static bool HasCustomerLogin
        {
            get
            {
                return CurrentUser?.Customer != null;
            }
        }
        public static bool HasStaffLogin
        {
            get
            {
                return CurrentUser?.Staff != null;
            }
        }
        public static int CurrentUserId
        {
            get
            {
                return int.Parse(System.Web.HttpContext.Current.User.Identity.GetUserId()??"0");
            }
        }

        public static User CurrentUser
        {
            get
            {
                using (var db = new xISPContext())
                {
                    return db.Users.Include(x => x.Staff).Include(x => x.Customer).SingleOrDefault(x => x.Id == CurrentUserId);
                }

                //User user = null;
                //if (System.Web.HttpContext.Current.Session != null)
                //{
                //    user = (User)System.Web.HttpContext.Current.Session["LOGIN_USER"];
                //}

                //if (user == null)
                //{
                //    using (var db = new xISPContext())
                //    {
                //        user = db.Users.Include(x => x.Staff).Include(x => x.Customer).SingleOrDefault(x => x.Id == CurrentUserId);
                //    }

                //    if (System.Web.HttpContext.Current.Session != null)
                //    {
                //        System.Web.HttpContext.Current.Session["LOGIN_USER"] = user;
                //    }
                //}

                //return user;
            }
        }
    }

    public class xISPDbContext : MySQLDatabase
    {
        public xISPDbContext(string connectionName)
            : base(connectionName)
        {
        }
        public static xISPDbContext Create()
        {
            return new xISPDbContext("CyberParkEntities");
        }
    }
}