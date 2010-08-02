using System;

namespace QLNet
{
	/// <summary>
	/// Binary gap payoff
	/// 
	/// This payoff is equivalent to being a) long a PlainVanillaPayoff at
	/// the first strike (same Call/Put type) and b) short a
	/// CashOrNothingPayoff at the first strike (same Call/Put type) with
	/// cash payoff equal to the difference between the second and the first
	/// strike.
	/// </summary>
	/// <remarks>
	/// This payoff can be negative depending on the strikes
	/// </remarks>
	public class GapPayoff : StrikedTypePayoff
	{
		private readonly double _secondStrike;

		public GapPayoff(Option.Type type, double strike, double secondStrike) // a.k.a. payoff strike
			: base(type, strike)
		{
			_secondStrike = secondStrike;
		}

		public double secondStrike()
		{
			return _secondStrike;
		}

		public override string name()
		{
			return "Gap";
		}

		public override string description()
		{
			return base.description() + ", " + secondStrike() + " strike payoff";
		}
		
		public override double value(double price)
		{
			switch (type_)
			{
				case Option.Type.Call:
					return (price - strike_ >= 0.0 ? price - _secondStrike : 0.0);

				case Option.Type.Put:
					return (strike_ - price >= 0.0 ? _secondStrike - price : 0.0);

				default:
					throw new ArgumentException("unknown/illegal option type");
			}
		}
	}
}