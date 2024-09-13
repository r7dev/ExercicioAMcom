using Dapper;
using NSubstitute;
using Questao5.Application.Commands.Requests;
using System.Data;
using Xunit;

namespace Questao5.Application.Commands.Handlers
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
		public async Task Handle_ShouldReturnTrue_WhenMovimentoIsInserted()
		{
			// Arrange
			var command = new MovimentarContaCommand
			{
				IdRequisicao = Guid.NewGuid().ToString(),
				IdContaCorrente = "B6BAFC09-6967-ED11-A567-055DFA4A16C9",
				TipoMovimento = "C",
				Valor = 100.00m
			};

			_dbConnection.ExecuteAsync(Arg.Any<string>(), Arg.Any<object>()).Returns(1);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.True(result.IsSuccess);
		}
	}
}
