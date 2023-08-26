using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TesteCobmais.Models;

    public class LogConsulta
    {
        [Key]
        public int Id { get; set; }
        public int DividaId { get; set; }
        [Display(Name = "Data e hora da consulta")]
        public DateTime ConsultaTimestamp{ get; set;}
        [Display(Name = "Atraso em dias")]
        public int AtrasoEmDias { get; set;}
        [Display(Name = "Valor atualizado")]
        public float ValorAtualizado { get; set;}
        [Display(Name = "Desconto máximo")]
        public float DescontoMaximo { get; set; }
        
        [ForeignKey("DividaId")]
        public Contrato contrato { get; set; }
    }

