namespace BTGDesktop;

public static class StringHelper
{
    public static string DoubleParaReal(this double valor)
    {
        return string.Format(new CultureInfo("pt-BR"), "{0:C}", valor);
    }

    public static string FloatParaReal(this float valor)
    {
        return string.Format(new CultureInfo("pt-BR"), "{0:C}", valor);
    }

    public static string DecimalParaReal(this decimal valor, bool arredondarParaCima = false)
    {
        return !arredondarParaCima ? string.Format(new CultureInfo("pt-BR"), "{0:C}", valor)
            : string.Format(new CultureInfo("pt-BR"), "{0:C}", Math.Ceiling(valor * 100) / 100.0M);
    }

    public static decimal StringMoneyToDecimal(this string value)
    {
        if (System.String.IsNullOrEmpty(value))
            return 0;
        value = value.Replace(".", "").Replace(",", "").Replace("R", "").Replace("$", "").Replace(" ", "");
        return !String.IsNullOrEmpty(value) ? Convert.ToDecimal(value) / 100 : 0.0m;
    }

    public static decimal RealParaDecimal(this string valor)
    {
        if (!string.IsNullOrEmpty(valor))
            return decimal.Parse(Regex.Replace(valor, @"[^\d+(\,)]", ""));

        return 0;
    }

    public static double ToPercent(this double valor)
    {
        return valor > 1 ? valor / 100 : valor;
    } 
}
