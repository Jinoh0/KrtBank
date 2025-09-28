namespace KrtBank.Application.Interfaces;

public interface INotificationService
{
    Task NotificarContaCriadaAsync(Guid contaId, string nomeTitular, string cpf);
    Task NotificarContaAtualizadaAsync(Guid contaId, string nomeTitular, string cpf, string status);
    Task NotificarContaRemovidaAsync(Guid contaId);
}

