using System.ComponentModel.DataAnnotations;

namespace KrtBank.Application.DTOs;

public class CriarContaDto
{
    [Required(ErrorMessage = "Nome do titular é obrigatório")]
    [StringLength(100, ErrorMessage = "Nome do titular deve ter no máximo 100 caracteres")]
    public string NomeTitular { get; set; } = string.Empty;

    [Required(ErrorMessage = "CPF é obrigatório")]
    public string Cpf { get; set; } = string.Empty;
}

