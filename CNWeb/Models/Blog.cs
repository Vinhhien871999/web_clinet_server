namespace CNWeb.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Blog")]
    public partial class Blog
    {
        public int id { get; set; }

        public int? user_id { get; set; }

        [StringLength(50)]
        public string title { get; set; }

        public string photo { get; set; }

        [StringLength(50)]
        public string body { get; set; }

        public DateTime? create_at { get; set; }

        public DateTime? update_at { get; set; }

        public virtual User User { get; set; }
    }
}
