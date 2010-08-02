using System;

namespace QLNet
{
	/// <summary>
	/// Predetermined cash flow
	/// This cash flow pays a predetermined amount at a given date.
	/// </summary>
	public class FractionalDividend : Dividend
	{
		private readonly double _rate;
		private double? _nominal;

		public FractionalDividend(double rate, Date date)
			: base(date)
		{
			_rate = rate;
			_nominal = null;
		}

		public FractionalDividend(double rate, double nominal, Date date)
			: base(date)
		{
			_rate = rate;
			_nominal = nominal;
		}

		public double rate()
		{
			return _rate;
		}
		
		public double? nominal()
		{
			return _nominal;
		}

		public override double amount()
		{
			if (_nominal == null) 
				throw new ApplicationException("no nominal given");

			return _rate * _nominal.GetValueOrDefault();
		}

		public override double amount(double underlying)
		{
			return _rate * underlying;
		}
	}
}