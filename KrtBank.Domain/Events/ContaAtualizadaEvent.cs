using KrtBank.Domain.Entities;

namespace KrtBank.Domain.Events;

public class ContaAtualizadaEvent
{
    public Guid ContaId { get; }
    public string NomeTitular { get; }
    public string Cpf { get; }
    public string Status { get; }
    public DateTime DataAtualizacao { get; }

    public ContaAtualizadaEvent(Conta conta)
    {
        ContaId = conta.Id;
        NomeTitular = conta.NomeTitular;
        Cpf = conta.Cpf.Valor;
        Status = conta.Status.ToString();
        DataAtualizacao = conta.DataAtualizacao ?? DateTime.UtcNow;
    }
}

