using KrtBank.Domain.Entities;

namespace KrtBank.Domain.Events;

public class ContaCriadaEvent
{
    public Guid ContaId { get; }
    public string NomeTitular { get; }
    public string Cpf { get; }
    public DateTime DataCriacao { get; }

    public ContaCriadaEvent(Conta conta)
    {
        ContaId = conta.Id;
        NomeTitular = conta.NomeTitular;
        Cpf = conta.Cpf.Valor;
        DataCriacao = conta.DataCriacao;
    }
}

