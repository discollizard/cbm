using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Protocol;
using TesteCobmais.CSV_Classes;
using TesteCobmais.Models;
using TesteCobmais.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.SqlServer.Server;

namespace TesteCobmais.Controllers
{
    public class ConsultasController : Controller
    {
        private readonly TesteCobmaisDbContext _context;

        public ConsultasController(TesteCobmaisDbContext context)
        {
            _context = context;
        }

        // GET: Consultas
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.LogConsultas.Include(l => l.contrato);
            return View(await applicationDbContext.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult>? ProcessarArquivoCSV()
        {
            var config = new CsvConfiguration(new CultureInfo("pt-BR"))
            {
                NewLine = Environment.NewLine,
                Delimiter = ";"
            };

            StreamReader reader = new StreamReader("Arquivos CSV/dividas-originais_teste_cobmais_2021.csv");


            using (var csv = new CsvReader(reader, config))
            {

                var records = csv.GetRecords<CSVImportTemplate>();

                Dictionary<long, int> relacaoIdClienteCPF = _context.Clientes
                .Select(cliente => new {cliente.Id, cliente.CPF})
                .ToDictionary(Cliente => Cliente.CPF, Cliente => Cliente.Id);

                var ContratosExistentes = _context.Contratos
                .Select(l => l.DividaId)
                .OrderBy(DividaId => DividaId)
                .ToList();

                //declarando timestamp de hoje fora do loop para evitar redeclarações desnecessarias
                DateTime hoje = DateTime.Today;

                //cacheando os clientes adicionados para não ter clientes duplicados no banco
                List<long> CPFsAdicionadosAgora = new List<long>();

                foreach(var record in records)
                {

                    //parse da data de vencimento
                    DateTime dataVencimento;
                    DateTime.TryParseExact(record.VENCIMENTO, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dataVencimento);

                    try
                    {
                        if (!relacaoIdClienteCPF.ContainsKey(record.CPF) && !CPFsAdicionadosAgora.Contains(record.CPF))
                        {
                            //registrando cliente novo
                            Cliente cliente = new Cliente();
                            cliente.CPF = record.CPF;
                            cliente.NomeCompleto = record.CLIENTE;
                            _context.Add(cliente);

                            CPFsAdicionadosAgora.Add(record.CPF);
                        }


                        if (!ContratosExistentes.Contains(record.CONTRATO))
                        {
                            //registrando contrato novo
                            Contrato contrato = new Contrato();
                            contrato.ClienteId = relacaoIdClienteCPF[record.CPF];
                            contrato.Vencimento = dataVencimento;
                            contrato.Valor = record.VALOR;
                            contrato.TipoContrato = record.TipoDeContrato;
                            _context.Add(contrato);
                        }
}
                    catch (Exception e)
                    {
                        TempData["ErroSalvarRegistros"] = true;
                        Console.WriteLine("\n\n\n ERRO AO SALVAR REGISTROS: "+e.Message+" \n\n\n"); 
                    }

                    try
                    {
                        //calculando atraso
                        TimeSpan atraso = hoje - dataVencimento;

                        //TODO: abstrair as chamadas pra API em uma classe se tiver tempo
                        //fazendo chamada pra api
                        using (HttpClient client = new HttpClient())
                        {
                            string url = "https://api.cobmais.com.br/testedev/calculo";
                            string payload = JsonConvert.SerializeObject(new Dictionary<string, object>
                            {
                                { "TipoContrato", record.TipoDeContrato },
                                { "Atraso", atraso.Days },
                                { "Valor", record.VALOR },
                            });

                            HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");

                            HttpResponseMessage response = await client.PostAsync(url, content);

                            if (!response.IsSuccessStatusCode)
                            {
                                throw new Exception(response.StatusCode.ToString());
                            }

                            string responseBody = await response.Content.ReadAsStringAsync();

                            Console.WriteLine("\n\n\n RESPONSE: \n"+responseBody+"\n\n\n -------------------- \n\n\n");

                        }
                    }
                    catch (Exception e)
                    {
                        TempData["ErroAoFazerRequisicaoAPI"] = true;
                        Console.WriteLine("\n\n\n ERRO AO FAZER REQUISICAO A API, STATUS: "+e.Message+" \n\n\n"); 
                    }
                        
                    await _context.SaveChangesAsync();

                }
            }

            return RedirectToAction("Index");

            //fazer parse do csv
            //para cada linha do arquivo
            //  separar as informações usando % e criando os registros apropriados nas tabelas de contrato e cliente
            //  depois, montar a chamada http para a api cobmais e salvar o retorno na tabela de log
        }

        private bool LogConsultaExists(int id)
        {
          return (_context.LogConsultas?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
