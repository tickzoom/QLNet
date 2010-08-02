namespace QLNet
{
	/// <summary>
	/// Black-formula pricer for capped/floored yoy inflation coupons
	/// </summary>
	public class BlackYoYInflationCouponPricer : YoYInflationCouponPricer
	{
		public BlackYoYInflationCouponPricer(Handle<YoYOptionletVolatilitySurface> capletVol)
			: base(capletVol)
		{
			if (capletVol == null)
				capletVol = new Handle<YoYOptionletVolatilitySurface>();
		}

		protected override double optionletPriceImp(Option.Type optionType, double effStrike, double forward, double stdDev)
		{
			return Utils.blackFormula(optionType, effStrike, forward, stdDev);
		}
	}
}