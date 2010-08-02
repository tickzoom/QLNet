namespace QLNet
{
	/// <summary>
	/// 3-months EUR Libor index
	/// </summary>
	public class EURLibor3M : EURLibor
	{
		public EURLibor3M()
			: base(new Period(3, TimeUnit.Months), new Handle<YieldTermStructure>())
		{
		}

		public EURLibor3M(Handle<YieldTermStructure> h)
			: base(new Period(3, TimeUnit.Months), h)
		{
		}
	}
}