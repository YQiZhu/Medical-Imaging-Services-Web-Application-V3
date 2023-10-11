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

        public string Description { get; set; }

        [Display(Name = "Appointment room no")]
        public string RoomNo { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public string Date { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public string Time { get; set; }
        public string AppointmentDateTime
        {
            get { return "Appointment "+Date + " " + Time + " at "+ Clinics.Name; }
        }

        [Required]
        [StringLength(128)]
        public string ClinicId { get; set; }

        [Required]
        [Display(Name = "Patient Id")]
        [StringLength(128)]
        public string PatientUserId { get; set; }

        [Required]
        [Display(Name = "Staff Id")]
        [StringLength(128)]
        public string StaffUserId { get; set; }

        public virtual Clinics Clinics { get; set; }

        [Display(Name = "Staff Name")]
        public virtual AspNetUsers StaffId { get; set; }

        [Display(Name = "Patient Name")]
        public virtual AspNetUsers PatientId { get; set; }
    }
}
