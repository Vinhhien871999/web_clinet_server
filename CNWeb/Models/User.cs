namespace CNWeb.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("User")]
    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            Blogs = new HashSet<Blog>();
            dbo_About = new HashSet<dbo_About>();
            formPayments = new HashSet<formPayment>();
            Receivers = new HashSet<Receiver>();
            Permissions = new HashSet<Permission>();
        }

        public int id { get; set; }

        public string photo { get; set; }

        public int? is_active { get; set; }

        public int? id_type { get; set; }

        [StringLength(50)]
        public string name { get; set; }

        public int cart_id { get; set; }

        [StringLength(50)]
        public string email { get; set; }

        [StringLength(50)]
        public string passwork { get; set; }

        [StringLength(50)]
        public string phone { get; set; }

        [StringLength(100)]
        public string address { get; set; }

        public DateTime create_at { get; set; }

        public DateTime? update_at { get; set; }

        public int? point { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Blog> Blogs { get; set; }

        public virtual Cart Cart { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dbo_About> dbo_About { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<formPayment> formPayments { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Receiver> Receivers { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Permission> Permissions { get; set; }
    }
}
