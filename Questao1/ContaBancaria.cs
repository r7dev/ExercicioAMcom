using System;
using System.Globalization;

namespace Questao1
{
	class ContaBancaria {

		public int Numero { get; private set; }
		public string Titular { get; set; }
		public double Saldo { get; private set; }

		private const double _taxaDeSaque = 3.50;

		public ContaBancaria(int numero, string titular)
		{
			Numero = numero;
			Titular = titular;
			Saldo = 0.00;
		}

		public ContaBancaria(int numero, string titular, double depositoInicial) : this(numero, titular)
		{
			Deposito(depositoInicial);
		}

		internal void Deposito(double quantia)
		{
			Saldo += quantia;
		}

		internal void Saque(double quantia)
		{
			Saldo -= quantia + _taxaDeSaque;
		}

		public override string ToString()
		{
			return "Conta " + Numero
				+ ", Titular: " + Titular
				+ ", Saldo: $ " + Saldo.ToString("F2", CultureInfo.InvariantCulture);
		}
	}
}
