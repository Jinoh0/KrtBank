using KrtBank.Domain.Entities;
using KrtBank.Domain.Interfaces;
using KrtBank.Domain.ValueObjects;
using KrtBank.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KrtBank.Infrastructure.Repositories;

public class ContaRepository : IContaRepository
{
    private readonly KrtBankContext _context;
    private readonly ILogger<ContaRepository> _logger;

    public ContaRepository(KrtBankContext context, ILogger<ContaRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Conta?> ObterPorIdAsync(Guid id)
    {
        try
        {
            _logger.LogDebug("Repository: Searching account by ID: {Id}", id);
            var conta = await _context.Contas.FindAsync(id);
            if (conta == null)
                _logger.LogDebug("Repository: Account not found: {Id}", id);
            else
                _logger.LogDebug("Repository: Account found: {Id}", id);
            return conta;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Repository: Error retrieving account by ID: {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Conta>> ObterTodosAsync()
    {
        return await _context.Contas.ToListAsync();
    }

    public async Task<Conta> AdicionarAsync(Conta entity)
    {
        _context.Contas.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task AtualizarAsync(Conta entity)
    {
        _context.Contas.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task RemoverAsync(Guid id)
    {
        var conta = await _context.Contas.FindAsync(id);
        if (conta != null)
        {
            _context.Contas.Remove(conta);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExisteAsync(Guid id)
    {
        return await _context.Contas.AnyAsync(c => c.Id == id);
    }

    public async Task<Conta?> ObterPorCpfAsync(Cpf cpf)
    {
        var contas = await _context.Contas.ToListAsync();
        return contas.FirstOrDefault(c => c.Cpf.Valor == cpf.Valor);
    }

    public async Task<bool> ExisteCpfAsync(Cpf cpf)
    {
        var contas = await _context.Contas.ToListAsync();
        return contas.Any(c => c.Cpf.Valor == cpf.Valor);
    }

    public async Task<IEnumerable<Conta>> ObterContasAtivasAsync()
    {
        return await _context.Contas.Where(c => c.Status == Domain.Enums.StatusConta.Ativa).ToListAsync();
    }
}
