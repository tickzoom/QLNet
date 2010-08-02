namespace QLNet
{
	/// <summary>
	/// 10-months EUR Libor index
	/// </summary>
	public class EURLibor10M : EURLibor
	{
		public EURLibor10M()
			: base(new Period(10, TimeUnit.Months), new Handle<YieldTermStructure>())
		{
		}

		public EURLibor10M(Handle<YieldTermStructure> h)
			: base(new Period(10, TimeUnit.Months), h)
		{
		}
	}
}