using System;

namespace QLNet
{
	/// <summary>
	/// Class for forward type payoffs
	/// </summary>
	public class ForwardTypePayoff : Payoff {
		protected Position.Type type_;
		public Position.Type forwardType() { return type_; }

		protected double strike_;
		public double strike() { return strike_; }

		public ForwardTypePayoff(Position.Type type, double strike) {
			type_ = type;
			strike_ = strike;
			if (strike < 0.0)
				throw new ApplicationException("negative strike given");
		}

		//! \name Payoff interface
		public override string name() { return "Forward";}
		public override string description()  {
			string result = name() + ", " + strike() + " strike";
			return result;
		}
		public override double value(double price)  {
			switch (type_) {
				case Position.Type.Long:
					return (price-strike_);
				case Position.Type.Short:
					return (strike_-price);
				default:
					throw new ApplicationException("unknown/illegal position type");
			}
		}
	}
}