using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TesteCobmais.Models;
    public class Cliente
    {
        [Key]
        public int Id { get; set; }
        [Index(IsUnique = true)]
        public long CPF { get; set; }
        public string NomeCompleto { get; set; }

        public ICollection<Contrato> Contratos { get; set; }

    }

