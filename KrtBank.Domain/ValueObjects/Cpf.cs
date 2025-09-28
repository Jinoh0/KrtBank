using KrtBank.Domain.Utils;

namespace KrtBank.Domain.ValueObjects;

public class Cpf
{
    public string Valor { get; private set; }

    public Cpf(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("CPF não pode ser vazio", nameof(valor));

        var cpfLimpo = CpfValidator.RemoveFormatting(valor);
        
        if (!CpfValidator.IsValid(valor))
            throw new ArgumentException("CPF inválido", nameof(valor));

        Valor = cpfLimpo;
    }

    public override string ToString()
    {
        return $"{Valor.Substring(0, 3)}.{Valor.Substring(3, 3)}.{Valor.Substring(6, 3)}-{Valor.Substring(9, 2)}";
    }

    public override bool Equals(object? obj)
    {
        if (obj is Cpf cpf)
            return Valor == cpf.Valor;
        return false;
    }

    public override int GetHashCode()
    {
        return Valor.GetHashCode();
    }
}

