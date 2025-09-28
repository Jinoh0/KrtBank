namespace KrtBank.Application.Interfaces;

public interface ICacheService
{
    Task<T?> ObterAsync<T>(string chave) where T : class;
    Task DefinirAsync<T>(string chave, T valor, TimeSpan? expiracao = null) where T : class;
    Task RemoverAsync(string chave);
    Task RemoverPorPadraoAsync(string padrao);
    Task AtualizarItemNaListaAsync<T>(string chaveLista, Guid itemId, T itemAtualizado, Func<T, Guid> obterId) where T : class;
}

