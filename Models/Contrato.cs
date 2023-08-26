using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TesteCobmais.Models;

    public class Contrato
    {
        [Key]
        public int Id { get; set; }
        [Index(IsUnique = true)]
        public int DividaId { get; set; }
        public int ClienteId { get; set; }
        public DateTime Vencimento { get; set; }
        public double Valor { get; set; }
        
        //eu interpretei o tipo de contrato como podendo assumir infinitos valores diferentes
        //mas eu poderia usar um enum caso os tipos de contratos fossem finitos
        public string TipoContrato { get; set; }

        [ForeignKey("ClienteId")]
        public Cliente cliente { get; set; }
    }

