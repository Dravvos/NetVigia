using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetVigia.Data.Models
{
    public class ServerModel:BaseModel
    {
        [Required]
        [Column("URL")]
        public string? URL { get; set; }

        [Required]
        [Column("CheckInterval")]
        [Range(1,int.MaxValue)]
        public int CheckInterval { get; set; }

        [Required]
        [Column("ExpectedStatusCode")]
        public int ExpectedStatusCode { get; set; }

        [Required]
        [Column("Active")]
        public bool Active { get; set; }

        [Required]
        [Column("UserId")]
        public Guid UserId { get; set; }
    }
}
