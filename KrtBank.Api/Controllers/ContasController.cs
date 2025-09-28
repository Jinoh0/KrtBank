using KrtBank.Application.DTOs;
using KrtBank.Application.Interfaces;
using KrtBank.Application.Utils;
using Microsoft.AspNetCore.Mvc;

namespace KrtBank.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContasController : ControllerBase
{
    private readonly IContaService _contaService;
    private readonly ILogger<ContasController> _logger;

    public ContasController(IContaService contaService, ILogger<ContasController> logger)
    {
        _contaService = contaService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ContaDto>>> ObterTodas()
    {
        try
        {
            _logger.LogInformation("API: Retrieving all accounts");
            var contas = await _contaService.ObterTodasAsync();
            _logger.LogInformation("API: {Count} accounts returned", contas.Count());
            return Ok(contas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API: Critical error retrieving all accounts");
            return StatusCode(500, new { 
                error = "Erro interno do servidor", 
                message = "Falha ao recuperar lista de contas",
                timestamp = DateTime.UtcNow
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ContaDto>> ObterPorId(Guid id)
    {
        try
        {
            _logger.LogInformation("API: Retrieving account by ID: {Id}", id);
            var conta = await _contaService.ObterPorIdAsync(id);
            if (conta == null)
            {
                _logger.LogWarning("API: Account not found: {Id}", id);
                return NotFound(new { 
                    error = "Conta não encontrada", 
                    id = id,
                    timestamp = DateTime.UtcNow
                });
            }

            _logger.LogInformation("API: Account found: {Id} - {NomeTitular}", id, conta.NomeTitular);
            return Ok(conta);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API: Critical error retrieving account: {Id}", id);
            return StatusCode(500, new { 
                error = "Erro interno do servidor", 
                message = "Falha ao recuperar conta",
                id = id,
                timestamp = DateTime.UtcNow
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult<ContaDto>> Criar([FromBody] CriarContaDto dto)
    {
        try
        {
            _logger.LogInformation("API: Starting account creation for CPF: {Cpf}", dto.Cpf);
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("API: Invalid data received for account creation: {Cpf}", dto.Cpf);
                return BadRequest(new { 
                    error = "Dados inválidos", 
                    details = ModelState,
                    timestamp = DateTime.UtcNow
                });
            }

            dto.Cpf = CpfNormalizer.Normalize(dto.Cpf);
            _logger.LogInformation("API: CPF normalized to: {Cpf}", dto.Cpf);

            var conta = await _contaService.CriarAsync(dto);
            _logger.LogInformation("API: Account created successfully: {Id} - {NomeTitular}", conta.Id, conta.NomeTitular);
            return CreatedAtAction(nameof(ObterPorId), new { id = conta.Id }, conta);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("API: Attempt to create account with duplicate CPF: {Cpf}", dto.Cpf);
            return BadRequest(new { 
                error = "CPF já cadastrado", 
                message = ex.Message,
                cpf = dto.Cpf,
                timestamp = DateTime.UtcNow
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("API: Invalid CPF format for account creation: {Cpf} - {Message}", dto.Cpf, ex.Message);
            return BadRequest(new { 
                error = "Formato de CPF inválido", 
                message = ex.Message,
                cpf = dto.Cpf,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API: Critical error creating account for CPF: {Cpf}", dto.Cpf);
            return StatusCode(500, new { 
                error = "Erro interno do servidor", 
                message = "Falha ao criar conta",
                cpf = dto.Cpf,
                timestamp = DateTime.UtcNow
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ContaDto>> Atualizar(Guid id, [FromBody] AtualizarContaDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var conta = await _contaService.AtualizarAsync(id, dto);
            return Ok(conta);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Conta não encontrada para atualização: {Id}", id);
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Dados inválidos para atualização de conta: {Id}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar conta {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Remover(Guid id)
    {
        try
        {
            var removida = await _contaService.RemoverAsync(id);
            if (!removida)
                return NotFound("Conta não encontrada");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover conta {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPatch("{id}/ativar")]
    public async Task<ActionResult<ContaDto>> Ativar(Guid id)
    {
        try
        {
            var conta = await _contaService.AtivarAsync(id);
            return Ok(conta);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Conta não encontrada para ativação: {Id}", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao ativar conta {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPatch("{id}/inativar")]
    public async Task<ActionResult<ContaDto>> Inativar(Guid id)
    {
        try
        {
            var conta = await _contaService.InativarAsync(id);
            return Ok(conta);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Conta não encontrada para inativação: {Id}", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao inativar conta {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }
}

