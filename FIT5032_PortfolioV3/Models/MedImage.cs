using FIT5032_PortfolioV3.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FIT5032_PortfolioV3.Models
{
    public class MedImage
    {

        [Required]
        [Display(Name = "Appointment Id")]
        public String Id
        {
            get;
            set;
        }

        [Required(ErrorMessage = "Please Upload Image")]
        [Display(Name = "X-ray Image")]
        [StringLength(50)]
        public string Path { get; set; }

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
        [Display(Name = "Appointment Id")]
        public String AppointmentId
        {
            get;
            set;
        }
        [ForeignKey("AppointmentId")]
        public Appointments Appointment { get; set; }
    }
}