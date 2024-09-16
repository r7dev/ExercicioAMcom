using System.Data;
using Domain.Entities;
using NSubstitute;
using Questao5.Application.Commands.Handlers;
using Questao5.Application.Commands.Requests;
using Questao5.Infrastructure.Services;
using Xunit;

namespace Questao5Test
{
	public class MovimentarContaHandlerTests
	{
		private readonly IDapperWrapper _mockDapperWrapper;
		private readonly IDbConnection _mockDbConnection;
		private readonly MovimentarContaHandler _handler;

		public MovimentarContaHandlerTests()
		{
			_mockDbConnection = Substitute.For<IDbConnection>();
			_mockDapperWrapper = Substitute.For<IDapperWrapper>();
			_handler = new MovimentarContaHandler(_mockDbConnection, _mockDapperWrapper);
		}

		[Fact]
		public async Task Handle_DeveRetornarErro_QuandoContaInvalida()
		{
			// Arrange
			var command = new MovimentarContaCommand
			{
				IdRequisicao = Guid.NewGuid().ToString(),
				IdContaCorrente = Guid.NewGuid().ToString(),
				Valor = 100.00M,
				TipoMovimento = "C"
			};

			_mockDapperWrapper.QuerySingleOrDefaultAsync<ContaCorrente?>(_mockDbConnection, Arg.Any<string>(), Arg.Any<object>())
				.Returns((ContaCorrente?)null);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Xunit.Assert.False(result.IsSuccess);
			Xunit.Assert.Equal("INVALID_ACCOUNT", result.ErrorCode);
		}

		[Fact]
		public async Task Handle_DeveRetornarErro_QuandoContaInativa()
		{
			// Arrange
			var command = new MovimentarContaCommand
			{
				IdRequisicao = Guid.NewGuid().ToString(),
				IdContaCorrente = Guid.NewGuid().ToString(),
				Valor = 100.00M,
				TipoMovimento = "C"
			};
			var conta = new ContaCorrente { Ativo = 0 };

			_mockDapperWrapper.QuerySingleOrDefaultAsync<ContaCorrente>(_mockDbConnection, Arg.Any<string>(), Arg.Any<object>())
				.Returns(Task.FromResult(conta));

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Xunit.Assert.False(result.IsSuccess);
			Xunit.Assert.Equal("INACTIVE_ACCOUNT", result.ErrorCode);
		}

		[Fact]
		public async Task Handle_DeveRetornarErro_QuandoValorInvalido()
		{
			// Arrange
			var command = new MovimentarContaCommand
			{
				IdRequisicao = Guid.NewGuid().ToString(),
				IdContaCorrente = Guid.NewGuid().ToString(),
				Valor = 0,
				TipoMovimento = "C"
			};
			var conta = new ContaCorrente { Ativo = 1 };

			_mockDapperWrapper.QuerySingleOrDefaultAsync<ContaCorrente>(_mockDbConnection, Arg.Any<string>(), Arg.Any<object>())
				.Returns(Task.FromResult(conta));

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Xunit.Assert.False(result.IsSuccess);
			Xunit.Assert.Equal("INVALID_VALUE", result.ErrorCode);
		}

		[Fact]
		public async Task Handle_DeveRetornarErro_QuandoTipoInvalido()
		{
			// Arrange
			var command = new MovimentarContaCommand
			{
				IdRequisicao = Guid.NewGuid().ToString(),
				IdContaCorrente = Guid.NewGuid().ToString(),
				Valor = 100.00M,
				TipoMovimento = "F"
			};
			var conta = new ContaCorrente { Ativo = 1 };

			_mockDapperWrapper.QuerySingleOrDefaultAsync<ContaCorrente>(_mockDbConnection, Arg.Any<string>(), Arg.Any<object>())
				.Returns(Task.FromResult(conta));

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Xunit.Assert.False(result.IsSuccess);
			Xunit.Assert.Equal("INVALID_TYPE", result.ErrorCode);
		}

		[Fact]
		public async Task Handle_DeveRetornarSucesso_QuandoMovimentoValido()
		{
			// Arrange
			var command = new MovimentarContaCommand
			{
				IdRequisicao = Guid.NewGuid().ToString(),
				IdContaCorrente = Guid.NewGuid().ToString(),
				Valor = 100.00M,
				TipoMovimento = "C"
			};

			var conta = new ContaCorrente
			{
				IdContaCorrente = Guid.NewGuid().ToString(),
				Numero = 12345,
				Nome = "Test Account",
				Ativo = 1
			};

			_mockDapperWrapper.QuerySingleOrDefaultAsync<ContaCorrente>(_mockDbConnection, Arg.Any<string>(), Arg.Any<object>())
						 .Returns(conta);

			_mockDapperWrapper.QuerySingleOrDefaultAsync<Idempotencia?>(_mockDbConnection, Arg.Any<string>(), Arg.Any<object>())
						 .Returns((Idempotencia?)null);

			_mockDapperWrapper.ExecuteAsync(_mockDbConnection, Arg.Any<string>(), Arg.Any<object>())
						 .Returns(Task.FromResult(1));

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Xunit.Assert.True(result.IsSuccess);
			Xunit.Assert.NotNull(result.Value);
		}
	}
}
