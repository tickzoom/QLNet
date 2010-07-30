using QLNet.Time;

namespace QLNet
{
	public class CappedFlooredCmsCoupon : CappedFlooredCoupon
	{
		public CappedFlooredCmsCoupon()
		{
		}

		public CappedFlooredCmsCoupon(double nominal, Date paymentDate, Date startDate, Date endDate, int fixingDays, SwapIndex index)
			: this(nominal, paymentDate, startDate, endDate, fixingDays, index, 1.0, 0.0, null, null, null, null, null, false)
		{
		}

		public CappedFlooredCmsCoupon(double nominal, Date paymentDate, Date startDate, Date endDate, int fixingDays, SwapIndex index, double gearing, double spread, double? cap, double? floor, Date refPeriodStart, Date refPeriodEnd, DayCounter dayCounter, bool isInArrears)
			: base(new CmsCoupon(nominal, paymentDate, startDate, endDate, fixingDays, index, gearing, spread, refPeriodStart, refPeriodEnd, dayCounter, isInArrears), cap, floor)
		{
		}
	}
}