using System;

namespace QLNet
{
	/// <summary>
	/// Payoff based on a floating strike
	/// </summary>
	public class FloatingTypePayoff : TypePayoff
	{
		public FloatingTypePayoff(Option.Type type) 
			: base(type)
		{
		}

		public override string name()
		{
			return "FloatingType";
		}

		public override double value(double k)
		{
			throw new NotSupportedException("floating payoff not handled");
		}
	}
}