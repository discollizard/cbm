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
using TesteCobmais.DTO;

namespace TesteCobmais.Controllers;
public class ConsultasController : Controller
{
    private readonly TesteCobmaisDbContext _context;

    private enum ErrosDeProcessamento
    {
        ERRO_AO_SALVAR_REGISTRO_CLIENTE,
        ERRO_AO_SALVAR_REGISTRO_CONTRATO,
        ERRO_AO_PROCESSAR_RESPOSTA_API
    }


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
    public async Task<IActionResult> ImportarArquivoCSV()
    {

        List<ErrosDeProcessamento> erros = new List<ErrosDeProcessamento>();

        var config = new CsvConfiguration(new CultureInfo("pt-BR"))
        {
            NewLine = Environment.NewLine,
            Delimiter = ";"
        };

        StreamReader reader = new StreamReader("Arquivos CSV/dividas-originais_teste_cobmais_2021.csv");

        using (var csv = new CsvReader(reader, config))
        {

            var registros = csv.GetRecords<CSVImportTemplate>();

            //Indexando relação entre atributo unique e chave primaria para
            //acessar diretamente dentro do loop e manter a complexidade temporal em O(n)
            Dictionary<long, int> relacaoIdClienteCPF = _context.Clientes
            .Select(cliente => new { cliente.Id, cliente.CPF })
            .ToDictionary(Cliente => Cliente.CPF, Cliente => Cliente.Id);

            Dictionary<string, int> relacaoIdContratoIdDivida = _context.Contratos
            .Select(contrato => new { contrato.Id, contrato.DividaId })
            .ToDictionary(contrato => contrato.DividaId, contrato => contrato.Id);

            //declarando timestamp de hoje fora do loop para evitar redeclarações desnecessarias

            foreach (var registro in registros)
            {
                //parse da data de vencimento
                DateTime dataVencimento;
                DateTime.TryParseExact(registro.VENCIMENTO, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dataVencimento);

                if (!relacaoIdClienteCPF.ContainsKey(registro.CPF))
                {
                    try
                    {
                        //registrando cliente novo
                        Cliente cliente = new Cliente();
                        cliente.CPF = registro.CPF;
                        cliente.NomeCompleto = registro.CLIENTE;
                        _context.Add(cliente);
                        await _context.SaveChangesAsync();

                        relacaoIdClienteCPF.TryAdd(registro.CPF, cliente.Id);
                    }
                    catch (Exception e)
                    {
                        erros.Add(ErrosDeProcessamento.ERRO_AO_SALVAR_REGISTRO_CLIENTE);
                    }
                }

                if (!relacaoIdContratoIdDivida.ContainsKey(registro.CONTRATO))
                {
                    try
                    {
                        //registrando contrato novo
                        Contrato contrato = new Contrato();
                        contrato.DividaId = registro.CONTRATO;
                        contrato.ClienteId = relacaoIdClienteCPF[registro.CPF];
                        contrato.Vencimento = dataVencimento;
                        contrato.Valor = registro.VALOR;
                        contrato.TipoContrato = registro.TipoDeContrato;
                        _context.Add(contrato);
                        await _context.SaveChangesAsync();

                        relacaoIdContratoIdDivida.TryAdd(registro.CONTRATO, contrato.Id);
                    }
                    catch (Exception e)
                    {
                        erros.Add(ErrosDeProcessamento.ERRO_AO_SALVAR_REGISTRO_CONTRATO);
                    }

                }

                int atrasoEmDias = Contrato.CalcularVencimento(dataVencimento);

                //TODO: abstrair as chamadas pra API em uma classe se tiver tempo
                //fazendo chamada pra api
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string url = "https://api.cobmais.com.br/testedev/calculo";
                        string payload = JsonConvert.SerializeObject(new Dictionary<string, object>
                            {
                                { "TipoContrato", registro.TipoDeContrato },
                                { "Atraso", atrasoEmDias },
                                { "Valor", registro.VALOR },
                            });

                        HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");

                        HttpResponseMessage response = await client.PostAsync(url, content);

                        if (!response.IsSuccessStatusCode)
                        {
                            throw new Exception(response.StatusCode.ToString());
                        }

                        string responseBodyJson = await response.Content.ReadAsStringAsync();
                        CalculoResponseBody responseBody = new CalculoResponseBody(responseBodyJson);

                        //adicionando registro de log no banco
                        LogConsulta logRegistro = new LogConsulta();
                        logRegistro.ConsultaTimestamp = DateTime.Now;
                        logRegistro.ContratoId = relacaoIdContratoIdDivida[registro.CONTRATO];
                        logRegistro.AtrasoEmDias = atrasoEmDias;
                        logRegistro.ValorAtualizado = responseBody.ValorAtualizado;
                        logRegistro.DescontoMaximo = responseBody.DescontoMaximo;
                        _context.Add(logRegistro);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (Exception e)
                {
                    erros.Add(ErrosDeProcessamento.ERRO_AO_PROCESSAR_RESPOSTA_API);
                }
            }

            Console.WriteLine(erros);

            return RedirectToAction("Index");
        }
    }

    [HttpGet]
    public async Task<IActionResult> ExportarDividasAtualizadas()
    {
        List<ErrosDeProcessamento> erros = new List<ErrosDeProcessamento>();

        DateTime today = DateTime.UtcNow.Date;

        var query = from co in _context.Contratos
                    join c in _context.Clientes on co.ClienteId equals c.Id
                    join l in _context.LogConsultas on co.Id equals l.ContratoId into logGroup
                    where logGroup.First() != null
                    select new
                    {
                        CPF = c.CPF,
                        ValorOriginal = co.Valor,
                        VencimentoContrato = co.Vencimento,
                        TipoDeContrato = co.TipoContrato,
                        ContratoId = co.Id,
                        DividaId = co.DividaId,
                        LogMaisRecente = logGroup.OrderByDescending(l => l.ConsultaTimestamp).First().ConsultaTimestamp,
                        ValorAtualizado = logGroup.OrderByDescending(l => l.ConsultaTimestamp).First().ValorAtualizado,
                        PorcentagemDescontoMaximo = logGroup.OrderByDescending(l => l.ConsultaTimestamp).First().DescontoMaximo
                    };

        var resultado = query.ToList();

        DateTime hoje = today;
        string hojeFormatadoLinha = hoje.ToString("dd/MM/yyyy");

        List<CSVExportTemplateLinha> dadosParaExportar = new List<CSVExportTemplateLinha>();

        foreach(var registro in resultado)
        {
            //reconsultando a API se não existe registros referentes ao valor atualizado
            //de hoje
            bool TimestampSimDeHoje = registro.LogMaisRecente.Date == DateTime.Today;

            double valorAtualizado = registro.ValorAtualizado;

            if (!TimestampSimDeHoje)
            {
                try { 
                using (HttpClient client = new HttpClient())
                    {
                    int atrasoEmDias = Contrato.CalcularVencimento(registro.VencimentoContrato);
                        string url = "https://api.cobmais.com.br/testedev/calculo";
                        string payload = JsonConvert.SerializeObject(new Dictionary<string, object>
                            {
                                { "TipoContrato", registro.TipoDeContrato },
                                { "Atraso", atrasoEmDias },
                                { "Valor", registro.ValorOriginal },
                            });

                        HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");

                        HttpResponseMessage response = await client.PostAsync(url, content);

                        if (!response.IsSuccessStatusCode)
                        {
                            throw new Exception(response.StatusCode.ToString());
                        }

                        string responseBodyJson = await response.Content.ReadAsStringAsync();
                        CalculoResponseBody responseBody = new CalculoResponseBody(responseBodyJson);

                        //adicionando registro de log no banco
                        LogConsulta logRegistro = new LogConsulta();
                        logRegistro.ConsultaTimestamp = DateTime.Now;
                        logRegistro.ContratoId = registro.ContratoId;
                        logRegistro.AtrasoEmDias = atrasoEmDias;
                        logRegistro.ValorAtualizado = responseBody.ValorAtualizado;
                        logRegistro.DescontoMaximo = responseBody.DescontoMaximo;
                        _context.Add(logRegistro);
                        await _context.SaveChangesAsync();

                        valorAtualizado = responseBody.ValorAtualizado;
                    }
                }
                catch (Exception e)
                {
                    erros.Add(ErrosDeProcessamento.ERRO_AO_PROCESSAR_RESPOSTA_API);
                }
     
            }

            var linhaFormatada = new CSVExportTemplateLinha();
            linhaFormatada.CPF = registro.CPF;
            linhaFormatada.DATA = hojeFormatadoLinha;
            linhaFormatada.CONTRATO = registro.DividaId;
            linhaFormatada.ValorOriginal = registro.ValorOriginal;
            linhaFormatada.ValorAtualizado = valorAtualizado;
            linhaFormatada.ValorDesconto = 
                valorAtualizado - ((valorAtualizado / 100) * registro.PorcentagemDescontoMaximo);

            dadosParaExportar.Add(linhaFormatada);
        }

        //exportando pra csv
        CsvConfiguration config = new CsvConfiguration(new CultureInfo("pt-BR"))
        {
            NewLine = Environment.NewLine,
            Delimiter = ";"
        };

        StreamReader reader = new StreamReader("Arquivos CSV/dividas-originais_teste_cobmais_2021.csv");
        string hojeFormatadoArquivo = hoje.ToString("dd-MM-yyyy");
        using (var writer = new StreamWriter("Arquivos CSV/Divida-Atualizada-"+hojeFormatadoArquivo+".csv"))
        using (var csv = new CsvWriter(writer, config))
        {
            csv.WriteRecords(dadosParaExportar);
        }

        return RedirectToAction("Index");
    }
}
