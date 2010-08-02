using System;

namespace QLNet
{
	public abstract class SimpleQuoteInstrument : Instrument
	{
		private readonly Handle<Quote> _quote;

		protected SimpleQuoteInstrument(Handle<Quote> quote)
		{
			if (quote == null)
			{
				throw new ArgumentNullException("quote");
			}

			_quote = quote;
			_quote.registerWith(update);
		}

		public override bool isExpired()
		{
			return false;
		}

		protected override void performCalculations()
		{
			if (_quote.IsEmpty)
			{
				throw new ApplicationException("null quote set");
			}

			NPV_ = _quote.link.Value();
		}
	}
}