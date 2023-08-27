using CsvHelper.Configuration.Attributes;
using System.ComponentModel.DataAnnotations;

namespace TesteCobmais.CSV_Classes
{
    public class CSVExportTemplate
    {
        public long CPF { get; set; }
        public string DATA { get; set; }
        [MaxLength(100)]
        public string CONTRATO { get; set; }
        [Name("VALOR ORIGINAL")]
        public double ValorOriginal { get; set; }
        [Name("VALOR ATUALIZADO")]
        public double ValorAtualizado { get; set; }
        [Name("VALOR DESCONTO")]
        public double ValorDesconto { get; set; }
    }
}
