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

// Implementación real usando ML.NET
public class MLNetPredictionService : IPredictionService
{
    public PredictionResult Predict(string text)
    {
        // Crear el input del modelo
        var sampleData = new LanguageErrorClassificationModel.ModelInput()
        {
            Col0 = text
        };

        // Realizar la predicción
        var result = LanguageErrorClassificationModel.Predict(sampleData);

        // Obtener todas las etiquetas con sus scores ordenados
        var sortedScores = LanguageErrorClassificationModel.GetSortedScoresWithLabels(result);
        var topPrediction = sortedScores.First();

        // Determinar la etiqueta basada en el valor predicho
        // PredictedLabel será 0 o 1 (0 = Sin Error, 1 = Con Error)
        string label = result.PredictedLabel == 1 ? "Con Error" : "Sin Error";
        
        // La confianza es el score de la predicción principal
        float confidence = topPrediction.Value;

        return new PredictionResult(label, confidence);
    }
}