using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TesteCobmais.Models;

    public class LogConsulta
    {
        [Key]
        public int Id { get; set; }
        public int ContratoId { get; set; }
        public DateTime ConsultaTimestamp{ get; set;}
        public int AtrasoEmDias { get; set;}
        public float ValorAtualizado { get; set;}
        public float DescontoMaximo { get; set; }
        
        [ForeignKey("ContratoId")]
        public Contrato contrato { get; set; }
    }

