namespace CNWeb.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Comment")]
    public partial class Comment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Comment()
        {
            Replies = new HashSet<Reply>();
        }

        public int id { get; set; }

        public int? product_id { get; set; }

        public int? is_active { get; set; }

        [StringLength(50)]
        public string author { get; set; }

        public string content { get; set; }

        public string path_img { get; set; }

        public DateTime? create_at { get; set; }

        public DateTime? update_at { get; set; }

        public int? rating { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Reply> Replies { get; set; }

        public virtual Product Product { get; set; }
    }
}
