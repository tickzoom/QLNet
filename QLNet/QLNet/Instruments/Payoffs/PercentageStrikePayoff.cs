using System;

namespace QLNet
{
	/// <summary>
	/// Payoff with strike expressed as percentage
	/// </summary>
	public class PercentageStrikePayoff : StrikedTypePayoff
	{
		public PercentageStrikePayoff(Option.Type type, double moneyness)
			: base(type, moneyness)
		{
		}

		public override string name()
		{
			return "PercentageStrike";
		}

		public override double value(double price)
		{
			switch (type_)
			{
				case Option.Type.Call:
					return price * Math.Max(1.0 - strike_, 0.0);

				case Option.Type.Put:
					return price * Math.Max(strike_ - 1.0, 0.0);

				default:
					throw new ArgumentException("unknown/illegal option type");
			}
		}
	}
}