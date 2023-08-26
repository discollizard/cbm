using Microsoft.EntityFrameworkCore;
using TesteCobmais.Models;

namespace TesteCobmais.Data;


public class ApplicationDbContext: DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
             : base(options)
    {

    }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Contrato> Contratos { get; set; }
    public DbSet<LogConsulta> LogConsultas { get; set; }
}

