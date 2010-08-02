namespace QLNet
{
	public class LoanPricingEngineResults : Instrument.Results
	{
		public readonly InitializedList<double?> legNPV;

		public LoanPricingEngineResults()
		{
			legNPV = new InitializedList<double?>();	
		}

		public override void reset()
		{
			base.reset();

			// clear all previous results
			legNPV.Erase();
		}
	}
}