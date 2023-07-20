using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BusinessData
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("LocalConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Franchise> Franchises { get; set; }
        public DbSet<NationalAccount> NationalAccounts { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<WorkOrder> WorkOrders { get; set; }
        public DbSet<WOCharges> Charges { get; set; }
        public DbSet<TestClient> TestClients { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder
                .Conventions
                .Remove<PluralizingTableNameConvention>();

            modelBuilder
                .Configurations
                .Add(new IdentityUserLoginConfiguration())
                .Add(new IdentityUserRoleConfiguration());

            modelBuilder.Entity<Franchise>()
                .HasMany(f => f.Clients)
                .WithRequired(c => c.Franchise)
                .HasForeignKey(c => c.FranchiseId)
                .WillCascadeOnDelete(false);  // Disables cascade delete for Clients when Franchise is deleted.

            modelBuilder.Entity<Client>()
                .HasMany(c => c.Locations)
                .WithRequired(l => l.Client)
                .HasForeignKey(l => l.ClientId)
                .WillCascadeOnDelete(false);  // Disables cascade delete for Locations when Client is deleted.

            modelBuilder.Entity<Contact>()
                .HasRequired(t => t.Location) // Contact must have a Location.
                .WithMany(l => l.Contacts) // A Location can have many Contacts.
                .HasForeignKey(t => t.LocationId) // LocationId is the foreign key.
                .WillCascadeOnDelete(false); // Turn off cascade delete.

            modelBuilder.Entity<Invoice>()
                .HasRequired(i => i.Client) // Invoice must have a Client.
                .WithMany(c => c.Invoices) // A Client can have many Invoices.
                .HasForeignKey(i => i.ClientId) // ClientId is the foreign key.
                .WillCascadeOnDelete(false); // Turn off cascade delete.

            modelBuilder.Entity<Invoice>()
                .HasRequired(i => i.Franchise) // Invoice must have a Franchise.
                .WithMany(f => f.Invoices) // A Franchise can have many Invoices.
                .HasForeignKey(i => i.FranchiseId) // FranchiseId is the foreign key.
                .WillCascadeOnDelete(false); // Turn off cascade delete.

            modelBuilder.Entity<WorkOrder>()
                .HasOptional(wo => wo.Invoice)
                .WithMany(i => i.WorkOrders) // An Invoice can have many WorkOrders.
                .HasForeignKey(wo => wo.InvoiceId) // InvoiceId is the foreign key.
                .WillCascadeOnDelete(false); // Turn off cascade delete.

            modelBuilder.Entity<WorkOrder>()
                .HasRequired(wo => wo.Franchise) // WorkOrder must have a Franchise.
                .WithMany(f => f.WorkOrders) // A Franchise can have many WorkOrders.
                .HasForeignKey(wo => wo.FranchiseId) // FranchiseId is the foreign key.
                .WillCascadeOnDelete(false); // Turn off cascade delete.

            modelBuilder.Entity<WOCharges>()
                .HasRequired(woc => woc.Franchise) // WOCharges must have a Franchise.
                .WithMany(f => f.WOCharges) // A Franchise can have many WOCharges.
                .HasForeignKey(woc => woc.FranchiseId) // FranchiseId is the foreign key.
                .WillCascadeOnDelete(false); // Turn off cascade delete.


            base.OnModelCreating(modelBuilder);
        }



    }

    public class IdentityUserLoginConfiguration : EntityTypeConfiguration<IdentityUserLogin>
    {
        public IdentityUserLoginConfiguration()
        {
            HasKey(iul => iul.UserId);
        }
    }

    public class IdentityUserRoleConfiguration : EntityTypeConfiguration<IdentityUserRole>
    {
        public IdentityUserRoleConfiguration()
        {
            HasKey(iur => iur.UserId);
        }
    }
}