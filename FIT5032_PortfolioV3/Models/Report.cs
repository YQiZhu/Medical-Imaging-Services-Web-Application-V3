using FIT5032_PortfolioV3.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FIT5032_PortfolioV3.Models
{
    public class Report
    {
        [Required]
        public String Description
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

        [DataType(DataType.Time)]
        [Required]
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
