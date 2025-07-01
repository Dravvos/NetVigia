using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetVigia.Data.Models
{
    [Table("Integrations")]
    public class IntegrationUserModel:BaseModel
    {
        [Required]
        [Column("IdTGIntegrationMethod")]
        public Guid IdTGIntegrationMethod { get; set; } //WEBHOOK, DISCORD, TELEGRAM, etc...
        
        [Required]
        [Column("IntegrationName")]
        public string? IntegrationName { get; set; }
        
        [Required]
        [Column("IntegrationEndpoint")]
        public string? IntegrationEndpoint { get; set; }

        [Required]
        [Column("IdTGSendNotification")]
        public Guid IdTGSendNotification { get; set; } //Instantly, after 1 minute, after 1 hour...

        [Required]
        [Column("IdTGTypeNotification")]
        public Guid IdTGTypeNotification { get; set; } //Success, Error or All

        [Required]
        [Column("Active")]
        public bool Active { get; set; }

        [Required]
        [Column("UserId")]
        public Guid UserId { get; set; }


        [ForeignKey("IdTGIntegrationMethod")]
        public virtual TabelaGeralItemModel? IntegrationMethod { get; set; }
        [ForeignKey("IdTGSendNotification")]
        public virtual TabelaGeralItemModel? SendNotification { get; set; }
        [ForeignKey("IdTGTypeNotification")]
        public virtual TabelaGeralItemModel? TypeNotification { get; set; }

        public virtual List<ServerModel> Servers { get; set; } = new List<ServerModel>();
    }
}
