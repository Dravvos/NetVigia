using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetVigia.DTO
{
    public class CheckDTO
    {
        public string? URL { get; set; }
        public bool Up { get; set; }
        public int StatusCode { get; set; }
        public float Latency { get; set; }
        public DateTime Timestamp { get; set; }
    }

}
