using KrtBank.Application.DTOs;
using KrtBank.Application.Interfaces;
using KrtBank.Application.Services;
using KrtBank.Domain.Entities;
using KrtBank.Domain.Interfaces;
using KrtBank.Domain.ValueObjects;
using Moq;
using Xunit;

namespace KrtBank.Tests.Services;

public class ContaServiceTests
{
    private readonly Mock<IContaRepository> _contaRepositoryMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly ContaService _contaService;

    public ContaServiceTests()
    {
        _contaRepositoryMock = new Mock<IContaRepository>();
        _cacheServiceMock = new Mock<ICacheService>();
        _notificationServiceMock = new Mock<INotificationService>();
        
        _contaService = new ContaService(
            _contaRepositoryMock.Object,
            _cacheServiceMock.Object,
            _notificationServiceMock.Object);
    }

    [Fact]
    public async Task CriarAsync_DeveCriarContaComSucesso()
    {
        // Arrange
        var dto = new CriarContaDto
        {
            NomeTitular = "João Silva",
            Cpf = "111.444.777-35"
        };

        var cpf = new Cpf(dto.Cpf);
        _contaRepositoryMock.Setup(x => x.ExisteCpfAsync(It.IsAny<Cpf>()))
            .ReturnsAsync(false);

        Conta? contaSalva = null;
        _contaRepositoryMock.Setup(x => x.AdicionarAsync(It.IsAny<Conta>()))
            .Callback<Conta>(c => contaSalva = c)
            .ReturnsAsync((Conta c) => c);

        // Act
        var resultado = await _contaService.CriarAsync(dto);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(dto.NomeTitular, resultado.NomeTitular);
        Assert.Equal(dto.Cpf, resultado.Cpf);
        Assert.Equal(Domain.Enums.StatusConta.Ativa, resultado.Status);
        
        _contaRepositoryMock.Verify(x => x.ExisteCpfAsync(It.IsAny<Cpf>()), Times.Once);
        _contaRepositoryMock.Verify(x => x.AdicionarAsync(It.IsAny<Conta>()), Times.Once);
        _notificationServiceMock.Verify(x => x.NotificarContaCriadaAsync(
            It.IsAny<Guid>(), 
            It.IsAny<string>(), 
            It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_DeveLancarExcecaoQuandoCpfJaExiste()
    {
        // Arrange
        var dto = new CriarContaDto
        {
            NomeTitular = "João Silva",
            Cpf = "111.444.777-35"
        };

        _contaRepositoryMock.Setup(x => x.ExisteCpfAsync(It.IsAny<Cpf>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _contaService.CriarAsync(dto));
        
        _contaRepositoryMock.Verify(x => x.ExisteCpfAsync(It.IsAny<Cpf>()), Times.Once);
        _contaRepositoryMock.Verify(x => x.AdicionarAsync(It.IsAny<Conta>()), Times.Never);
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarContaDoCache()
    {
        // Arrange
        var id = Guid.NewGuid();
        var contaDto = new ContaDto
        {
            Id = id,
            NomeTitular = "João Silva",
            Cpf = "111.444.777-35",
            Status = Domain.Enums.StatusConta.Ativa
        };

        _cacheServiceMock.Setup(x => x.ObterAsync<List<ContaDto>>("contasCache"))
            .ReturnsAsync(new List<ContaDto> { contaDto });

        // Act
        var resultado = await _contaService.ObterPorIdAsync(id);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(contaDto.Id, resultado.Id);
        Assert.Equal(contaDto.NomeTitular, resultado.NomeTitular);
        
        _cacheServiceMock.Verify(x => x.ObterAsync<List<ContaDto>>("contasCache"), Times.Once);
        _contaRepositoryMock.Verify(x => x.ObterPorIdAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveBuscarDoRepositorioQuandoNaoExisteNoCache()
    {
        // Arrange
        var id = Guid.NewGuid();
        var conta = new Conta("João Silva", new Cpf("111.444.777-35"));

        _cacheServiceMock.Setup(x => x.ObterAsync<List<ContaDto>>("contasCache"))
            .ReturnsAsync((List<ContaDto>?)null);

        _contaRepositoryMock.Setup(x => x.ObterPorIdAsync(id))
            .ReturnsAsync(conta);

        // Act
        var resultado = await _contaService.ObterPorIdAsync(id);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(conta.Id, resultado.Id);
        Assert.Equal(conta.NomeTitular, resultado.NomeTitular);
        
        _cacheServiceMock.Verify(x => x.ObterAsync<List<ContaDto>>("contasCache"), Times.Once);
        _contaRepositoryMock.Verify(x => x.ObterPorIdAsync(id), Times.Once);
        _cacheServiceMock.Verify(x => x.DefinirAsync("contasCache", It.IsAny<List<ContaDto>>(), It.IsAny<TimeSpan>()), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_DeveAtualizarContaComSucesso()
    {
        // Arrange
        var id = Guid.NewGuid();
        var conta = new Conta("João Silva", new Cpf("111.444.777-35"));
        var dto = new AtualizarContaDto { NomeTitular = "João Silva Santos" };

        _contaRepositoryMock.Setup(x => x.ObterPorIdAsync(id))
            .ReturnsAsync(conta);

        _cacheServiceMock.Setup(x => x.ObterAsync<List<ContaDto>>("contasCache"))
            .ReturnsAsync(new List<ContaDto>());

        // Act
        var resultado = await _contaService.AtualizarAsync(id, dto);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(dto.NomeTitular, resultado.NomeTitular);
        
        _contaRepositoryMock.Verify(x => x.ObterPorIdAsync(id), Times.Once);
        _contaRepositoryMock.Verify(x => x.AtualizarAsync(conta), Times.Once);
        _cacheServiceMock.Verify(x => x.AtualizarItemNaListaAsync(
            It.IsAny<string>(), 
            It.IsAny<Guid>(), 
            It.IsAny<ContaDto>(), 
            It.IsAny<Func<ContaDto, Guid>>()), Times.Once);
        _notificationServiceMock.Verify(x => x.NotificarContaAtualizadaAsync(
            It.IsAny<Guid>(), 
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_DeveLancarExcecaoQuandoContaNaoExiste()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new AtualizarContaDto { NomeTitular = "João Silva Santos" };

        _contaRepositoryMock.Setup(x => x.ObterPorIdAsync(id))
            .ReturnsAsync((Conta?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _contaService.AtualizarAsync(id, dto));
        
        _contaRepositoryMock.Verify(x => x.ObterPorIdAsync(id), Times.Once);
        _contaRepositoryMock.Verify(x => x.AtualizarAsync(It.IsAny<Conta>()), Times.Never);
    }

    [Fact]
    public async Task RemoverAsync_DeveRemoverContaComSucesso()
    {
        // Arrange
        var id = Guid.NewGuid();
        var conta = new Conta("João Silva", new Cpf("111.444.777-35"));

        _contaRepositoryMock.Setup(x => x.ObterPorIdAsync(id))
            .ReturnsAsync(conta);

        _cacheServiceMock.Setup(x => x.ObterAsync<List<ContaDto>>("contasCache"))
            .ReturnsAsync(new List<ContaDto>());

        // Act
        var resultado = await _contaService.RemoverAsync(id);

        // Assert
        Assert.True(resultado);
        
        _contaRepositoryMock.Verify(x => x.RemoverAsync(id), Times.Once);
        _cacheServiceMock.Verify(x => x.ObterAsync<List<ContaDto>>("contasCache"), Times.Once);
        _cacheServiceMock.Verify(x => x.AtualizarConteudoAsync("contasCache", It.IsAny<List<ContaDto>>()), Times.Once);
        _notificationServiceMock.Verify(x => x.NotificarContaRemovidaAsync(id), Times.Once);
    }

    [Fact]
    public async Task RemoverAsync_DeveRetornarFalseQuandoContaNaoExiste()
    {
        // Arrange
        var id = Guid.NewGuid();

        _contaRepositoryMock.Setup(x => x.ObterPorIdAsync(id))
            .ReturnsAsync((Conta?)null);

        // Act
        var resultado = await _contaService.RemoverAsync(id);

        // Assert
        Assert.False(resultado);
        
        _contaRepositoryMock.Verify(x => x.RemoverAsync(It.IsAny<Guid>()), Times.Never);
    }
}
