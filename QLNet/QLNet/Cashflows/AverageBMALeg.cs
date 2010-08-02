using System;
using System.Collections.Generic;
using System.Linq;
using QLNet.Time;

namespace QLNet
{
	/// <summary>
	/// Helper class building a sequence of average BMA coupons
	/// </summary>
	public class AverageBMALeg : Cashflows.RateLegBase
	{
		private BMAIndex index_;
		private List<double> gearings_;
		private List<double> spreads_;

		public AverageBMALeg(Schedule schedule, BMAIndex index)
		{
			schedule_ = schedule;
			index_ = index;
			paymentAdjustment_ = BusinessDayConvention.Following;
		}

		public AverageBMALeg withPaymentDayCounter(DayCounter dayCounter)
		{
			paymentDayCounter_ = dayCounter;
			return this;
		}

		public AverageBMALeg withGearings(double gearing)
		{
			gearings_ = new List<double>() { gearing };
			return this;
		}

		public AverageBMALeg withGearings(List<double> gearings)
		{
			gearings_ = gearings;
			return this;
		}

		public AverageBMALeg withSpreads(double spread)
		{
			spreads_ = new List<double>() { spread };
			return this;
		}

		public AverageBMALeg withSpreads(List<double> spreads)
		{
			spreads_ = spreads;
			return this;
		}

		public override List<CashFlow> value()
		{
			if (notionals_.Count == 0)
				throw new ApplicationException("no notional given");

			List<CashFlow> cashflows = new List<CashFlow>();

			// the following is not always correct
			Calendar calendar = schedule_.calendar();

			Date refStart, start, refEnd, end;
			Date paymentDate;

			int n = schedule_.Count - 1;
			for (int i = 0; i < n; ++i)
			{
				refStart = start = schedule_.date(i);
				refEnd = end = schedule_.date(i + 1);
				paymentDate = calendar.adjust(end, paymentAdjustment_);
				if (i == 0 && !schedule_.isRegular(i + 1))
					refStart = calendar.adjust(end - schedule_.tenor(), paymentAdjustment_);
				if (i == n - 1 && !schedule_.isRegular(i + 1))
					refEnd = calendar.adjust(start + schedule_.tenor(), paymentAdjustment_);

				cashflows.Add(new AverageBMACoupon(Utils.Get<double>(notionals_, i, notionals_.Last()),
				                                   paymentDate, start, end,
				                                   index_,
				                                   Utils.Get<double>(gearings_, i, 1.0),
				                                   Utils.Get<double>(spreads_, i, 0.0),
				                                   refStart, refEnd,
				                                   paymentDayCounter_));
			}

			return cashflows;
		}
	}
}