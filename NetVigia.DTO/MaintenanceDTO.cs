using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetVigia.DTO
{
    public class MaintenanceDTO:BaseDTO
    {
        public string? Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid UserId { get; set; }
        public List<ServerDTO> Servers { get; set; } = new List<ServerDTO>();
    }
}
