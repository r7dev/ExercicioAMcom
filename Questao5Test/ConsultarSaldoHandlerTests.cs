using System.Data;
using Domain.Entities;
using NSubstitute;
using Questao5.Application.Queries.Handlers;
using Questao5.Application.Queries.Requests;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Services;
using Xunit;

namespace Questao5Test
{
	public class ConsultarSaldoHandlerTests
	{
		private readonly IDapperWrapper _mockDapperWrapper;
		private readonly IDbConnection _mockDbConnection;
		private readonly ConsultarSaldoHandler _handler;

		public ConsultarSaldoHandlerTests()
		{
			_mockDbConnection = Substitute.For<IDbConnection>();
			_mockDapperWrapper = Substitute.For<IDapperWrapper>();
			_handler = new ConsultarSaldoHandler(_mockDbConnection, _mockDapperWrapper);
		}

		[Fact]
		public async Task Handle_ShouldReturnInvalidAccount_WhenAccountDoesNotExist()
		{
			// Arrange
			var query = new ConsultarSaldoQuery { IdContaCorrente = (Guid.NewGuid()).ToString() };
			_mockDapperWrapper.QuerySingleOrDefaultAsync<ContaCorrente>(_mockDbConnection, Arg.Any<string>(), Arg.Any<object>())
				.Returns(Task.FromResult<ContaCorrente>(null));

			// Act
			var result = await _handler.Handle(query, CancellationToken.None);

			// Assert
			Xunit.Assert.False(result.IsSuccess);
			Xunit.Assert.Equal("INVALID_ACCOUNT", result.ErrorCode);
		}

		[Fact]
		public async Task Handle_ShouldReturnInactiveAccount_WhenAccountIsInactive()
		{
			// Arrange
			var query = new ConsultarSaldoQuery { IdContaCorrente = (Guid.NewGuid()).ToString() };
			var conta = new ContaCorrente { Ativo = 0 };
			_mockDapperWrapper.QuerySingleOrDefaultAsync<ContaCorrente>(_mockDbConnection, Arg.Any<string>(), Arg.Any<object>())
				.Returns(Task.FromResult(conta));

			// Act
			var result = await _handler.Handle(query, CancellationToken.None);

			// Assert
			Xunit.Assert.False(result.IsSuccess);
			Xunit.Assert.Equal("INACTIVE_ACCOUNT", result.ErrorCode);
		}

		[Fact]
		public async Task Handle_ShouldReturnCorrectBalance_WhenAccountIsActive()
		{
			// Arrange
			var query = new ConsultarSaldoQuery { IdContaCorrente = (Guid.NewGuid()).ToString() };
			var conta = new ContaCorrente { Ativo = 1, Numero = 12345, Nome = "Test Account" };
			_mockDapperWrapper.QuerySingleOrDefaultAsync<ContaCorrente>(_mockDbConnection, Arg.Any<string>(), Arg.Any<object>())
				.Returns(Task.FromResult(conta));
			_mockDapperWrapper.QuerySingleOrDefaultAsync<decimal>(_mockDbConnection, Arg.Any<string>(), Arg.Any<object>())
				.Returns(Task.FromResult(100.0m));

			// Act
			var result = await _handler.Handle(query, CancellationToken.None);

			// Assert
			var resultCS = result.Value as ResultadoConsultaSaldo;

			Xunit.Assert.True(result.IsSuccess);
			Xunit.Assert.Equal(12345, resultCS.Numero);
			Xunit.Assert.Equal("Test Account", resultCS.Nome);
			Xunit.Assert.Equal(100.0m, resultCS.Saldo);
		}
	}
}
