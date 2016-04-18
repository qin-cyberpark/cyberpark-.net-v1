namespace CyberPark.Domain.Core
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class xISPContext : DbContext
    {
        public xISPContext()
            : base("name=CyberParkEntities")
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }
        //login
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Staff> Staffs { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        
        //account
        public virtual DbSet<UserClaim> UserClaims { get; set; }
        public virtual DbSet<UserLogin> UserLogins { get; set; }

        //management
        public virtual DbSet<PstnCallingRate> PstnCallingRates { get; set; }
        public virtual DbSet<VoipCallingRate> VoipCallingRates { get; set; }
        public virtual DbSet<CallingInternationalRegion> CallingInternationalRegions { get; set; }
        public virtual DbSet<CallingInternationalRegionDetail> CallingInternationalRegionDetails { get; set; }
        public virtual DbSet<ExternalBill> ExternalBills { get; set; }
        public virtual DbSet<Warning> Warnings { get; set; }
        public virtual DbSet<Branch> Branches { get; set; }
        public virtual DbSet<ExternalAddOnConverter> ExternalAddOnConverters { get; set; }
        public virtual DbSet<EquipmentModel> EquipmentModels { get; set; }
        public virtual DbSet<Equipment> Equipments { get; set; }
        public virtual DbSet<Plan> Plans { get; set; }
        public virtual DbSet<Ticket> Tickets { get; set; }
        public virtual DbSet<TicketComment> TicketComments { get; set; }

        //product
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<ServiceUsageOffer> ServiceUsageOffers { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<CallingRecord> CallingRecords { get; set; }


        //invoice
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<ProductCharge> ProductCharges { get; set; }
        public virtual DbSet<AddonCharge> AddonCharges { get; set; }
        public virtual DbSet<CallingCharge> CallingCharges { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<Adjustment> Adjustments { get; set; }
        public virtual DbSet<VoidInvoice> VoidInvoices { get; set; }

        //system

    }
}
