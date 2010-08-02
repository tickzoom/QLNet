namespace QLNet
{
	/// <summary>
	/// Unit-Displaced-Black-formula pricer for capped/floored yoy inflation coupons
	/// </summary>
	public class UnitDisplacedBlackYoYInflationCouponPricer : YoYInflationCouponPricer
	{
		public UnitDisplacedBlackYoYInflationCouponPricer(Handle<YoYOptionletVolatilitySurface> capletVol = null)
			: base(capletVol)
		{
			if (capletVol == null)
				capletVol = new Handle<YoYOptionletVolatilitySurface>();

		}

		protected override double optionletPriceImp(Option.Type optionType, double effStrike,
		                                            double forward, double stdDev)
		{

			return Utils.blackFormula(optionType, effStrike + 1.0, forward + 1.0, stdDev);
		}
	}
}