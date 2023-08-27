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
    public class ClientesController : Controller
    {
        private readonly TesteCobmaisDbContext _context;

        public ClientesController(TesteCobmaisDbContext context)
        {
            _context = context;
        }

        // GET: Clientes
        public async Task<IActionResult> Index()
        {
            return _context.Clientes != null ?
                        View(await _context.Clientes.ToListAsync()) :
                        Problem("Entity set 'TesteCobmaisDbContext.Clientes'  is null.");
        }


    }
}
