namespace FIT5032_PortfolioV3.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Appointments
    {
        public string Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string RoomNo { get; set; }

        [Required]
        public string Date { get; set; }

        [Required]
        public string Time { get; set; }

        [Required]
        [StringLength(128)]
        public string ClinicId { get; set; }

        [Required]
        [StringLength(128)]
        public string PatientUserId { get; set; }

        [Required]
        [StringLength(128)]
        public string StaffUserId { get; set; }

        public virtual Clinics Clinics { get; set; }

        public virtual AspNetUsers AspNetUsers { get; set; }

        public virtual AspNetUsers AspNetUsers1 { get; set; }
    }
}
