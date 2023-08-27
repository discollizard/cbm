using Newtonsoft.Json;
using System.Text;
using TesteCobmais.DTO;

namespace TesteCobmais.Helpers
{
    public class API
    {
        public string BaseUrl = "https://api.cobmais.com.br/";
        public string Endpoint;
        public string Payload;

        //essa classe ta um pouco menos parametrizada do que eu faria em um caso mais generalizável
        //mas como o escopo do teste é menor, eu preferi deixar as coisas mais estáticas
        public async Task<string?> Call()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpContent content = new StringContent(Payload, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(BaseUrl+Endpoint, content);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception(response.StatusCode.ToString());
                    }

                    string responseBodyJson = await response.Content.ReadAsStringAsync();
                    return responseBodyJson;
                }
            }
            catch (Exception e)
            {
                return null;
            }

        }
    }
}
