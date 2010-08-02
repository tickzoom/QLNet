namespace QLNet
{
	/// <summary>
	/// Simple investment fund.
	/// </summary>
	public class InvestmentFund : SimpleQuoteInstrument
	{
		public InvestmentFund(Handle<Quote> quote) 
			: base(quote)
		{
		}
	}
}