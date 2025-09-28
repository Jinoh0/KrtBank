using KrtBank.Domain.Entities;
using KrtBank.Domain.ValueObjects;

namespace KrtBank.Domain.Interfaces;

public interface IContaRepository : IRepository<Conta>
{
    Task<Conta?> ObterPorCpfAsync(Cpf cpf);
    Task<bool> ExisteCpfAsync(Cpf cpf);
    Task<IEnumerable<Conta>> ObterContasAtivasAsync();
}

