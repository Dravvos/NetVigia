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
        public string? URL { get; set; }
        public int CheckInterval { get; set; }
        public int ExpectedStatusCode { get; set; }
        public bool Active { get; set; }
    }
}
