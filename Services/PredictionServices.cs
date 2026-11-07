namespace MLconsole.Services;

public record PredictionResult(string Label, float Confidence);

public interface IPredictionService
{
    PredictionResult Predict(string text);
}

// Temporal: se reemplaza por el modelo real cuando lo entreguen tus compañeros
public class FakePredictionService : IPredictionService
{
    public PredictionResult Predict(string text)
    {
        var positivo = text.Contains("good", StringComparison.OrdinalIgnoreCase)
                    || text.Contains("bueno", StringComparison.OrdinalIgnoreCase);
        return new(positivo ? "Positivo" : "Negativo", 0.87f);
    }
}