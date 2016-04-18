namespace CyberPark.Domain.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Linq;

    public class WarningModules
    {
        public const string ExternalBill = "ExternalBill";
        public const string Invoice = "Invoice";
        public const string ServiceCharge = "ServiceCharge";
    }

    [Table("warnings")]
    public partial class Warning
    {
        private Warning()
        {

        }
        public Warning(string module, string opt, string msg)
        {
            Id = Guid.NewGuid().ToString();
            Module = module;
            Operate = opt;
            Message = msg;
            CreateDate = DateTime.Now;
        }

        public string Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Module { get; set; }

        [Required]
        [StringLength(50)]
        public string Operate { get; set; }

        [Required]
        [StringLength(255)]
        public string Message { get; set; }

        public int? CustomerId { get; set; }
        public int? AccountId { get; set; }
        public int? UserId { get; set; }

        public DateTime CreateDate { get; set; }
        public int? ClearBy { get; set; }
        public DateTime? ClearDate { get; set; }

        public static void WriteAsync(string module, string opt, string msg, int? userId = null, int? customerId = null, int? accountId = null)
        {
            var w = new Warning(module, opt, msg)
            {
                UserId = userId,
                CustomerId = customerId,
                AccountId = accountId
            };

            using(var db = new xISPContext())
            {
                db.Warnings.Add(w);
                db.SaveChangesAsync();
            }

            return;
        }

        public static IList<Warning> Get(xISPContext db)
        {
            return db.Warnings.Where(x => x.ClearBy == null).OrderByDescending(x => x.CreateDate).ToList();
        }

        public static bool Clear(xISPContext db, string id, int userId)
        {
            var w = db.Warnings.SingleOrDefault(x => x.Id.Equals(id));
            if (w == null)
            {
                return false;
            }
            w.ClearBy = userId;
            w.ClearDate = DateTime.Now;
            db.SaveChanges();
            w = db.Warnings.SingleOrDefault(x => x.Id.Equals(id));
            return w.ClearBy == userId;
        }
    }
}