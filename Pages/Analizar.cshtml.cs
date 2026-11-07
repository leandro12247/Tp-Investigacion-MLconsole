using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MLconsole.Data;
using MLconsole.Models;
using MLconsole.Services;

namespace MLconsole.Pages;

[Authorize]
public class AnalizarModel : PageModel
{
    private readonly IPredictionService _prediction;
    private readonly ApplicationDbContext _db;

    public AnalizarModel(IPredictionService prediction, ApplicationDbContext db)
    {
        _prediction = prediction;
        _db = db;
    }

    [BindProperty]
    [Required(ErrorMessage = "Escribí un texto para analizar")]
    [StringLength(500)]
    public string Texto { get; set; } = "";

    public PredictionResult? Resultado { get; set; }
    public List<Prediccion> Historial { get; set; } = new();

    public async Task OnGetAsync() => await CargarHistorial();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await CargarHistorial();
            return Page();
        }

        Resultado = _prediction.Predict(Texto);

        _db.Predicciones.Add(new Prediccion
        {
            Texto = Texto,
            Etiqueta = Resultado.Label,
            Confianza = Resultado.Confidence,
            UsuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier)!
        });
        await _db.SaveChangesAsync();

        await CargarHistorial();
        return Page();
    }

    private async Task CargarHistorial()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        Historial = await _db.Predicciones
            .Where(p => p.UsuarioId == userId)
            .OrderByDescending(p => p.Fecha)
            .Take(10)
            .ToListAsync();
    }
}