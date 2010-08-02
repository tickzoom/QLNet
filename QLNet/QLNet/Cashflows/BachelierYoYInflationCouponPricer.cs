namespace QLNet
{
	/// <summary>
	/// Bachelier-formula pricer for capped/floored yoy inflation coupons
	/// </summary>
	public class BachelierYoYInflationCouponPricer : YoYInflationCouponPricer
	{
		public BachelierYoYInflationCouponPricer(Handle<YoYOptionletVolatilitySurface> capletVol = null)
			: base(capletVol)
		{
			if (capletVol == null)
				capletVol = new Handle<YoYOptionletVolatilitySurface>();
		}

		protected override double optionletPriceImp(Option.Type optionType, double effStrike,
		                                            double forward, double stdDev)
		{
			return Utils.bachelierBlackFormula(optionType, effStrike, forward, stdDev);
		}
	}
}