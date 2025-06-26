using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetVigia.Data.Models
{
    [Table("Maintenances")]
    public class MaintenanceModel:BaseModel
    {
        [Required]
        [Column("Title")]
        public string? Title { get; set; }

        [Required]
        [Column("StartDate")]
        public DateTime StartDate { get; set; }

        [Required]
        [Column("EndDate")]
        public DateTime EndDate { get; set; }

        [Required]
        [Column("UserId")]
        public Guid UserId { get; set; }
        
        public virtual List<ServerModel> Servers { get; set; } = new List<ServerModel>();
    }
}
