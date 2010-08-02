namespace QLNet
{
	/// <summary>
	/// Rate helper with date schedule relative to the global evaluation date
	/// 
	/// This class takes care of rebuilding the date schedule when the global evaluation date changes
	/// </summary>
	public abstract class RelativeDateRateHelper : RateHelper
	{
		protected Date evaluationDate_;

		///////////////////////////////////////////
		// constructors
		public RelativeDateRateHelper(Handle<Quote> quote)
			: base(quote)
		{
			Settings.registerWith(update);
			evaluationDate_ = Settings.evaluationDate();
		}

		public RelativeDateRateHelper(double quote)
			: base(quote)
		{
			Settings.registerWith(update);
			evaluationDate_ = Settings.evaluationDate();
		}


		//////////////////////////////////////
		//! Observer interface
		public override void update()
		{
			if (evaluationDate_ != Settings.evaluationDate())
			{
				evaluationDate_ = Settings.evaluationDate();
				initializeDates();
			}
			base.update();
		}

		///////////////////////////////////////////
		protected abstract void initializeDates();
	}
}