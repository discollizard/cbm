using System.ComponentModel.DataAnnotations;

namespace TesteCobmais.Models;
    public class Cliente
    {
        [Key]
        public int Id { get; set; }
        public long CPF { get; set; }
        public string NomeCompleto { get; set; }

    }

