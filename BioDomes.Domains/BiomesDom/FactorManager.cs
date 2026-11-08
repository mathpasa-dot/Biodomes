namespace BioDomes.Domains.BiomesDom;

public static class FactorManager
{
    public static BiomeState TranslateTemperatureToState(double temperature)
    {
        if (temperature < -20 || temperature > 40) return BiomeState.Critical;

        if (temperature < 0 || temperature > 30) return BiomeState.Unstable;

        return BiomeState.Optimal;
    }

    public static string? Respect(double temperature, double moisture)
    {
        var result = 216.7 * 6.112 * Math.Pow(Math.E, 17.67 * temperature / (temperature + 243.5)) /
                     (temperature + 273.15);

        if (result < moisture)
            return "L'humidité max pour une température " + temperature + " est de " + result + ". Reçu: " + moisture;

        return null;
    }
}