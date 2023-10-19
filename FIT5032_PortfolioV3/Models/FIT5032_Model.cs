using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace FIT5032_PortfolioV3.Models
{
    public partial class FIT5032_Model : DbContext
    {
        public FIT5032_Model()
            : base("name=FIT5032_Model")
        {
            this.Configuration.LazyLoadingEnabled = true;
            this.Configuration.ProxyCreationEnabled = true;
        }

        public virtual DbSet<Appointments> Appointments { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<Clinics> Clinics { get; set; }
        public virtual DbSet<MedImage> MedImages { get; set; }
        public virtual DbSet<Rating> Ratings { get; set; }
        public virtual DbSet<Report> Reports { get; set; }
        public virtual DbSet<TimeSlot> TimeSlots { get; set; }
        public virtual DbSet<BookedSlot> BookedSlots { get; set; }
        public virtual DbSet<WorkClinic> WorkClinic { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRoles>()
                .HasMany(e => e.AspNetUsers)
                .WithMany(e => e.AspNetRoles)
                .Map(m => m.ToTable("AspNetUserRoles").MapLeftKey("RoleId").MapRightKey("UserId"));

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.Appointments)
                .WithRequired(e => e.PatientId)
                .HasForeignKey(e => e.PatientUserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.Appointments1)
                .WithRequired(e => e.StaffId)
                .HasForeignKey(e => e.StaffUserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.AspNetUserLogins)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<Clinics>()
                .HasMany(e => e.Appointments)
                .WithRequired(e => e.Clinics)
                .HasForeignKey(e => e.ClinicId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Clinics>()
                .Property(e => e.Latitude)
                .HasPrecision(10, 8);

            modelBuilder.Entity<Clinics>()
                .Property(e => e.Longitude)
                .HasPrecision(11, 8);
        }
    }
}
