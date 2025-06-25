using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetVigia.DTO
{
    public class IntegrationDTO:BaseDTO
    {
        public Guid IdTGIntegrationMethod { get; set; } //WEBHOOK, DISCORD, TELEGRAM, etc...
        public string? IntegrationName { get; set; }
        public string? IntegrationEndpoint { get; set; }
        public Guid UserId { get; set; }
        public TabelaGeralItemDTO? IntegrationMethod { get; set; }
    }
}
