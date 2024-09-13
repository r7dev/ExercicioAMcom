using System.Data;
using Dapper;
using Domain.Entities;
using NSubstitute;
using Questao5.Application.Commands.Handlers;
using Questao5.Application.Commands.Requests;
using Xunit;

namespace Questao5Test.Mock
{
	public class MovimentarContaHandlerTests
	{
		private readonly IDbConnection _dbConnection;
		private readonly MovimentarContaHandler _handler;

		public MovimentarContaHandlerTests()
		{
			_dbConnection = Substitute.For<IDbConnection>();
			_handler = new MovimentarContaHandler(_dbConnection);
		}

		[Fact]
		public async Task Handle_DeveRetornarErro_QuandoContaInvalida()
		{
			// Arrange
			var command = new MovimentarContaCommand
			{
				IdRequisicao = Guid.NewGuid().ToString(),
				IdContaCorrente = "A6BAFC09-6967-ED11-A567-055DFA4A16C9",
				Valor = 100.00M,
				TipoMovimento = "C"
			};

			_dbConnection.QuerySingleOrDefaultAsync<ContaCorrente>(Arg.Any<string>(), Arg.Any<object>())
						 .Returns((ContaCorrente)null);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Xunit.Assert.False(result.IsSuccess);
			Xunit.Assert.Equal("INVALID_ACCOUNT", result.ErrorCode);
		}

		[Fact]
		public async Task Handle_DeveRetornarSucesso_QuandoMovimentoValido()
		{
			// Arrange
			var command = new MovimentarContaCommand
			{
				IdRequisicao = Guid.NewGuid().ToString(),
				IdContaCorrente = "B6BAFC09-6967-ED11-A567-055DFA4A16C9",
				Valor = 100.00M,
				TipoMovimento = "C"
			};

			var conta = new ContaCorrente
			{
				IdContaCorrente = "B6BAFC09-6967-ED11-A567-055DFA4A16C9",
				Numero = 123,
				Nome = "Katherine Sanchez",
				Ativo = 1
			};

			_dbConnection.QuerySingleOrDefaultAsync<ContaCorrente>(Arg.Any<string>(), Arg.Any<object>())
						 .Returns(conta);

			_dbConnection.QuerySingleOrDefaultAsync<Idempotencia>(Arg.Any<string>(), Arg.Any<object>())
						 .Returns((Idempotencia)null);

			_dbConnection.ExecuteAsync(Arg.Any<string>(), Arg.Any<object>())
						 .Returns(Task.FromResult(1));

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Xunit.Assert.True(result.IsSuccess);
			Xunit.Assert.NotNull(result.Value);
		}
	}
}
