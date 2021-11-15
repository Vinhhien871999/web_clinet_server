namespace CNWeb.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Receiver")]
    public partial class Receiver
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Receiver()
        {
            formPayments = new HashSet<formPayment>();
        }

        public int id { get; set; }

        public int? id_user { get; set; }

        [StringLength(50)]
        public string name { get; set; }

        [StringLength(13)]
        public string phone { get; set; }

        [StringLength(100)]
        public string address { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<formPayment> formPayments { get; set; }

        public virtual User User { get; set; }
    }
}
