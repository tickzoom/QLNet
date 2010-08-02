using System;
using System.Collections.Generic;
using System.Linq;

namespace QLNet
{
	public class AverageBMACouponPricer : FloatingRateCouponPricer {
		private AverageBMACoupon coupon_;

		public override void initialize(FloatingRateCoupon coupon) {
			coupon_ = coupon as AverageBMACoupon;
			if (coupon_ == null)
				throw new ApplicationException("wrong coupon type");
		}

		public override double swapletRate() {
			List<Date> fixingDates = coupon_.fixingDates();
			InterestRateIndex index = coupon_.index();

			int cutoffDays = 0; // to be verified
			Date startDate = coupon_.accrualStartDate() - cutoffDays,
			     endDate = coupon_.accrualEndDate() - cutoffDays,
			     d1 = startDate,
			     d2 = startDate;

			if (!(fixingDates.Count > 0)) throw new ApplicationException("fixing date list empty");
			if (!(index.valueDate(fixingDates.First()) <= startDate))
				throw new ApplicationException("first fixing date valid after period start");
			if (!(index.valueDate(fixingDates.Last()) >= endDate))
				throw new ApplicationException("last fixing date valid before period end");

			double avgBMA = 0.0;
			int days = 0;
			for (int i=0; i<fixingDates.Count - 1; ++i) {
				Date valueDate = index.valueDate(fixingDates[i]);
				Date nextValueDate = index.valueDate(fixingDates[i+1]);

				if (fixingDates[i] >= endDate || valueDate >= endDate)
					break;
				if (fixingDates[i+1] < startDate || nextValueDate <= startDate)
					continue;

				d2 = Date.Min(nextValueDate, endDate);

				avgBMA += index.fixing(fixingDates[i]) * (d2 - d1);

				days += d2 - d1;
				d1 = d2;
			}
			avgBMA /= (endDate - startDate);

			if (!(days == endDate - startDate))
				throw new ApplicationException("averaging days " + days + " differ from " +
				                               "interest days " + (endDate - startDate));

			return coupon_.gearing()*avgBMA + coupon_.spread();
		}

		public override double swapletPrice() {
			throw new ApplicationException("not available");
		}
		public override double capletPrice(double d) {
			throw new ApplicationException("not available");
		}
		public override double capletRate(double d) {
			throw new ApplicationException("not available");
		}
		public override double floorletPrice(double d) {
			throw new ApplicationException("not available");
		}
		public override double floorletRate(double d) {
			throw new ApplicationException("not available");
		}

		// recheck
		protected override double optionletPrice(Option.Type t, double d) {
			throw new ApplicationException("not available");
		}
	}
}