namespace QLNet
{
	/// <summary>
	/// 1-month EUR Libor index
	/// </summary>
	public class EURLibor1M : EURLibor
	{
		public EURLibor1M()
			: base(new Period(1, TimeUnit.Months), new Handle<YieldTermStructure>())
		{
		}

		public EURLibor1M(Handle<YieldTermStructure> h)
			: base(new Period(1, TimeUnit.Months), h)
		{
		}
	}
}