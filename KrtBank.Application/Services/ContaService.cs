using KrtBank.Application.DTOs;
using KrtBank.Application.Interfaces;
using KrtBank.Domain.Entities;
using KrtBank.Domain.Interfaces;
using KrtBank.Domain.ValueObjects;

namespace KrtBank.Application.Services;

public class ContaService : IContaService
{
    private readonly IContaRepository _contaRepository;
    private readonly ICacheService _cacheService;
    private readonly INotificationService _notificationService;
    private const string CACHE_KEY_PREFIX = "conta:";
    private const string CACHE_KEY_ALL = "contas:todas";

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
        var cacheKey = $"{CACHE_KEY_PREFIX}{id}";
        var contaCache = await _cacheService.ObterAsync<ContaDto>(cacheKey);
        
        if (contaCache != null)
            return contaCache;

        var conta = await _contaRepository.ObterPorIdAsync(id);
        if (conta == null)
            return null;

        var contaDto = MapearParaDto(conta);
        await _cacheService.DefinirAsync(cacheKey, contaDto, TimeSpan.FromHours(3));
        
        return contaDto;
    }

    public async Task<IEnumerable<ContaDto>> ObterTodasAsync()
    {
        var contasCache = await _cacheService.ObterAsync<IEnumerable<ContaDto>>(CACHE_KEY_ALL);
        
        if (contasCache != null)
            return contasCache;

        var contas = await _contaRepository.ObterTodosAsync();
        var contasDto = contas.Select(MapearParaDto);
        
        await _cacheService.DefinirAsync(CACHE_KEY_ALL, contasDto.ToList(), TimeSpan.FromHours(3));
        
        return contasDto;
    }

    public async Task<ContaDto> CriarAsync(CriarContaDto dto)
    {
        var cpf = new Cpf(dto.Cpf);
        
        if (await _contaRepository.ExisteCpfAsync(cpf))
            throw new InvalidOperationException("Já existe uma conta com este CPF");

        var conta = new Conta(dto.NomeTitular, cpf);
        await _contaRepository.AdicionarAsync(conta);

        await _notificationService.NotificarContaCriadaAsync(conta.Id, conta.NomeTitular, conta.Cpf.Valor);

        return MapearParaDto(conta);
    }

    public async Task<ContaDto> AtualizarAsync(Guid id, AtualizarContaDto dto)
    {
        var conta = await _contaRepository.ObterPorIdAsync(id);
        if (conta == null)
            throw new InvalidOperationException("Conta não encontrada");

        conta.AtualizarNomeTitular(dto.NomeTitular);
        await _contaRepository.AtualizarAsync(conta);

        var contaDto = MapearParaDto(conta);
        
        await _cacheService.AtualizarItemNaListaAsync(CACHE_KEY_ALL, id, contaDto, c => c.Id);
        
        await _notificationService.NotificarContaAtualizadaAsync(conta.Id, conta.NomeTitular, conta.Cpf.Valor, conta.Status.ToString());

        return contaDto;
    }

    public async Task<bool> RemoverAsync(Guid id)
    {
        var conta = await _contaRepository.ObterPorIdAsync(id);
        if (conta == null)
            return false;

        await _contaRepository.RemoverAsync(id);

        await _cacheService.RemoverAsync($"{CACHE_KEY_PREFIX}{id}");
        await _cacheService.RemoverPorPadraoAsync(CACHE_KEY_ALL);
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
        
        await _cacheService.AtualizarItemNaListaAsync(CACHE_KEY_ALL, id, contaDto, c => c.Id);
        
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
        
        await _cacheService.AtualizarItemNaListaAsync(CACHE_KEY_ALL, id, contaDto, c => c.Id);
        
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