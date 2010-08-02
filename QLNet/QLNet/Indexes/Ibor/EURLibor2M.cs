namespace QLNet
{
	/// <summary>
	/// 2-months EUR Libor index
	/// </summary>
	public class EURLibor2M : EURLibor
	{
		public EURLibor2M()
			: base(new Period(2, TimeUnit.Months), new Handle<YieldTermStructure>())
		{
		}

		public EURLibor2M(Handle<YieldTermStructure> h)
			: base(new Period(2, TimeUnit.Months), h)
		{
		}
	}
}