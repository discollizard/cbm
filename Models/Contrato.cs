using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TesteCobmais.Models;

    public class Contrato
    {
        [Key]
        public int Id { get; set; }
        [Index(IsUnique = true)]
        [MaxLength(100)]
        [Display(Name = "Id da dívida")]
        public string DividaId { get; set; }
        [Display(Name = "Id do cliente")]
        public int ClienteId { get; set; }
        [Display(Name = "Data de vencimento")]
        public DateTime Vencimento { get; set; }
        [Display(Name = "Valor original")]
        public double Valor { get; set; }
        
        //eu interpretei o tipo de contrato como podendo assumir infinitos valores diferentes
        //mas eu poderia usar um enum caso os tipos de contratos fossem finitos
        [Display(Name = "Tipo de contrato")]
        public string TipoContrato { get; set; }

        [ForeignKey("ClienteId")]
        public Cliente cliente { get; set; }

        public string VencimentoFormatado
        {
            get { return Vencimento.ToString("dd/MM/yyyy"); }
        }

    public static int CalcularVencimento(DateTime vencimento)
        {
            DateTime hoje = DateTime.Today;
            TimeSpan atraso = hoje - vencimento;
            int atrasoEmDias = atraso.Days;
            return atrasoEmDias;
        }
    }

