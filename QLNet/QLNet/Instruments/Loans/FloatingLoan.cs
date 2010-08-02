using System.Collections.Generic;
using QLNet.Time;

namespace QLNet
{
	public class FloatingLoan : Loan
	{
		private Type type_;
		private double nominal_;
		private Schedule floatingSchedule_;
		private double floatingSpread_;
		private DayCounter floatingDayCount_;
		private Schedule principalSchedule_;
		private BusinessDayConvention paymentConvention_;
		private IborIndex iborIndex_;

		public FloatingLoan(Type type, double nominal,
		                    Schedule floatingSchedule, double floatingSpread, DayCounter floatingDayCount,
		                    Schedule principalSchedule, BusinessDayConvention? paymentConvention, IborIndex index) :
		                    	base(2)
		{
			type_ = type;
			nominal_ = nominal;
			floatingSchedule_ = floatingSchedule;
			floatingSpread_ = floatingSpread;
			floatingDayCount_ = floatingDayCount;
			principalSchedule_ = principalSchedule;
			iborIndex_ = index;

			if (paymentConvention.HasValue)
				paymentConvention_ = paymentConvention.Value;
			else
				paymentConvention_ = floatingSchedule_.businessDayConvention();

			List<CashFlow> principalLeg = new PricipalLeg(principalSchedule, floatingDayCount)
				.withNotionals(nominal)
				.withPaymentAdjustment(paymentConvention_)
				.withSign(type == Type.Loan ? -1 : 1);

			// temporary 
			for (int i = 0; i < principalLeg.Count - 1; i++)
			{
				Principal p = (Principal)principalLeg[i];
				notionals_.Add(p.nominal());
			}

			List<CashFlow> floatingLeg = new IborLeg(floatingSchedule, iborIndex_)
				.withPaymentDayCounter(floatingDayCount_)
				.withSpreads(floatingSpread_)
				.withPaymentAdjustment(paymentConvention_)
				.withNotionals(notionals_);


			legs_[0] = floatingLeg;
			legs_[1] = principalLeg;
			if (type_ == Type.Loan)
			{
				payer_[0] = -1;
				payer_[1] = +1;
			}
			else
			{
				payer_[0] = +1;
				payer_[1] = -1;
			}
		}

		public List<CashFlow> floatingLeg() { return legs_[0]; }
		public List<CashFlow> principalLeg() { return legs_[1]; }
	}
}