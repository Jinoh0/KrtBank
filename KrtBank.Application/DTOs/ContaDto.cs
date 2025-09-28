using KrtBank.Domain.Enums;

namespace KrtBank.Application.DTOs;

public class ContaDto
{
    public Guid Id { get; set; }
    public string NomeTitular { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public StatusConta Status { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
}

