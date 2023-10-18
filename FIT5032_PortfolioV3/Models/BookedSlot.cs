using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FIT5032_PortfolioV3.Models
{
    public class BookedSlot
    {
        [Key]
        public String BookingId { get; set; }
        public String SlotId { get; set; }
        public string StaffUserId { get; set; }
        public String Date { get; set; }

        [ForeignKey("SlotId")]
        public TimeSlot TimeSlot { get; set; }
    }
}