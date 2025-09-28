using KrtBank.Domain.Utils;

namespace KrtBank.Application.Utils;

public static class CpfNormalizer
{
    public static string Normalize(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            throw new ArgumentException("CPF não pode ser vazio", nameof(cpf));

        var digitsOnly = CpfValidator.RemoveFormatting(cpf);
        
        if (!CpfValidator.HasValidLength(digitsOnly))
            throw new ArgumentException($"CPF deve ter 11 dígitos. Fornecidos: {digitsOnly.Length}", nameof(cpf));

        return $"{digitsOnly[0..3]}.{digitsOnly[3..6]}.{digitsOnly[6..9]}-{digitsOnly[9..11]}";
    }
}
