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
        [Column("Name")]
        public string? Name { get; set; }

        [Required]
        [Column("URL")]
        public string? URL { get; set; }

        [Required]
        [Column("CheckIntervalSeconds")]
        [Range(1,int.MaxValue)]
        public int CheckIntervalSeconds { get; set; }

        [Required]
        [Column("ExpectedStatusCode")]
        public int ExpectedStatusCode { get; set; }
        
        [Column("ExpectedConten")]
        public string? ExpectedContent { get; set; }

        [Required]
        [Column("Active")]
        public bool Active { get; set; }

        [Required]
        [Column("TimeoutInSeconds")]
        [Range(1, int.MaxValue)]
        public int TimeoutInSeconds { get; set; } = 10;

        [Required]
        [Column("IdTGMonitoringType ")]
        public Guid IdTGMonitoringType { get; set; }

        [Column("IdTGHTTPMethod")]
        public Guid? IdTGHTTPMethod { get; set; }

        [Column("Port")]
        public int Port { get; set; }

        [Required]
        [Column("UserId")]
        public Guid UserId { get; set; }


        [ForeignKey("IdTGMonitoringType")]
        public virtual TabelaGeralItemModel? MonitoringType { get; set; }

        [ForeignKey("IdTGHTTPMethod")]
        public virtual TabelaGeralItemModel? HTTPMethod { get; set; }
    }
}
