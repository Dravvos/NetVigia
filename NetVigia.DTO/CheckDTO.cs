using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetVigia.DTO
{
    public class CheckDTO
    {
        public Guid ServerId { get; set; }
        public bool Up { get; set; }
        public int StatusCode { get; set; }
        public float ResponseTimeInMs { get; set; }
        public DateTime Timestamp { get; set; }
        public string? ErrorMessage { get; set; }

        public ServerDTO? Server { get; set; }
    }

}
