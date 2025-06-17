using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetVigia.DTO
{
    public class ServerDTO : BaseDTO
    {
        public string? Name { get; set; }
        public string? URL { get; set; }
        public int CheckIntervalSeconds { get; set; } = 300;
        public int ExpectedStatusCode { get; set; } = 200;
        public bool Active { get; set; } = true;
        public int TimeoutInSeconds { get; set; } = 10;
        public string? ExpectedContent { get; set; }
        public Guid IdTGMonitoringType { get; set; }
        public Guid UserId { get; set; }
        public TabelaGeralItemDTO? MonitoringType { get; set; }
    }
}
