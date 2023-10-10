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

        public string RoomNo { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public string Date { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public string Time { get; set; }
        public string AppointmentDateTime
        {
            get { return "Appointment at "+Date + " " + Time; }
        }

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

        public virtual AspNetUsers StaffId { get; set; }

        public virtual AspNetUsers PatientId { get; set; }
    }
}
