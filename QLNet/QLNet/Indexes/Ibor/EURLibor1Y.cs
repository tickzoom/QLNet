namespace QLNet
{
	/// <summary>
	/// 1-year EUR Libor index
	/// </summary>
	public class EURLibor1Y : EURLibor
	{
		public EURLibor1Y()
			: base(new Period(1, TimeUnit.Years), new Handle<YieldTermStructure>())
		{
		}

		public EURLibor1Y(Handle<YieldTermStructure> h)
			: base(new Period(1, TimeUnit.Years), h)
		{
		}
	}
}