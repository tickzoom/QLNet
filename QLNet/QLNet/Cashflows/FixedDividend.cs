namespace QLNet
{
	/// <summary>
	/// Predetermined cash flow
	/// This cash flow pays a predetermined amount at a given date.
	/// </summary>
	public class FixedDividend : Dividend
	{
		private readonly double _amount;
		
		public FixedDividend(double amount, Date date)
			: base(date)
		{
			_amount = amount;
		}

		public override double amount()
		{
			return _amount;
		}

		public override double amount(double d)
		{
			return _amount;
		}
	}
}