namespace QLNet
{
	/// <summary>
	/// Intermediate class for payoffs based on a fixed strike
	/// </summary>
	public class StrikedTypePayoff : TypePayoff
	{
		protected double strike_;

		public StrikedTypePayoff(Option.Type type, double strike)
			: base(type)
		{
			strike_ = strike;
		}

		public override string description()
		{
			return base.description() + ", " + strike() + " strike";
		}

		public double strike()
		{
			return strike_;
		}
	}
}