using KrtBank.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace KrtBank.Infrastructure.Services;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<CacheService> _logger;
    private readonly TimeSpan _defaultExpiration = TimeSpan.FromHours(1);

    public CacheService(IMemoryCache memoryCache, ILogger<CacheService> logger)
    {
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public async Task<T?> ObterAsync<T>(string chave) where T : class
    {
        if (_memoryCache.TryGetValue(chave, out var valor))
        {
            _logger.LogInformation("CACHE HIT for key: {Chave}", chave);
            
            if (valor is T resultado)
                return resultado;
            
            if (valor is string json)
                return JsonSerializer.Deserialize<T>(json);
        }

        _logger.LogInformation("CACHE MISS for key: {Chave}", chave);
        await Task.CompletedTask;
        return null;
    }

    public async Task DefinirAsync<T>(string chave, T valor, TimeSpan? expiracao = null) where T : class
    {
        var opcoes = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiracao ?? _defaultExpiration,
            SlidingExpiration = TimeSpan.FromMinutes(30)
        };

        _memoryCache.Set(chave, valor, opcoes);
        _logger.LogInformation("CACHE SET for key: {Chave} (expires in: {Expiracao})", chave, expiracao ?? _defaultExpiration);
        await Task.CompletedTask;
    }

    public async Task RemoverAsync(string chave)
    {
        _memoryCache.Remove(chave);
        _logger.LogInformation("CACHE REMOVE for key: {Chave}", chave);
        await Task.CompletedTask;
    }

    public async Task RemoverPorPadraoAsync(string padrao)
    {
        if (!string.IsNullOrEmpty(padrao) && !padrao.EndsWith("*"))
        {
            _memoryCache.Remove(padrao);
            _logger.LogInformation("CACHE REMOVE PATTERN for key: {Chave}", padrao);
            await Task.CompletedTask;
            return;
        }

        if (padrao?.EndsWith("*") == true)
        {
            var prefixo = padrao.Substring(0, padrao.Length - 1);
            
            var chavesConhecidas = new[]
            {
                "contas:todas"
            };

            foreach (var chave in chavesConhecidas)
            {
                _memoryCache.Remove(chave);
                _logger.LogInformation("CACHE REMOVE PATTERN for key: {Chave}", chave);
            }
        }
        else
        {
            var chavesConhecidas = new[]
            {
                "contas:todas"
            };

            foreach (var chave in chavesConhecidas)
            {
                _memoryCache.Remove(chave);
                _logger.LogInformation("CACHE REMOVE PATTERN for key: {Chave}", chave);
            }
        }
        
        await Task.CompletedTask;
    }

    public async Task AtualizarItemNaListaAsync<T>(string chaveLista, Guid itemId, T itemAtualizado, Func<T, Guid> obterId) where T : class
    {
        if (_memoryCache.TryGetValue(chaveLista, out var listaCache))
        {
            if (listaCache is List<T> lista)
            {
                var indice = lista.FindIndex(item => obterId(item) == itemId);
                if (indice >= 0)
                {
                    lista[indice] = itemAtualizado;
                    _logger.LogInformation("CACHE UPDATE ITEM in list: {Chave} for item: {ItemId}", chaveLista, itemId);
                }
                else
                {
                    _logger.LogInformation("CACHE UPDATE ITEM: Item {ItemId} not found in list {Chave}", itemId, chaveLista);
                }
            }
            else
            {
                _logger.LogInformation("CACHE UPDATE ITEM: List {Chave} is not of expected type", chaveLista);
            }
        }
        else
        {
            _logger.LogInformation("CACHE UPDATE ITEM: List {Chave} not found in cache, skipping update", chaveLista);
        }
        
        await Task.CompletedTask;
    }
}

