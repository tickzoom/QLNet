namespace QLNet
{
	/// <summary>
	/// 5-months EUR Libor index
	/// </summary>
	public class EURLibor5M : EURLibor
	{
		public EURLibor5M()
			: base(new Period(5, TimeUnit.Months), new Handle<YieldTermStructure>())
		{
		}

		public EURLibor5M(Handle<YieldTermStructure> h)
			: base(new Period(5, TimeUnit.Months), h)
		{
		}
	}
}