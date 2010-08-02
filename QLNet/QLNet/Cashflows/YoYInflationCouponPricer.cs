using System;

namespace QLNet
{
	/// <summary>
	/// base pricer for capped/floored YoY inflation coupons
	/// </summary>
	/// <remarks>
	/// This pricer can already do swaplets but to get 
	/// volatility-dependent coupons you need the descendents.
	/// </remarks>
	public class YoYInflationCouponPricer : InflationCouponPricer
	{
		public YoYInflationCouponPricer(Handle<YoYOptionletVolatilitySurface> capletVol)
		{
			if (capletVol == null)
				capletVol = new Handle<YoYOptionletVolatilitySurface>();

			capletVol_ = capletVol;

			if (!capletVol.empty()) capletVol_.registerWith(update);
		}

		public virtual Handle<YoYOptionletVolatilitySurface> capletVolatility()
		{
			return capletVol_;
		}

		public virtual void setCapletVolatility(Handle<YoYOptionletVolatilitySurface> capletVol)
		{
			if (capletVol.empty())
				throw new ApplicationException("empty capletVol handle");

			capletVol_ = capletVol;
			capletVol_.registerWith(update);
		}

		//! \name InflationCouponPricer interface
		//@{
		public override double swapletPrice()
		{
			double swapletPrice = adjustedFixing() * coupon_.accrualPeriod() * discount_;
			return gearing_ * swapletPrice + spreadLegValue_;
		}

		public override double swapletRate()
		{
			// This way we do not require the index to have
			// a yield curve, i.e. we do not get the problem
			// that a discounting-instrument-pricer is used
			// with a different yield curve
			return gearing_ * adjustedFixing() + spread_;
		}

		public override double capletPrice(double effectiveCap)
		{
			double capletPrice = optionletPrice(Option.Type.Call, effectiveCap);
			return gearing_ * capletPrice;
		}

		public override double capletRate(double effectiveCap)
		{
			return capletPrice(effectiveCap) / (coupon_.accrualPeriod() * discount_);
		}

		public override double floorletPrice(double effectiveFloor)
		{
			double floorletPrice = optionletPrice(Option.Type.Put, effectiveFloor);
			return gearing_ * floorletPrice;
		}

		public override double floorletRate(double effectiveFloor)
		{
			return floorletPrice(effectiveFloor) /
			       (coupon_.accrualPeriod() * discount_);
		}

		public override void initialize(InflationCoupon coupon)
		{
			coupon_ = coupon as YoYInflationCoupon;
			gearing_ = coupon_.gearing();
			spread_ = coupon_.spread();
			PaymentDate = coupon_.Date;
			YoYInflationIndex y = (YoYInflationIndex)(coupon.index());
			RateCurve = y.yoyInflationTermStructure().link.nominalTermStructure();

			// past or future fixing is managed in YoYInflationIndex::fixing()
			// use yield curve from index (which sets discount)

			discount_ = 1.0;
			if (PaymentDate > RateCurve.link.referenceDate())
				discount_ = RateCurve.link.discount(PaymentDate);

			spreadLegValue_ = spread_ * coupon_.accrualPeriod() * discount_;
		}
		//@}

		//! car replace this if really required
		protected virtual double optionletPrice(Option.Type optionType, double effStrike)
		{

			Date fixingDate = coupon_.fixingDate();
			if (fixingDate <= Settings.evaluationDate())
			{
				// the amount is determined
				double a, b;
				if (optionType == Option.Type.Call)
				{
					a = coupon_.indexFixing();
					b = effStrike;
				}
				else
				{
					a = effStrike;
					b = coupon_.indexFixing();
				}
				return Math.Max(a - b, 0.0) * coupon_.accrualPeriod() * discount_;

			}
			else
			{
				// not yet determined, use Black/DD1/Bachelier/whatever from Impl
				if (capletVolatility().empty())
					throw new ApplicationException("missing optionlet volatility");

				double stdDev =
					Math.Sqrt(capletVolatility().link.totalVariance(fixingDate, effStrike));

				double fixing = optionletPriceImp(optionType,
				                                  effStrike,
				                                  adjustedFixing(),
				                                  stdDev);
				return fixing * coupon_.accrualPeriod() * discount_;

			}
		}

		//! usually only need implement this (of course they may need
		//! to re-implement initialize too ...)
		protected virtual double optionletPriceImp(Option.Type t, double strike,
		                                           double forward, double stdDev)
		{
			throw new ApplicationException("you must implement this to get a vol-dependent price");
		}

		protected virtual double adjustedFixing()
		{
			return adjustedFixing(null);
		}

		protected virtual double adjustedFixing(double? fixing)
		{
			if (fixing == null)
				fixing = coupon_.indexFixing();

			// no adjustment
			return fixing.Value;
		}

		//! data
		Handle<YoYOptionletVolatilitySurface> capletVol_;
		YoYInflationCoupon coupon_;
		double gearing_;
		double spread_;
		double discount_;
		double spreadLegValue_;
	}
}