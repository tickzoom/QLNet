namespace QLNet
{
	/// <summary>
	/// Overnight GBP Libor index.
	/// </summary>
	public class GBPLiborON : DailyTenorGBPLibor
	{
		public GBPLiborON(Handle<YieldTermStructure> h) 
			: base(0, h)
		{
		}
	}
}