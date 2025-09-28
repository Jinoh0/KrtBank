using System.ComponentModel.DataAnnotations;

namespace KrtBank.Application.DTOs;

public class CriarContaDto
{
    [Required(ErrorMessage = "Nome do titular é obrigatório")]
    [StringLength(100, ErrorMessage = "Nome do titular deve ter no máximo 100 caracteres")]
    public string NomeTitular { get; set; } = string.Empty;

    [Required(ErrorMessage = "CPF é obrigatório")]
    [RegularExpression(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$", ErrorMessage = "CPF deve estar no formato 000.000.000-00")]
    public string Cpf { get; set; } = string.Empty;
}

