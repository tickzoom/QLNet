namespace QLNet
{
	/// <summary>
	/// 9-months EUR Libor index
	/// </summary>
	public class EURLibor9M : EURLibor
	{
		public EURLibor9M()
			: base(new Period(9, TimeUnit.Months), new Handle<YieldTermStructure>())
		{
		}

		public EURLibor9M(Handle<YieldTermStructure> h)
			: base(new Period(9, TimeUnit.Months), h)
		{
		}
	}
}