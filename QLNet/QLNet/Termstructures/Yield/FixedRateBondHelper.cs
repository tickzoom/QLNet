using System.Collections.Generic;
using QLNet.Time;

namespace QLNet
{
	public class FixedRateBondHelper : AbstractBondHelper<FixedRateBond>
	{
		public FixedRateBondHelper(Handle<Quote> cleanPrice, int settlementDays, double faceAmount, Schedule schedule, List<double> coupons, DayCounter dayCounter, BusinessDayConvention paymentConvention)
			: base(cleanPrice, new FixedRateBond(settlementDays, faceAmount, schedule, coupons, dayCounter, paymentConvention, 100.0, null))
		{
		}

		public FixedRateBondHelper(Handle<Quote> cleanPrice, int settlementDays, double faceAmount, Schedule schedule, List<double> coupons, DayCounter dayCounter, BusinessDayConvention paymentConvention, double redemption, Date issueDate)
			: base(cleanPrice, new FixedRateBond(settlementDays, faceAmount, schedule, coupons, dayCounter, paymentConvention, redemption, issueDate))
		{
		}
	}
}