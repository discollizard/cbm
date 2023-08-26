using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;
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
using TesteCobmais.Data;
using TesteCobmais.Models;

namespace TesteCobmais.Controllers
{
    public class ConsultasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ConsultasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Consultas
        public async Task<IActionResult> Index()
        {
            if (TempData.TryGetValue("ArquivoInvalidoError", out var hasError))
            {
                if ((bool)hasError)
                {
                    ModelState.AddModelError("ArquivoInvalidoError", "O arquivo enviado é inválido ou inexistente");
                }
            }

            var applicationDbContext = _context.LogConsultas.Include(l => l.contrato);
            return View(await applicationDbContext.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult>? ProcessarArquivoCSV(IFormFile file)
        {
                
            if(file == null)
            {
                TempData["ArquivoInvalidoError"] = true;
                return RedirectToAction("Index");
            }

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                NewLine = Environment.NewLine,
                Delimiter = ";"
            };

            StreamReader reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);


            using (var csv = new CsvReader(reader, config))
            {
                var records = csv.GetRecords<CSVImportTemplate>();
                foreach(var record in records)
                {
                    Console.WriteLine(record.CPF);
                }
            }
            
            return RedirectToAction("Index");

            //fazer parse do csv
            //para cada linha do arquivo
            //  separar as informações usando % e criando os registros apropriados nas tabelas de contrato e cliente
            //  depois, montar a chamada http para a api cobmais e salvar o retorno na tabela de log
        }

        // GET: Consultas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.LogConsultas == null)
            {
                return NotFound();
            }

            var logConsulta = await _context.LogConsultas
                .Include(l => l.contrato)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (logConsulta == null)
            {
                return NotFound();
            }

            return View(logConsulta);
        }

        // GET: Consultas/Create
        public IActionResult Create()
        {
            ViewData["ContratoId"] = new SelectList(_context.Contratos, "Id", "Id");
            return View();
        }

        // POST: Consultas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ContratoId,ConsultaTimestamp,AtrasoEmDias,ValorAtualizado,DescontoMaximo")] LogConsulta logConsulta)
        {
            if (ModelState.IsValid)
            {
                _context.Add(logConsulta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ContratoId"] = new SelectList(_context.Contratos, "Id", "Id", logConsulta.ContratoId);
            return View(logConsulta);
        }

        // GET: Consultas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.LogConsultas == null)
            {
                return NotFound();
            }

            var logConsulta = await _context.LogConsultas.FindAsync(id);
            if (logConsulta == null)
            {
                return NotFound();
            }
            ViewData["ContratoId"] = new SelectList(_context.Contratos, "Id", "Id", logConsulta.ContratoId);
            return View(logConsulta);
        }

        // POST: Consultas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ContratoId,ConsultaTimestamp,AtrasoEmDias,ValorAtualizado,DescontoMaximo")] LogConsulta logConsulta)
        {
            if (id != logConsulta.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(logConsulta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LogConsultaExists(logConsulta.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ContratoId"] = new SelectList(_context.Contratos, "Id", "Id", logConsulta.ContratoId);
            return View(logConsulta);
        }

        // GET: Consultas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.LogConsultas == null)
            {
                return NotFound();
            }

            var logConsulta = await _context.LogConsultas
                .Include(l => l.contrato)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (logConsulta == null)
            {
                return NotFound();
            }

            return View(logConsulta);
        }

        // POST: Consultas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.LogConsultas == null)
            {
                return Problem("Entity set 'ApplicationDbContext.LogConsultas'  is null.");
            }
            var logConsulta = await _context.LogConsultas.FindAsync(id);
            if (logConsulta != null)
            {
                _context.LogConsultas.Remove(logConsulta);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LogConsultaExists(int id)
        {
          return (_context.LogConsultas?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
