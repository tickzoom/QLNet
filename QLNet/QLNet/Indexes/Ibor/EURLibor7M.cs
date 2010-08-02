namespace QLNet
{
	/// <summary>
	/// 7-months EUR Libor index
	/// </summary>
	public class EURLibor7M : EURLibor
	{
		public EURLibor7M()
			: base(new Period(7, TimeUnit.Months), new Handle<YieldTermStructure>())
		{
		}

		public EURLibor7M(Handle<YieldTermStructure> h)
			: base(new Period(7, TimeUnit.Months), h)
		{
		}
	}
}