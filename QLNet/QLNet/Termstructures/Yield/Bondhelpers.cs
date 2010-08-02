/*
 Copyright (C) 2008, 2009 Siarhei Novik (snovik@gmail.com)
  
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
using System;
using System.Linq;
using System.Text;

namespace QLNet
{
	//! fixed-coupon bond helper
	/*! \warning This class assumes that the reference date
				 does not change between calls of setTermStructure().
	*/
	public abstract class AbstractBondHelper<T> : RelativeDateRateHelper
		where T : Bond
	{
		private readonly T _bond;

		// need to init this because it is used before the handle has any link, i.e. setTermStructure will be used after ctor
		private readonly RelinkableHandle<YieldTermStructure> _termStructureHandle;


		/*! \warning Setting a pricing engine to the passed bond from
					 external code will cause the bootstrap to fail or
					 to give wrong results. It is advised to discard
					 the bond after creating the helper, so that the
					 helper has sole ownership of it.
		*/

		protected AbstractBondHelper(Handle<Quote> cleanPrice, T bond)
			: base(cleanPrice)
		{
			_bond = bond;

			latestDate_ = _bond.maturityDate();
			initializeDates();

			_termStructureHandle = new RelinkableHandle<YieldTermStructure>();
			IPricingEngine bondEngine = new DiscountingBondEngine(_termStructureHandle);
			_bond.setPricingEngine(bondEngine);
		}

		public T bond()
		{
			return _bond;
		}

		public override void setTermStructure(YieldTermStructure t)
		{
			// do not set the relinkable handle as an observer - force recalculation when needed
			_termStructureHandle.linkTo(t, false);
			base.setTermStructure(t);
		}

		public override double impliedQuote()
		{
			if (termStructure_ == null)
			{
				throw new ApplicationException("term structure not set");
			}

			// we didn't register as observers - force calculation
			_bond.recalculate();
			return _bond.cleanPrice();
		}

		protected override void initializeDates()
		{
			earliestDate_ = _bond.nextCouponDate();
		}
	}

	public class BondHelper : AbstractBondHelper<Bond>
	{
		public BondHelper(Handle<Quote> cleanPrice, Bond bond) 
			: base(cleanPrice, bond)
		{
		}
	}
}
