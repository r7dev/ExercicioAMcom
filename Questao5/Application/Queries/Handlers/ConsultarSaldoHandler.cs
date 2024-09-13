using Application.Common;
using Dapper;
using Domain.Entities;
using MediatR;
using Questao5.Application.Commands.Handlers;
using Questao5.Application.Queries.Requests;
using System.Data;
using System.Resources;

namespace Questao5.Application.Queries.Handlers
{
	public class ConsultarSaldoHandler : IRequestHandler<ConsultarSaldoQuery, Result>
	{
		private readonly IDbConnection _dbConnection;

		public ConsultarSaldoHandler(IDbConnection dbConnection)
		{
			_dbConnection = dbConnection;
		}

		public async Task<Result> Handle(ConsultarSaldoQuery request, CancellationToken cancellationToken)
		{
			ResourceManager rm = new ResourceManager("Questao5.Domain.Language.Error", typeof(MovimentarContaHandler).Assembly);

			// Validações de negócio
			var conta = await _dbConnection.QuerySingleOrDefaultAsync<ContaCorrente>(
				"SELECT * FROM contacorrente WHERE idcontacorrente = @IdContaCorrente",
				new { request.IdContaCorrente });

			if (conta == null)
				return Result.Fail("INVALID_ACCOUNT", rm.GetString("INVALID_ACCOUNT"));

			if (conta.Ativo == 0)
				return Result.Fail("INACTIVE_ACCOUNT", rm.GetString("INACTIVE_ACCOUNT"));

			// Calcular saldo
			var saldo = await _dbConnection.QuerySingleOrDefaultAsync<decimal>(
				$@"SELECT 
						   COALESCE(SUM(CASE WHEN tipomovimento = '{(char)TipoMovimento.Credito}' THEN valor ELSE -valor END), 0.0)
					   FROM movimento 
					   WHERE idcontacorrente = @IdContaCorrente",
				new { request.IdContaCorrente });

			var resultado = new
			{
				Numero = conta.Numero,
				Nome = conta.Nome,
				DataHora = DateTime.Now,
				Saldo = saldo
			};

			return Result.Ok(resultado);
		}
	}

}
