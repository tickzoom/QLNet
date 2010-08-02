using System;

namespace QLNet
{
	/// <summary>
	/// Binary asset-or-nothing payoff
	/// </summary>
	public class AssetOrNothingPayoff : StrikedTypePayoff
	{
		public AssetOrNothingPayoff(Option.Type type, double strike) 
			: base(type, strike)
		{
		}

		public override string name()
		{
			return "AssetOrNothing";
		}

		public override double value(double price)
		{
			switch (type_)
			{
				case Option.Type.Call:
					return (price - strike_ > 0.0 ? price : 0.0);

				case Option.Type.Put:
					return (strike_ - price > 0.0 ? price : 0.0);

				default:
					throw new ArgumentException("unknown/illegal option type");
			}
		}
	}
}