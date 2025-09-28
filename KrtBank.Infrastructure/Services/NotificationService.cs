using KrtBank.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace KrtBank.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
    }

    public async Task NotificarContaCriadaAsync(Guid contaId, string nomeTitular, string cpf)
    {
        _logger.LogInformation("Notificando áreas do banco sobre conta criada: {ContaId}", contaId);
        
        await NotificarAreaFraudeAsync(contaId, nomeTitular, cpf, "CRIADA");
        await NotificarAreaCartoesAsync(contaId, nomeTitular, cpf, "CRIADA");
        await NotificarAreaCreditoAsync(contaId, nomeTitular, cpf, "CRIADA");
        
        _logger.LogInformation("Notificações enviadas para todas as áreas sobre conta criada: {ContaId}", contaId);
    }

    public async Task NotificarContaAtualizadaAsync(Guid contaId, string nomeTitular, string cpf, string status)
    {
        _logger.LogInformation("Notificando áreas do banco sobre conta atualizada: {ContaId}", contaId);
        
        await NotificarAreaFraudeAsync(contaId, nomeTitular, cpf, "ATUALIZADA");
        await NotificarAreaCartoesAsync(contaId, nomeTitular, cpf, "ATUALIZADA");
        await NotificarAreaCreditoAsync(contaId, nomeTitular, cpf, "ATUALIZADA");
        
        _logger.LogInformation("Notificações enviadas para todas as áreas sobre conta atualizada: {ContaId}", contaId);
    }

    public async Task NotificarContaRemovidaAsync(Guid contaId)
    {
        _logger.LogInformation("Notificando áreas do banco sobre conta removida: {ContaId}", contaId);
        
        await NotificarAreaFraudeAsync(contaId, "", "", "REMOVIDA");
        await NotificarAreaCartoesAsync(contaId, "", "", "REMOVIDA");
        await NotificarAreaCreditoAsync(contaId, "", "", "REMOVIDA");
        
        _logger.LogInformation("Notificações enviadas para todas as áreas sobre conta removida: {ContaId}", contaId);
    }

    private async Task NotificarAreaFraudeAsync(Guid contaId, string nomeTitular, string cpf, string acao)
    {
        _logger.LogInformation("Área de Fraude: Conta {Acao} - ID: {ContaId}, Nome: {NomeTitular}, CPF: {Cpf}", 
            acao, contaId, nomeTitular, cpf);
        
        await Task.Delay(100);
    }

    private async Task NotificarAreaCartoesAsync(Guid contaId, string nomeTitular, string cpf, string acao)
    {
        _logger.LogInformation("Área de Cartões: Conta {Acao} - ID: {ContaId}, Nome: {NomeTitular}, CPF: {Cpf}", 
            acao, contaId, nomeTitular, cpf);
        
        await Task.Delay(100);
    }

    private async Task NotificarAreaCreditoAsync(Guid contaId, string nomeTitular, string cpf, string acao)
    {
        _logger.LogInformation("Área de Crédito: Conta {Acao} - ID: {ContaId}, Nome: {NomeTitular}, CPF: {Cpf}", 
            acao, contaId, nomeTitular, cpf);
        
        await Task.Delay(100);
    }
}
