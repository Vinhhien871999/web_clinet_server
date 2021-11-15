namespace CNWeb.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Product")]
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            Cart_detail = new HashSet<Cart_detail>();
            Comments = new HashSet<Comment>();
            detailPayments = new HashSet<detailPayment>();
            Colors = new HashSet<Color>();
            Sizes = new HashSet<Size>();
        }

        public int id { get; set; }

        [StringLength(50)]
        public string product_name { get; set; }

        public int? product_category_id { get; set; }

        public string description { get; set; }

        public int price { get; set; }

        public int? stock { get; set; }

        public int? rating { get; set; }

        public double? discount { get; set; }

        [StringLength(50)]
        public string create_by { get; set; }

        public int? status { get; set; }

        public string photo { get; set; }

        public DateTime? update_at { get; set; }

        public int? viewCount { get; set; }

        public int? sex { get; set; }

        public int score { get; set; }

        public DateTime? create_at { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cart_detail> Cart_detail { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment> Comments { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<detailPayment> detailPayments { get; set; }

        public virtual Product_category Product_category { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Color> Colors { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Size> Sizes { get; set; }
    }
}
