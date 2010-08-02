namespace QLNet
{
	/// <summary>
	/// 4-months EUR Libor index
	/// </summary>
	public class EURLibor4M : EURLibor
	{
		public EURLibor4M()
			: base(new Period(4, TimeUnit.Months), new Handle<YieldTermStructure>())
		{
		}

		public EURLibor4M(Handle<YieldTermStructure> h)
			: base(new Period(4, TimeUnit.Months), h)
		{
		}
	}
}