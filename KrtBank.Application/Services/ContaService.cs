using KrtBank.Application.DTOs;
using KrtBank.Application.Interfaces;
using KrtBank.Application.Utils;
using KrtBank.Domain.Entities;
using KrtBank.Domain.Interfaces;
using KrtBank.Domain.ValueObjects;

namespace KrtBank.Application.Services;

public class ContaService : IContaService
{
    private readonly IContaRepository _contaRepository;
    private readonly ICacheService _cacheService;
    private readonly INotificationService _notificationService;
    private const string CACHE_KEY_CONTAS = "contasCache";
    private const string CACHE_KEY_CONTAS_ATUALIZADAS = "contasCacheAtualizadas";

    public ContaService(
        IContaRepository contaRepository,
        ICacheService cacheService,
        INotificationService notificationService)
    {
        _contaRepository = contaRepository;
        _cacheService = cacheService;
        _notificationService = notificationService;
    }

    public async Task<ContaDto?> ObterPorIdAsync(Guid id)
    {
        var listaCache = await _cacheService.ObterAsync<List<ContaDto>>(CACHE_KEY_CONTAS);
        if (listaCache != null)
        {
            var contaCache = listaCache.FirstOrDefault(c => c.Id == id);
            if (contaCache != null)
                return contaCache;
        }

        var conta = await _contaRepository.ObterPorIdAsync(id);
        if (conta == null)
            return null;

        var contaDto = MapearParaDto(conta);
        
        if (listaCache != null)
        {
            listaCache.Add(contaDto);
            await _cacheService.AtualizarConteudoAsync(CACHE_KEY_CONTAS, listaCache);
        }
        else
        {
            await _cacheService.DefinirAsync(CACHE_KEY_CONTAS, new List<ContaDto> { contaDto }, TimeSpan.FromHours(3));
        }

        return contaDto;
    }

    public async Task<IEnumerable<ContaDto>> ObterTodasAsync()
    {
        var isComplete = await _cacheService.ObterAsync<string>(CACHE_KEY_CONTAS_ATUALIZADAS);
        var contasCache = await _cacheService.ObterAsync<List<ContaDto>>(CACHE_KEY_CONTAS);
        
        if (isComplete == "true" && contasCache != null)
            return contasCache;

        var contas = await _contaRepository.ObterTodosAsync();
        var contasDto = contas.Select(MapearParaDto).ToList();
        
        await _cacheService.DefinirAsync(CACHE_KEY_CONTAS, contasDto, TimeSpan.FromHours(3));
        
        await _cacheService.DefinirAsync(CACHE_KEY_CONTAS_ATUALIZADAS, "true", TimeSpan.FromMinutes(165));
        
        return contasDto;
    }

    public async Task<ContaDto> CriarAsync(CriarContaDto dto)
    {
        dto.Cpf = CpfNormalizer.Normalize(dto.Cpf);
        var cpf = new Cpf(dto.Cpf);
        
        if (await _contaRepository.ExisteCpfAsync(cpf))
            throw new InvalidOperationException("Já existe uma conta com este CPF");

        var conta = new Conta(dto.NomeTitular, cpf);
        await _contaRepository.AdicionarAsync(conta);

        var contaDto = MapearParaDto(conta);
        
        var listaExistente = await _cacheService.ObterAsync<List<ContaDto>>(CACHE_KEY_CONTAS);
        if (listaExistente != null)
        {
            listaExistente.Add(contaDto);
            await _cacheService.AtualizarConteudoAsync(CACHE_KEY_CONTAS, listaExistente);
        }
        else
        {
            await _cacheService.DefinirAsync(CACHE_KEY_CONTAS, new List<ContaDto> { contaDto }, TimeSpan.FromHours(3));
        }
        
        
        await _notificationService.NotificarContaCriadaAsync(conta.Id, conta.NomeTitular, conta.Cpf.Valor);

        return contaDto;
    }

    public async Task<ContaDto> AtualizarAsync(Guid id, AtualizarContaDto dto)
    {
        var conta = await _contaRepository.ObterPorIdAsync(id);
        if (conta == null)
            throw new InvalidOperationException("Conta não encontrada");

        conta.AtualizarNomeTitular(dto.NomeTitular);
        await _contaRepository.AtualizarAsync(conta);

        var contaDto = MapearParaDto(conta);
        
        var listaExistente = await _cacheService.ObterAsync<List<ContaDto>>(CACHE_KEY_CONTAS);
        if (listaExistente != null)
        {
            await _cacheService.AtualizarItemNaListaAsync(CACHE_KEY_CONTAS, id, contaDto, c => c.Id);
        }
        else
        {
            await _cacheService.DefinirAsync(CACHE_KEY_CONTAS, new List<ContaDto> { contaDto }, TimeSpan.FromHours(3));
        }
        
        
        await _notificationService.NotificarContaAtualizadaAsync(conta.Id, conta.NomeTitular, conta.Cpf.Valor, conta.Status.ToString());

        return contaDto;
    }

    public async Task<bool> RemoverAsync(Guid id)
    {
        var conta = await _contaRepository.ObterPorIdAsync(id);
        if (conta == null)
            return false;

        await _contaRepository.RemoverAsync(id);

        var listaExistente = await _cacheService.ObterAsync<List<ContaDto>>(CACHE_KEY_CONTAS);
        if (listaExistente != null)
        {
            listaExistente.RemoveAll(c => c.Id == id);
            await _cacheService.AtualizarConteudoAsync(CACHE_KEY_CONTAS, listaExistente);
        }        
        
        await _notificationService.NotificarContaRemovidaAsync(id);

        return true;
    }

    public async Task<ContaDto> AtivarAsync(Guid id)
    {
        var conta = await _contaRepository.ObterPorIdAsync(id);
        if (conta == null)
            throw new InvalidOperationException("Conta não encontrada");

        conta.Ativar();
        await _contaRepository.AtualizarAsync(conta);

        var contaDto = MapearParaDto(conta);
        
        var listaExistente = await _cacheService.ObterAsync<List<ContaDto>>(CACHE_KEY_CONTAS);
        if (listaExistente != null)
        {
            await _cacheService.AtualizarItemNaListaAsync(CACHE_KEY_CONTAS, id, contaDto, c => c.Id);
        }
        else
        {
            await _cacheService.DefinirAsync(CACHE_KEY_CONTAS, new List<ContaDto> { contaDto }, TimeSpan.FromHours(3));
        }
        
        
        await _notificationService.NotificarContaAtualizadaAsync(conta.Id, conta.NomeTitular, conta.Cpf.Valor, conta.Status.ToString());

        return contaDto;
    }

    public async Task<ContaDto> InativarAsync(Guid id)
    {
        var conta = await _contaRepository.ObterPorIdAsync(id);
        if (conta == null)
            throw new InvalidOperationException("Conta não encontrada");

        conta.Inativar();
        await _contaRepository.AtualizarAsync(conta);

        var contaDto = MapearParaDto(conta);
        
        var listaExistente = await _cacheService.ObterAsync<List<ContaDto>>(CACHE_KEY_CONTAS);
        if (listaExistente != null)
        {
            await _cacheService.AtualizarItemNaListaAsync(CACHE_KEY_CONTAS, id, contaDto, c => c.Id);
        }
        else
        {
            await _cacheService.DefinirAsync(CACHE_KEY_CONTAS, new List<ContaDto> { contaDto }, TimeSpan.FromHours(3));
        }
        
        
        await _notificationService.NotificarContaAtualizadaAsync(conta.Id, conta.NomeTitular, conta.Cpf.Valor, conta.Status.ToString());

        return contaDto;
    }

    private static ContaDto MapearParaDto(Conta conta)
    {
        return new ContaDto
        {
            Id = conta.Id,
            NomeTitular = conta.NomeTitular,
            Cpf = conta.Cpf.ToString(),
            Status = conta.Status,
            DataCriacao = conta.DataCriacao,
            DataAtualizacao = conta.DataAtualizacao
        };
    }
}