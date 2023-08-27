using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TesteCobmais.Data;
using TesteCobmais.Models;

namespace TesteCobmais.Controllers
{
    public class ContratosController : Controller
    {
        private readonly TesteCobmaisDbContext _context;

        public ContratosController(TesteCobmaisDbContext context)
        {
            _context = context;
        }

        // GET: Contratos
        public async Task<IActionResult> Index()
        {
            var testeCobmaisDbContext = _context.Contratos.Include(c => c.cliente);
            return View(await testeCobmaisDbContext.ToListAsync());
        }
    }
}
