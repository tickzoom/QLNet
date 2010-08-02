using System.Collections.Generic;
using QLNet.Time.DayCounters;

namespace QLNet
{
	public class Cash : Loan
	{
		private Type type_;
		private double nominal_;
		private Schedule principalSchedule_;
		private BusinessDayConvention paymentConvention_;

		public Cash(Type type, double nominal, Schedule principalSchedule, BusinessDayConvention? paymentConvention)
			: base(1)
		{
			type_ = type;
			nominal_ = nominal;
			principalSchedule_ = principalSchedule;
			paymentConvention_ = paymentConvention.Value;

			List<CashFlow> principalLeg = new PricipalLeg(principalSchedule, new Actual365Fixed())
				.withNotionals(nominal)
				.withPaymentAdjustment(paymentConvention_)
				.withSign(type == Type.Loan ? -1 : 1);

			legs_[0] = principalLeg;
			if (type_ == Type.Loan)
			{
				payer_[0] = +1;
			}
			else
			{
				payer_[0] = -1;
			}
		}

		public List<CashFlow> principalLeg() { return legs_[0]; }
	}
}