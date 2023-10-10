namespace FIT5032_PortfolioV3.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Clinics
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Clinics()
        {
            Appointments = new HashSet<Appointments>();
        }

        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string PhoneNo { get; set; }

        [Required]
        public string AddressDetail { get; set; }

        [Required]
        public string Postcode { get; set; }

        public string Description { get; set; }

        [Required]
        [Column(TypeName = "numeric")]
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90.")]
        public decimal Latitude { get; set; }

        [Required]
        [Column(TypeName = "numeric")]
        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180.")]
        public decimal Longitude { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Appointments> Appointments { get; set; }
    }
}
