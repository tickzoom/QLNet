using System;

namespace QLNet
{
	/// <summary>
	/// Binary supershare payoff
	/// </summary>
	public class SuperSharePayoff : StrikedTypePayoff
	{
		private readonly double _secondStrike;
		private readonly double _cashPayoff;

		public SuperSharePayoff(double strike, double secondStrike, double cashPayoff)
			: base(Option.Type.Call, strike)
		{
			if (!(secondStrike > strike))
			{
				throw new ApplicationException("second strike (" + secondStrike + ") must be higher than first strike (" + strike + ")");
			} 
			
			_secondStrike = secondStrike;
			_cashPayoff = cashPayoff;
		}

		public double secondStrike()
		{
			return _secondStrike;
		}

		public double cashPayoff()
		{
			return _cashPayoff;
		}

		public override string name()
		{
			return "SuperShare";
		}

		public override string description()
		{
			return base.description() + ", " + secondStrike() + " second strike" + ", " + cashPayoff() + " amount"; ;
		}

		public override double value(double price)
		{
			return (price >= strike_ && price < _secondStrike) ? _cashPayoff : 0.0;
		}
	}
}