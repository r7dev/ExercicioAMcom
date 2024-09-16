using Application.Common;
using Dapper;
using Domain.Entities;
using MediatR;
using Newtonsoft.Json;
using Questao5.Application.Commands.Requests;
using Questao5.Infrastructure.Services;
using System.Data;
using System.Resources;

namespace Questao5.Application.Commands.Handlers
{
	public class MovimentarContaHandler : IRequestHandler<MovimentarContaCommand, Result>
	{
		private readonly IDbConnection _dbConnection;
		private readonly IDapperWrapper _dapperWrapper;

		public MovimentarContaHandler(IDbConnection dbConnection, IDapperWrapper dapperWrapper)
		{
			_dbConnection = dbConnection;
			_dapperWrapper = dapperWrapper;
		}

		public async Task<Result> Handle(MovimentarContaCommand request, CancellationToken cancellationToken)
		{
			ResourceManager rm = new ResourceManager("Questao5.Domain.Language.Error", typeof(MovimentarContaHandler).Assembly);

			// Validações de negócio
			var conta = await _dapperWrapper.QuerySingleOrDefaultAsync<ContaCorrente>(
				_dbConnection,
				"SELECT * FROM contacorrente WHERE idcontacorrente = @IdContaCorrente",
				new { request.IdContaCorrente });

			if (conta == null)
				return Result.Fail("INVALID_ACCOUNT", rm.GetString("INVALID_ACCOUNT"));

			if (conta.Ativo == 0)
				return Result.Fail("INACTIVE_ACCOUNT", rm.GetString("INACTIVE_ACCOUNT"));

			if (request.Valor <= 0)
				return Result.Fail("INVALID_VALUE", rm.GetString("INVALID_VALUE"));

			if (request.TipoMovimento != ((char)TipoMovimento.Credito).ToString()
				&& request.TipoMovimento != ((char)TipoMovimento.Debito).ToString())
				return Result.Fail("INVALID_TYPE", rm.GetString("INVALID_TYPE"));

			// Verificar idempotência
			var idempotencia = await _dapperWrapper.QuerySingleOrDefaultAsync<Idempotencia>(
				_dbConnection,
				"SELECT * FROM idempotencia WHERE chave_idempotencia = @IdRequisicao",
				new { request.IdRequisicao });

			if (idempotencia != null)
				return Result.Ok(idempotencia.Resultado);

			// Inserir movimento
			var idMovimento = Guid.NewGuid().ToString();
			var sql = @"INSERT INTO movimento (idmovimento, idcontacorrente, datamovimento, tipomovimento, valor)
						VALUES (@IdMovimento, @IdContaCorrente, @DataMovimento, @TipoMovimento, @Valor)";

			var parameters = new
			{
				IdMovimento = idMovimento,
				request.IdContaCorrente,
				DataMovimento = DateTime.Now.ToString("dd/MM/yyyy"),
				request.TipoMovimento,
				request.Valor
			};

			await _dapperWrapper.ExecuteAsync(_dbConnection, sql, parameters);

			// Salvar idempotência
			var resultado = new { IdMovimento = idMovimento };
			await _dapperWrapper.ExecuteAsync(
				_dbConnection,
				@"INSERT INTO idempotencia (chave_idempotencia, requisicao, resultado)
					  VALUES (@ChaveIdempotencia, @Requisicao, @Resultado)",
				new
				{
					ChaveIdempotencia = request.IdRequisicao,
					Requisicao = JsonConvert.SerializeObject(request),
					Resultado = JsonConvert.SerializeObject(resultado)
				});

			return Result.Ok(resultado);
		}
	}
}