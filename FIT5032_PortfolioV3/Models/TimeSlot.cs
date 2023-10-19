using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FIT5032_PortfolioV3.Models
{
    public class TimeSlot
    {
        [Key]
        public String SlotId { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        [Display(Name = "Appointment Time")]
        public string Name { get { return StartTime.ToString() + " - " + EndTime.ToString(); } }

        public virtual ICollection<BookedSlot> BookedSlots { get; set; }
    }
}