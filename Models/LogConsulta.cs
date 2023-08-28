using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TesteCobmais.Models;

    public class LogConsulta
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(100)]
        [Display(Name = "Id do contrato")]
        public int ContratoId { get; set; }
        [Display(Name = "Data e hora da consulta")]
        public DateTime ConsultaTimestamp{ get; set;}
        [Display(Name = "Atraso em dias")]
        public int? AtrasoEmDias { get; set;}
        [Display(Name = "Valor atualizado")]
        public double? ValorAtualizado { get; set;}
        [Display(Name = "Desconto máximo")]
        public double? DescontoMaximo { get; set; }
        
        [ForeignKey("ContratoId")]
        public Contrato contrato { get; set; }
    }

