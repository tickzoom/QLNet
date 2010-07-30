using System;
using System.Collections.Generic;
using System.Linq;

namespace QLNet
{
	public class OvernightIndexedCouponPricer : FloatingRateCouponPricer
	{
		private OvernightIndexedCoupon coupon_;

		public override void initialize(FloatingRateCoupon coupon)
		{
			coupon_ = coupon as OvernightIndexedCoupon;
			if (coupon_ == null)
				throw new ApplicationException("wrong coupon type");
		}

		public override double swapletRate()
		{
			OvernightIndex index = coupon_.index() as OvernightIndex;

			List<Date> fixingDates = coupon_.fixingDates();
			List<double> dt = coupon_.dt();

			int n = dt.Count();
			int i = 0;

			double compoundFactor = 1.0;

			// already fixed part
			Date today = Settings.evaluationDate();
			while (fixingDates[i] < today && i < n)
			{
				// rate must have been fixed
				double pastFixing = IndexManager.instance().getHistory(
					index.name()).value()[fixingDates[i]];

				if (pastFixing == default(double))
					throw new ApplicationException("Missing " + index.name() + " fixing for "
												   + fixingDates[i].ToString());

				compoundFactor *= (1.0 + pastFixing * dt[i]);
				++i;
			}

			// today is a border case
			if (fixingDates[i] == today && i < n)
			{
				// might have been fixed
				try
				{
					double pastFixing = IndexManager.instance().getHistory(
						index.name()).value()[fixingDates[i]];

					if (pastFixing != default(double))
					{
						compoundFactor *= (1.0 + pastFixing * dt[i]);
						++i;
					}
					else
					{
						;   // fall through and forecast
					}
				}
				catch (Exception e)
				{
					;       // fall through and forecast
				}
			}

			// forward part using telescopic property in order
			// to avoid the evaluation of multiple forward fixings
			if (i < n)
			{
				Handle<YieldTermStructure> curve = index.forwardingTermStructure();
				if (curve.empty())
					throw new ArgumentException("null term structure set to this instance of" +
												index.name());

				List<Date> dates = coupon_.valueDates();
				double startDiscount = curve.link.discount(dates[i]);
				double endDiscount = curve.link.discount(dates[n]);

				compoundFactor *= startDiscount / endDiscount;
			}

			double rate = (compoundFactor - 1.0) / coupon_.accrualPeriod();
			return coupon_.gearing() * rate + coupon_.spread();
		}

		public override double swapletPrice()
		{ throw new ApplicationException("swapletPrice not available"); }
		public override double capletPrice(double d)
		{ throw new ApplicationException("capletPrice not available"); }
		public override double capletRate(double d)
		{ throw new ApplicationException("capletRate not available"); }
		public override double floorletPrice(double d)
		{ throw new ApplicationException("floorletPrice not available"); }
		public override double floorletRate(double d)
		{ throw new ApplicationException("floorletRate not available"); }
		protected override double optionletPrice(Option.Type t, double d)
		{ throw new ApplicationException("optionletPrice not available"); }

	}
}