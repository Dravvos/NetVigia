using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetVigia.Data.Models
{
    public class BaseModel
    {
        [Required]
        [Key]
        [Column("Id")]
        public Guid Id { get; set; }

        [Required]
        [Column("DataInclusao")]
        public DateTime DataInclusao { get; set; }
        
        [Required]
        [Column("UsuarioInclusao")]
        public string? UsuarioInclusao { get; set; }

        [Column("DataAlteracao")]
        public DateTime? DataAlteracao { get; set; }

        [Column("UsuarioAlteracao")]
        public string? UsuarioAlteracao { get; set; }
    }
}
