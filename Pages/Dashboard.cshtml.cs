using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MLconsole.Data;

namespace MLconsole.Pages;

[Authorize]
public class DashboardModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public DashboardModel(ApplicationDbContext db) => _db = db;

    public int Total { get; set; }
    public int Positivos { get; set; }
    public int Negativos { get; set; }
    public double ConfianzaPromedio { get; set; }
    public List<string> Dias { get; set; } = new();
    public List<int> PorDia { get; set; } = new();

    public async Task OnGetAsync()
    {
        Total = await _db.Predicciones.CountAsync();
        Positivos = await _db.Predicciones.CountAsync(p => p.Etiqueta == "Positivo");
        Negativos = Total - Positivos;

        if (Total > 0)
            ConfianzaPromedio = (double)await _db.Predicciones.AverageAsync(p => p.Confianza) * 100;

        // Predicciones de los últimos 7 días
        var desde = DateTime.UtcNow.Date.AddDays(-6);
        var fechas = await _db.Predicciones
            .Where(p => p.Fecha >= desde)
            .Select(p => p.Fecha)
            .ToListAsync();

        for (int i = 0; i < 7; i++)
        {
            var dia = desde.AddDays(i);
            Dias.Add(dia.ToString("dd/MM"));
            PorDia.Add(fechas.Count(f => f.Date == dia));
        }
    }
}