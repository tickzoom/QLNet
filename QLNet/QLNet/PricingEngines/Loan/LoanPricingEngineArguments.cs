using System;
using System.Collections.Generic;

namespace QLNet
{
	public class LoanPricingEngineArguments : IPricingEngineArguments
	{
		public readonly List<List<CashFlow>> legs;
		public readonly List<double> payer;

		public LoanPricingEngineArguments()
		{
			legs = new List<List<CashFlow>>();
			payer = new List<double>();
		}

		public virtual void validate()
		{
			if (legs.Count != payer.Count)
			{
				throw new ArgumentException("number of legs and multipliers differ");
			}
		}
	}
}