using KrtBank.Domain.Enums;
using KrtBank.Domain.ValueObjects;

namespace KrtBank.Domain.Entities;

public class Conta
{
    public Guid Id { get; private set; }
    public string NomeTitular { get; private set; } = string.Empty;
    public Cpf Cpf { get; private set; } = null!;
    public StatusConta Status { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime? DataAtualizacao { get; private set; }

    private Conta() { }

    public Conta(string nomeTitular, Cpf cpf)
    {
        Id = Guid.NewGuid();
        NomeTitular = nomeTitular;
        Cpf = cpf;
        Status = StatusConta.Ativa;
        DataCriacao = DateTime.UtcNow;
    }

    public void AtualizarNomeTitular(string novoNome)
    {
        if (string.IsNullOrWhiteSpace(novoNome))
            throw new ArgumentException("Nome do titular não pode ser vazio", nameof(novoNome));

        NomeTitular = novoNome;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void Ativar()
    {
        Status = StatusConta.Ativa;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void Inativar()
    {
        Status = StatusConta.Inativa;
        DataAtualizacao = DateTime.UtcNow;
    }

    public bool EstaAtiva() => Status == StatusConta.Ativa;
}
