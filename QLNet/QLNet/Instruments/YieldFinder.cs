using System.Collections.Generic;
using QLNet.Time;

namespace QLNet.Instruments
{
	public class YieldFinder : ISolver1d
	{
		private readonly double _faceAmount;
		private readonly List<CashFlow> _cashflows;
		private readonly double _dirtyPrice;
		private readonly Compounding _compounding;
		private readonly DayCounter _dayCounter;
		private readonly Frequency _frequency;
		private readonly Date _settlement;

		public YieldFinder(double faceAmount, List<CashFlow> cashflows, double dirtyPrice, DayCounter dayCounter, Compounding compounding, Frequency frequency, Date settlement)
		{
			_faceAmount = faceAmount;
			_cashflows = cashflows;
			_dirtyPrice = dirtyPrice;
			_compounding = compounding;
			_dayCounter = dayCounter;
			_frequency = frequency;
			_settlement = settlement;
		}

		public override double value(double yield)
		{
			return _dirtyPrice - Utils.dirtyPriceFromYield(_faceAmount, _cashflows, yield, _dayCounter, _compounding, _frequency, _settlement);
		}
	}
}