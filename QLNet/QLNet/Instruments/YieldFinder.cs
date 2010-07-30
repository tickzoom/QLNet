using System.Collections.Generic;
using QLNet.Time;

namespace QLNet.Instruments
{
	public class YieldFinder : ISolver1d
	{
		private readonly double faceAmount_;
		private readonly List<CashFlow> cashflows_;
		private readonly double dirtyPrice_;
		private readonly Compounding compounding_;
		private readonly DayCounter dayCounter_;
		private readonly Frequency frequency_;
		private readonly Date settlement_;

		public YieldFinder(double faceAmount, List<CashFlow> cashflows, double dirtyPrice, DayCounter dayCounter, Compounding compounding, Frequency frequency, Date settlement)
		{
			faceAmount_ = faceAmount;
			cashflows_ = cashflows;
			dirtyPrice_ = dirtyPrice;
			compounding_ = compounding;
			dayCounter_ = dayCounter;
			frequency_ = frequency;
			settlement_ = settlement;
		}

		public override double value(double yield)
		{
			return dirtyPrice_ - Utils.dirtyPriceFromYield(faceAmount_, cashflows_, yield, dayCounter_, compounding_, frequency_, settlement_);
		}
	}
}