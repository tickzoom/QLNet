/*
 Copyright (C) 2008, 2009 , 2010  Andrea Maggiulli (a.maggiulli@gmail.com)
 * 
 This file is part of QLNet Project http://www.qlnet.org

 QLNet is free software: you can redistribute it and/or modify it
 under the terms of the QLNet license.  You should have received a
 copy of the license along with this program; if not, license is  
 available online at <http://trac2.assembla.com/QLNet/wiki/License>.
  
 QLNet is a based on QuantLib, a free-software/open-source library
 for financial quantitative analysts and developers - http://quantlib.org/
 The QuantLib license is available online at http://quantlib.org/license.shtml.
 
 This program is distributed in the hope that it will be useful, but WITHOUT
 ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 FOR A PARTICULAR PURPOSE.  See the license for more details.
*/

namespace QLNet
{
	/// <summary>
	/// Cash flow dependent on an index ratio.
	/// 
	/// This cash flow is not a coupon, i.e., there's no accrual.  The
	/// amount is either i(T)/i(0) or i(T)/i(0) - 1, depending on the
	/// growthOnly parameter.
	/// 
	/// We expect this to be used inside an instrument that does all the date
	/// adjustment etc., so this takes just dates and does not change them.
	/// growthOnly = false means i(T)/i(0), which is a bond-type setting.
	/// growthOnly = true means i(T)/i(0) - 1, which is a swap-type setting.
	/// </summary>
	public class IndexedCashFlow : CashFlow
	{
		private readonly double _notional;
		private readonly Index _index;
		private readonly Date _baseDate;
		private readonly Date _fixingDate;
		private readonly Date _paymentDate;
		private readonly bool _growthOnly;

		public IndexedCashFlow(double notional, Index index, Date baseDate, Date fixingDate, Date paymentDate)
			: this(notional, index, baseDate, fixingDate, paymentDate, false)
		{
		}

		public IndexedCashFlow(double notional, Index index, Date baseDate, Date fixingDate, Date paymentDate, bool growthOnly)
		{
			_notional = notional;
			_index = index;
			_baseDate = baseDate;
			_fixingDate = fixingDate;
			_paymentDate = paymentDate;
			_growthOnly = growthOnly;
		}

		public override Date Date
		{
			get { return _paymentDate; }
		}

		public virtual double notional()
		{
			return _notional;
		}

		public virtual Date baseDate()
		{
			return _baseDate;
		}

		public virtual Date fixingDate()
		{
			return _fixingDate;
		}

		public virtual Index index()
		{
			return _index;
		}

		public virtual bool growthOnly()
		{
			return _growthOnly;
		}

		public override double amount()
		{
			double I0 = _index.fixing(_baseDate);
			double I1 = _index.fixing(_fixingDate);

			return _growthOnly ? _notional * (I1 / I0 - 1.0) : _notional * (I1 / I0);
		}
	}
}
