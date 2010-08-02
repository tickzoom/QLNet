using QLNet.Currencies;

namespace QLNet
{
	/// <summary>
	/// base class for overnight indexed swap indexes
	/// </summary>
	public class OvernightIndexedSwapIndex : SwapIndex
	{
		public OvernightIndexedSwapIndex(
			string familyName,
			Period tenor,
			int settlementDays,
			Currency currency,
			OvernightIndex overnightIndex)
			: base(familyName, tenor, settlementDays,
			       currency, overnightIndex.fixingCalendar(),
			       new Period(1, TimeUnit.Years), BusinessDayConvention.ModifiedFollowing,
			       overnightIndex.dayCounter(), overnightIndex)
		{
			overnightIndex_ = overnightIndex;
		}
		//! \name Inspectors
		//@{
		public OvernightIndex overnightIndex() { return overnightIndex_; }
		/*! \warning Relinking the term structure underlying the index will
					 not have effect on the returned swap.
		*/
		OvernightIndexedSwap underlyingSwap(Date fixingDate)
		{
			double fixedRate = 0.0;
			return new MakeOIS(tenor_, overnightIndex_, fixedRate)
				.withEffectiveDate(valueDate(fixingDate))
				.withFixedLegDayCount(dayCounter_);
		}
		//@}
		protected OvernightIndex overnightIndex_;
	};
}