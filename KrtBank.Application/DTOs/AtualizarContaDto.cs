using System.ComponentModel.DataAnnotations;

namespace KrtBank.Application.DTOs;

public class AtualizarContaDto
{
    [Required(ErrorMessage = "Nome do titular é obrigatório")]
    [StringLength(100, ErrorMessage = "Nome do titular deve ter no máximo 100 caracteres")]
    public string NomeTitular { get; set; } = string.Empty;
}

