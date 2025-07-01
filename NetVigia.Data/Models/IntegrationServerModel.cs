using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetVigia.Data.Models
{
    [Table("IntegrationServers")]
    public class IntegrationServerModel:BaseModel
    {
        [Column("IntegrationId")]
        [Required]
        public Guid IntegrationId { get; set; }

        [Required]
        [Column("ServerId")]
        public Guid ServerId { get; set; }


        [ForeignKey("IntegrationId")]
        public virtual IntegrationUserModel? Integration { get; set; }

        [ForeignKey("ServerId")]
        public virtual ServerModel? Server { get; set; }

    }
}
