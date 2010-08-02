namespace QLNet
{
	/// <summary>
	/// Intermediate class for put/call payoffs
	/// </summary>
	public class TypePayoff : Payoff
	{
		protected Option.Type type_;

		public Option.Type optionType()
		{
			return type_;
		}

		public TypePayoff(Option.Type type)
		{
			type_ = type;
		}

		/// <summary>
		/// Payoff interface
		/// </summary>
		/// <returns></returns>
		public override string description()
		{
			return name() + " " + optionType();
		}
	}
}