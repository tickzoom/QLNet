using System.Collections.Generic;
using System.Linq;
using QLNet.Time;

namespace QLNet
{
	public class CommercialPaper : Loan
	{
		private readonly Loan.Type type_;
		private double nominal_;
		private readonly Schedule fixedSchedule_;
		private double fixedRate_;
		private DayCounter fixedDayCount_;
		private Schedule principalSchedule_;
		private readonly BusinessDayConvention paymentConvention_;

		public CommercialPaper(Loan.Type type, double nominal, Schedule fixedSchedule, double fixedRate, DayCounter fixedDayCount, Schedule principalSchedule, BusinessDayConvention? paymentConvention) 
			: base(2)
		{
			type_ = type;
			nominal_ = nominal;
			fixedSchedule_ = fixedSchedule;
			fixedRate_ = fixedRate;
			fixedDayCount_ = fixedDayCount;
			principalSchedule_ = principalSchedule;

			if (paymentConvention.HasValue)
				paymentConvention_ = paymentConvention.Value;
			else
				paymentConvention_ = fixedSchedule_.businessDayConvention();

			List<CashFlow> principalLeg = new PricipalLeg(principalSchedule, fixedDayCount)
				.withNotionals(nominal)
				.withPaymentAdjustment(paymentConvention_)
				.withSign(type == Type.Loan ? -1 : 1);

			// temporary 
			for (int i = 0; i < principalLeg.Count - 1; i++)
			{
				Principal p = (Principal)principalLeg[i];
				notionals_.Add(p.nominal());
			}

			List<CashFlow> fixedLeg = new FixedRateLeg(fixedSchedule)
				.withCouponRates(fixedRate, fixedDayCount)
				.withPaymentAdjustment(paymentConvention_)
				.withNotionals(notionals_);

			// Discounting Pricipal
			notionals_.Clear();
			double n;
			for (int i = 0; i < fixedLeg.Count; i++)
			{
				FixedRateCoupon c = (FixedRateCoupon)fixedLeg[i];
				n = i > 0 ? notionals_.Last() : c.nominal();
				notionals_.Add(n / (1 + (c.rate() * c.dayCounter().yearFraction(c.refPeriodStart, c.refPeriodEnd))));
			}

			// New Leg
			List<CashFlow> discountedFixedLeg = new FixedRateLeg(fixedSchedule)
				.withCouponRates(fixedRate, fixedDayCount)
				.withPaymentAdjustment(paymentConvention_)
				.withNotionals(notionals_);
			// Adjust Principal
			Principal p0 = (Principal)principalLeg[0];
			p0.setAmount(notionals_.Last());

			legs_[0] = discountedFixedLeg;
			legs_[1] = principalLeg;
			if (type_ == Type.Loan)
			{
				payer_[0] = +1;
				payer_[1] = -1;
			}
			else
			{
				payer_[0] = -1;
				payer_[1] = +1;
			}
		}

		public List<CashFlow> fixedLeg() { return legs_[0]; }
		public List<CashFlow> principalLeg() { return legs_[1]; }
	}
}