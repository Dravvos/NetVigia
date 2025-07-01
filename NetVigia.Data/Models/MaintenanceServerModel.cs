using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetVigia.Data.Models
{
    [Table("MaintenanceServers")]
    public class MaintenanceServerModel:BaseModel
    {
        [Required]
        [Column("MaintenanceId")]
        public Guid MaintenanceId { get; set; }

        [Required]
        [Column("ServerId")]
        public Guid ServerId { get; set; }


        [ForeignKey("MaintenanceId")]
        public virtual MaintenanceModel? Maintenance { get; set; }
        
        [ForeignKey("ServerId")]
        public virtual ServerModel? Server { get; set; }
    }
}
