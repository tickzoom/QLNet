using System;

namespace QLNet
{
	/// <summary>
	/// Binary cash-or-nothing payoff
	/// </summary>
	public class CashOrNothingPayoff : StrikedTypePayoff
	{
		private readonly double _cashPayoff;

		public CashOrNothingPayoff(Option.Type type, double strike, double cashPayoff)
			: base(type, strike)
		{
			_cashPayoff = cashPayoff;
		}

		public double cashPayoff()
		{
			return _cashPayoff;
		}

		public override string name()
		{
			return "CashOrNothing";
		}

		public override string description()
		{
			return base.description() + ", " + cashPayoff() + " cash payoff";
		}
		
		public override double value(double price)
		{
			switch (type_)
			{
				case Option.Type.Call:
					return (price - strike_ > 0.0 ? _cashPayoff : 0.0);

				case Option.Type.Put:
					return (strike_ - price > 0.0 ? _cashPayoff : 0.0);

				default:
					throw new ArgumentException("unknown/illegal option type");
			}
		}
	}
}