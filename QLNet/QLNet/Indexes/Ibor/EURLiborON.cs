namespace QLNet
{
	public class EURLiborON : DailyTenorEURLibor
	{
		public EURLiborON()
			: base(0, new Handle<YieldTermStructure>())
		{
		}

		public EURLiborON(Handle<YieldTermStructure> h)
			: base(0, h)
		{
		}
	}
}