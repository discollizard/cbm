using CsvHelper.Configuration.Attributes;
using Humanizer.Localisation.TimeToClockNotation;

namespace TesteCobmais.CSV_Classes
{
    public class CSVImportTemplate
    {
        public long CPF { get;set; }
        public string CLIENTE { get;set; }
        public string CONTRATO { get;set; }
        //vai ser lido como string, transformado para date dps
        public string VENCIMENTO { get;set; }
        public double VALOR { get;set; }
        [Name("TIPO DE CONTRATO")]
        public string TipoDeContrato { get; set; } 
    }
}
