using Newtonsoft.Json;

namespace TesteCobmais.DTO
{
    public class CalculoResponseBody
    {
        public CalculoResponseBody(string responseJson) {
            try
            {
                Dictionary<string, object>? decodedJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseJson);   

                if(decodedJson == null ) {
                    throw new Exception();
                }

                if (decodedJson.TryGetValue("TipoContrato", out object tipoContratoObj) && tipoContratoObj is string tipoContrato)
                {
                    this.TipoContrato = tipoContrato;
                }

                if (decodedJson.TryGetValue("Atraso", out object atrasoObj) && atrasoObj is long atrasoLong)
                {
                    this.Atraso = (int)atrasoLong; 
                }

                if (decodedJson.TryGetValue("Valor", out object valorObj) && valorObj is double valorDouble)
                {
                    this.Valor = valorDouble;
                }

                if (decodedJson.TryGetValue("ValorAtualizado", out object valorAtualizadoObj) && valorAtualizadoObj is double valorAtualizadoDouble)
                {
                    this.ValorAtualizado = valorAtualizadoDouble;
                }

                if (decodedJson.TryGetValue("DescontoMaximo", out object descontoMaximoObj) && descontoMaximoObj is double descontoMaximoDouble)
                {
                    this.DescontoMaximo = descontoMaximoDouble;
                }


            }
            catch(Exception e){ 
                this.IsValid = false;
                Console.WriteLine("\n\n Erro ao transformar JSON retornado da API em um objeto DTO: "+responseJson + "\n\n" +e.Message+"\n\n");
            }
        }

        public bool IsValid { get; set; }

        public string TipoContrato { get; set; }
        public int Atraso { get; set; }
        public double Valor { get; set; }
        public double ValorAtualizado { get; set; }
        public double DescontoMaximo { get; set; }
    }
}
