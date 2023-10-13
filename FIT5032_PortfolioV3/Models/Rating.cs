using FIT5032_PortfolioV3.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FIT5032_PortfolioV3.Models
{
    public class Rating
    {
        public String Description
        {
            get;
            set;
        }

        [Required]
        [Display(Name = "Rate your Appointment from 1 (bad) to 5 (Very Good)")]
        [Range(1, 5, ErrorMessage = "Rate your Appointment from 1 (bad) to 5 (Very Good)")]
        public int Rate
        {
            get;
            set;
        }

        [Required]
        public String Id
        {
            get;
            set;
        }

        [Required]
        [DataType(DataType.Date)]
        public String Date
        {
            get;
            set;
        }

        [Required]
        [DataType(DataType.Time)]
        public String Time
        {
            get;
            set;
        }

        [Required]
        public String AppointmentId
        {
            get;
            set;
        }
        [ForeignKey("AppointmentId")]
        public Appointments Appointment { get; set; }
    }
}