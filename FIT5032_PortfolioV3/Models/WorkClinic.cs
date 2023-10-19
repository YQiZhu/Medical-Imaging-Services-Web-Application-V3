using EllipticCurve.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Twilio.Http;

namespace FIT5032_PortfolioV3.Models
{
    public class WorkClinic
    {
        [Key]
        [Column(Order = 1)]
        public string ClinicId { get; set; }

        [Key]
        [Column(Order = 2)]
        public string StaffId { get; set; }

    }
}