namespace QLNet
{
	/// <summary>
	/// 11-months EUR Libor index
	/// </summary>
	public class EURLibor11M : EURLibor
	{
		public EURLibor11M()
			: base(new Period(11, TimeUnit.Months), new Handle<YieldTermStructure>())
		{
		}

		public EURLibor11M(Handle<YieldTermStructure> h)
			: base(new Period(11, TimeUnit.Months), h)
		{
		}
	}
}