using Application.Common;
using MediatR;

namespace Questao5.Application.Commands.Requests
{
	public class MovimentarContaCommand : IRequest<Result>
	{
		public string IdRequisicao { get; set; }
		public string IdContaCorrente { get; set; }
		public string TipoMovimento { get; set; } // 'C' para crédito, 'D' para débito
		public decimal Valor { get; set; }
	}
}
