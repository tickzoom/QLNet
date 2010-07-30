using QLNet.Time;

namespace QLNet
{
	public class CappedFlooredIborCoupon : CappedFlooredCoupon
	{
		// need by CashFlowVectors
		public CappedFlooredIborCoupon()
		{
		}

		public CappedFlooredIborCoupon(double nominal, Date paymentDate, Date startDate, Date endDate, int fixingDays, IborIndex index)
			: this(nominal, paymentDate, startDate, endDate, fixingDays, index, 1.0, 0.0, null, null)
		{
		}

		public CappedFlooredIborCoupon(double nominal, Date paymentDate, Date startDate, Date endDate, int fixingDays, IborIndex index, double gearing, double spread, double? cap, double? floor)
			: this(nominal, paymentDate, startDate, endDate, fixingDays, index, gearing, spread, cap, floor, null, null, null, false)
		{
		}

		public CappedFlooredIborCoupon(double nominal, Date paymentDate, Date startDate, Date endDate, int fixingDays, IborIndex index, double gearing, double spread, double? cap, double? floor, Date refPeriodStart, Date refPeriodEnd, DayCounter dayCounter, bool isInArrears)
			: base(new IborCoupon(nominal, paymentDate, startDate, endDate, fixingDays, index, gearing, spread, refPeriodStart, refPeriodEnd, dayCounter, isInArrears), cap, floor)
		{
		}
	}
}