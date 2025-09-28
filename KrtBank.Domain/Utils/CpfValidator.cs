using System.Text.RegularExpressions;

namespace KrtBank.Domain.Utils;

public static class CpfValidator
{
    public static bool IsValid(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        var cpfLimpo = RemoveFormatting(cpf);
        
        if (!HasValidLength(cpfLimpo))
            return false;

        if (HasAllSameDigits(cpfLimpo))
            return false;

        return HasValidCheckDigits(cpfLimpo);
    }

    public static string RemoveFormatting(string cpf)
    {
        return Regex.Replace(cpf, @"[^\d]", "");
    }

    public static bool HasValidLength(string cpf)
    {
        return cpf.Length == 11;
    }

    public static bool HasAllSameDigits(string cpf)
    {
        return cpf.All(c => c == cpf[0]);
    }

    public static bool HasValidCheckDigits(string cpf)
    {
        var soma = 0;
        for (int i = 0; i < 9; i++)
        {
            soma += int.Parse(cpf[i].ToString()) * (10 - i);
        }

        var resto = soma % 11;
        var digito1 = resto < 2 ? 0 : 11 - resto;

        if (int.Parse(cpf[9].ToString()) != digito1)
            return false;

        soma = 0;
        for (int i = 0; i < 10; i++)
        {
            soma += int.Parse(cpf[i].ToString()) * (11 - i);
        }

        resto = soma % 11;
        var digito2 = resto < 2 ? 0 : 11 - resto;

        return int.Parse(cpf[10].ToString()) == digito2;
    }
}
