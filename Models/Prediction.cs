namespace MLconsole.Models;

public class Prediccion
{
    public int Id { get; set; }
    public string Texto { get; set; } = "";
    public string Etiqueta { get; set; } = "";
    public float Confianza { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public string UsuarioId { get; set; } = "";
}