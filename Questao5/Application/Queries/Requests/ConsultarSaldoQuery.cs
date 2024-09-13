using Application.Common;
using MediatR;

namespace Questao5.Application.Queries.Requests
{
	public class ConsultarSaldoQuery : IRequest<Result>
	{
		public string IdContaCorrente { get; set; }
	}
}
