using System;

namespace QLNet
{
	public class BlackVanillaOptionPricer : VanillaOptionPricer
	{
		private readonly double _forwardValue;
		private readonly Date _expiryDate;
		private readonly Period _swapTenor;
		private readonly SwaptionVolatilityStructure _volatilityStructure;
		private readonly SmileSection _smile;

		public BlackVanillaOptionPricer(double forwardValue, Date expiryDate, Period swapTenor, SwaptionVolatilityStructure volatilityStructure)
		{
			_forwardValue = forwardValue;
			_expiryDate = expiryDate;
			_swapTenor = swapTenor;
			_volatilityStructure = volatilityStructure;
			_smile = _volatilityStructure.smileSection(_expiryDate, _swapTenor);
		}

		public override double value(double strike, Option.Type optionType, double deflator)
		{
			double variance = _smile.variance(strike);
			return deflator * Utils.blackFormula(optionType, strike, _forwardValue, Math.Sqrt(variance));
		}
	}
}