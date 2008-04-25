/*
 Copyright (C) 2008 Toyin Akin (toyin_akin@hotmail.com)
  
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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QLNet {

	//! %EurliborSwapFixIFR index base class
//    ! EuriborSwapFix indexes published by IFR Markets and distributed
//        by Reuters page TGM42281 and by Telerate. For more info see
//        <http://www.ifrmarkets.com>.
//
//        \warning The 1Y swap's floating leg is based on Eurlibor3M; the
//                 floating legs of longer swaps are based on Eurlibor6M
//    
	public class EurliborSwapFixIFR : SwapIndex
	{
        public EurliborSwapFixIFR(Period tenor)
            : base("EurliborSwapFixIFR", tenor, 2, new EURCurrency(), new TARGET(), new Period(1, TimeUnit.Years), BusinessDayConvention.ModifiedFollowing, new Thirty360(Thirty360.Thirty360Convention.BondBasis),
                tenor > new Period(1, TimeUnit.Years) ? 
                    new Euribor6M(new Handle<YieldTermStructure>()) as IborIndex : 
                        new Euribor3M(new Handle<YieldTermStructure>()) as IborIndex)
        {
        }
        public EurliborSwapFixIFR(Period tenor, Handle<YieldTermStructure> h)
            : base("EurliborSwapFixIFR", tenor, 2, new EURCurrency(), new TARGET(), new Period(1, TimeUnit.Years), BusinessDayConvention.ModifiedFollowing, new Thirty360(Thirty360.Thirty360Convention.BondBasis),
                tenor > new Period(1, TimeUnit.Years) ? 
                    new Euribor6M(h) as IborIndex : new Euribor3M(h) as IborIndex)
		{
		}
	}

	//! 1-year %EurliborSwapFixIFR index
	public class EurliborSwapFixIFR1Y : EurliborSwapFixIFR
	{
		public EurliborSwapFixIFR1Y(Handle<YieldTermStructure> h) : base(new Period(1, TimeUnit.Years), h)
		{
		}
	}

	//! 2-year %EurliborSwapFixIFR index
	public class EurliborSwapFixIFR2Y : EurliborSwapFixIFR
	{
		public EurliborSwapFixIFR2Y(Handle<YieldTermStructure> h) : base(new Period(2, TimeUnit.Years), h)
		{
		}
	}

	//! 3-year %EurliborSwapFixIFR index
	public class EurliborSwapFixIFR3Y : EurliborSwapFixIFR
	{
		public EurliborSwapFixIFR3Y(Handle<YieldTermStructure> h) : base(new Period(3, TimeUnit.Years), h)
		{
		}
	}

	//! 4-year %EurliborSwapFixIFR index
	public class EurliborSwapFixIFR4Y : EurliborSwapFixIFR
	{
		public EurliborSwapFixIFR4Y(Handle<YieldTermStructure> h) : base(new Period(4, TimeUnit.Years), h)
		{
		}
	}

	//! 5-year %EurliborSwapFixIFR index
	public class EurliborSwapFixIFR5Y : EurliborSwapFixIFR
	{
		public EurliborSwapFixIFR5Y(Handle<YieldTermStructure> h) : base(new Period(5, TimeUnit.Years), h)
		{
		}
	}

	//! 6-year %EurliborSwapFixIFR index
	public class EurliborSwapFixIFR6Y : EurliborSwapFixIFR
	{
		public EurliborSwapFixIFR6Y(Handle<YieldTermStructure> h) : base(new Period(6, TimeUnit.Years), h)
		{
		}
	}

	//! 7-year %EurliborSwapFixIFR index
	public class EurliborSwapFixIFR7Y : EurliborSwapFixIFR
	{
		public EurliborSwapFixIFR7Y(Handle<YieldTermStructure> h) : base(new Period(7, TimeUnit.Years), h)
		{
		}
	}

	//! 8-year %EurliborSwapFixIFR index
	public class EurliborSwapFixIFR8Y : EurliborSwapFixIFR
	{
		public EurliborSwapFixIFR8Y(Handle<YieldTermStructure> h) : base(new Period(8, TimeUnit.Years), h)
		{
		}
	}

	//! 9-year %EurliborSwapFixIFR index
	public class EurliborSwapFixIFR9Y : EurliborSwapFixIFR
	{
		public EurliborSwapFixIFR9Y(Handle<YieldTermStructure> h) : base(new Period(9, TimeUnit.Years), h)
		{
		}
	}

	//! 10-year %EurliborSwapFixIFR index
	public class EurliborSwapFixIFR10Y : EurliborSwapFixIFR
	{
		public EurliborSwapFixIFR10Y(Handle<YieldTermStructure> h) : base(new Period(10, TimeUnit.Years), h)
		{
		}
	}

	//! 12-year %EurliborSwapFixIFR index
	public class EurliborSwapFixIFR12Y : EurliborSwapFixIFR
	{
		public EurliborSwapFixIFR12Y(Handle<YieldTermStructure> h) : base(new Period(12, TimeUnit.Years), h)
		{
		}
	}

	//! 15-year %EurliborSwapFixIFR index
	public class EurliborSwapFixIFR15Y : EurliborSwapFixIFR
	{
		public EurliborSwapFixIFR15Y(Handle<YieldTermStructure> h) : base(new Period(15, TimeUnit.Years), h)
		{
		}
	}

	//! 20-year %EurliborSwapFixIFR index
	public class EurliborSwapFixIFR20Y : EurliborSwapFixIFR
	{
		public EurliborSwapFixIFR20Y(Handle<YieldTermStructure> h) : base(new Period(20, TimeUnit.Years), h)
		{
		}
	}

	//! 25-year %EurliborSwapFixIFR index
	public class EurliborSwapFixIFR25Y : EurliborSwapFixIFR
	{
		public EurliborSwapFixIFR25Y(Handle<YieldTermStructure> h) : base(new Period(25, TimeUnit.Years), h)
		{
		}
	}

	//! 30-year %EurliborSwapFixIFR index
	public class EurliborSwapFixIFR30Y : EurliborSwapFixIFR
	{
		public EurliborSwapFixIFR30Y(Handle<YieldTermStructure> h) : base(new Period(30, TimeUnit.Years), h)
		{
		}
	}

}
