using MediatR;
using Microsoft.AspNetCore.Mvc;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Queries.Requests;
using Swashbuckle.AspNetCore.Annotations;

namespace Questao5.Infrastructure.Services.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ContaCorrenteController : ControllerBase
	{
		private readonly IMediator _mediator;

		public ContaCorrenteController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpPost("movimentar")]
		[SwaggerOperation(Summary = "Movimenta uma conta corrente", Description = "Realiza um crédito ou débito na conta corrente.")]
		public async Task<IActionResult> MovimentarConta([FromBody] MovimentarContaCommand command)
		{
			var result = await _mediator.Send(command);
			if (result.IsSuccess)
				return Ok(result.Value);
			return BadRequest(new { result.ErrorCode, result.ErrorMessage });
		}

		[HttpGet("saldo/{idContaCorrente}")]
		[SwaggerOperation(Summary = "Consulta o saldo da conta corrente", Description = "Retorna o saldo atual da conta corrente.")]
		public async Task<IActionResult> ConsultarSaldo(string idContaCorrente)
		{
			var query = new ConsultarSaldoQuery { IdContaCorrente = idContaCorrente };
			var result = await _mediator.Send(query);
			if (result.IsSuccess)
				return Ok(result.Value);
			return BadRequest(new { result.ErrorCode, result.ErrorMessage });
		}
	}
}
