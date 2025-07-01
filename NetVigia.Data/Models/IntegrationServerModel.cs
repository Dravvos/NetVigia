using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetVigia.Data.Models
{
    public class IntegrationServerModel:BaseModel
    {
        public Guid IntegrationId { get; set; } //FK to IntegrationUserModel
        public Guid ServerId { get; set; } //FK to ServerModel

        [ForeignKey("IntegrationId")]
        public virtual IntegrationUserModel? Integration { get; set; }
        [ForeignKey("ServerId")]
        public virtual ServerModel? Server { get; set; }

        public IntegrationServerModel()
        {
            Integration = new IntegrationUserModel();
            Server = new ServerModel();
        }
    }
}
