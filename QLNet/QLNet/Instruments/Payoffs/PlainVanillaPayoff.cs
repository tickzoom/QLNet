using System;

namespace QLNet
{
	/// <summary>
	/// Plain-vanilla payoff
	/// </summary>
	public class PlainVanillaPayoff : StrikedTypePayoff
	{
		public PlainVanillaPayoff(Option.Type type, double strike) 
			: base(type, strike)
		{
		}

		public override string name()
		{
			return "Vanilla";
		}

		public override double value(double price)
		{
			switch (type_)
			{
				case Option.Type.Call:
					return Math.Max(price - strike_, 0.0);
				case Option.Type.Put:
					return Math.Max(strike_ - price, 0.0);
				default:
					throw new ArgumentException("unknown/illegal option type");
			}
		}
	}
}