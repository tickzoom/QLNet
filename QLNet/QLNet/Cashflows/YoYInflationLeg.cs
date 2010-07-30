using System.Collections.Generic;

namespace QLNet.Cashflows
{
	/// <summary>
	/// Helper class building a sequence of capped/floored yoy inflation coupons.  Payoff is: spread + gearing x index.
	/// </summary>
	public class YoYInflationLeg : YoYInflationLegBase
	{
		public YoYInflationLeg(Schedule schedule, Calendar cal, YoYInflationIndex index, Period observationLag)
		{
			schedule_ = schedule;
			index_ = index;
			observationLag_ = observationLag;
			paymentAdjustment_ = BusinessDayConvention.ModifiedFollowing;
			paymentCalendar_ = cal;
		}

		public override List<CashFlow> value()
		{
			return CashFlowVectors.yoyInflationLeg(notionals_, schedule_, paymentAdjustment_, index_, gearings_, spreads_, paymentDayCounter_, caps_, floors_, paymentCalendar_, fixingDays_, observationLag_);
		}
	}
}