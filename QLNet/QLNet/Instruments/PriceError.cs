using System;

namespace QLNet
{
	public class PriceError : ISolver1d
	{
		private IPricingEngine engine_;
		private SimpleQuote vol_;
		private double targetValue_;
		private Instrument.Results results_;

		public PriceError(IPricingEngine engine, SimpleQuote vol, double targetValue)
		{
			engine_ = engine;
			vol_ = vol;
			targetValue_ = targetValue;

			results_ = engine_.getResults() as Instrument.Results;
			if (results_ == null)
				throw new ApplicationException("pricing engine does not supply needed results");
		}

		public override double value(double x)
		{
			vol_.setValue(x);
			engine_.calculate();
			return results_.value.GetValueOrDefault() - targetValue_;
		}
	}
}