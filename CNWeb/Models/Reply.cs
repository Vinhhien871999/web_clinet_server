namespace CNWeb.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Reply")]
    public partial class Reply
    {
        public int id { get; set; }

        public int? comment_id { get; set; }

        public int? is_active { get; set; }

        [StringLength(50)]
        public string author { get; set; }

        [StringLength(50)]
        public string content { get; set; }

        public string path_img { get; set; }

        public DateTime? create_at { get; set; }

        public DateTime? update_at { get; set; }

        public virtual Comment Comment { get; set; }
    }
}
