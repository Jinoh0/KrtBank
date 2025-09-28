using KrtBank.Application.DTOs;

namespace KrtBank.Application.Interfaces;

public interface IContaService
{
    Task<ContaDto?> ObterPorIdAsync(Guid id);
    Task<IEnumerable<ContaDto>> ObterTodasAsync();
    Task<ContaDto> CriarAsync(CriarContaDto dto);
    Task<ContaDto> AtualizarAsync(Guid id, AtualizarContaDto dto);
    Task<bool> RemoverAsync(Guid id);
    Task<ContaDto> AtivarAsync(Guid id);
    Task<ContaDto> InativarAsync(Guid id);
}

