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

	//! %EuriborSwapFix index base class
//    ! EuriborSwapFixB indexes fixed by ISDA at 12:00AM FRANKFURT.
//        Reuters page ISDAFIX2 or EURSFIXB=.
//
//        \warning The 1Y swap's floating leg is based on Euribor3M; the
//                 floating legs of longer swaps are based on Euribor6M
//    
	public class EuriborSwapFixB : SwapIndex
	{
        public EuriborSwapFixB(Period tenor)
            : base("EuriborSwapFixB", tenor, 2, new EURCurrency(), new TARGET(), new Period(1, TimeUnit.Years), BusinessDayConvention.ModifiedFollowing, new Thirty360(Thirty360.Thirty360Convention.BondBasis),
                tenor > new Period(1, TimeUnit.Years) ? 
                    new Euribor6M(new Handle<YieldTermStructure>()) as IborIndex : 
                        new Euribor3M(new Handle<YieldTermStructure>()) as IborIndex)
        {
        }
        public EuriborSwapFixB(Period tenor, Handle<YieldTermStructure> h)
            : base("EuriborSwapFixB", tenor, 2, new EURCurrency(), new TARGET(), new Period(1, TimeUnit.Years), BusinessDayConvention.ModifiedFollowing, new Thirty360(Thirty360.Thirty360Convention.BondBasis),
                tenor > new Period(1, TimeUnit.Years) ? 
                    new Euribor6M(h) as IborIndex : new Euribor3M(h) as IborIndex)
		{
		}
	}

	//! 1-year %EuriborSwapFixB index
	public class EuriborSwapFixB1Y : EuriborSwapFixB
	{
		public EuriborSwapFixB1Y(Handle<YieldTermStructure> h) : base(new Period(1, TimeUnit.Years), h)
		{
		}
	}

	//! 2-year %EuriborSwapFixB index
	public class EuriborSwapFixB2Y : EuriborSwapFixB
	{
		public EuriborSwapFixB2Y(Handle<YieldTermStructure> h) : base(new Period(2, TimeUnit.Years), h)
		{
		}
	}

	//! 3-year %EuriborSwapFixB index
	public class EuriborSwapFixB3Y : EuriborSwapFixB
	{
		public EuriborSwapFixB3Y(Handle<YieldTermStructure> h) : base(new Period(3, TimeUnit.Years), h)
		{
		}
	}

	//! 4-year %EuriborSwapFixB index
	public class EuriborSwapFixB4Y : EuriborSwapFixB
	{
		public EuriborSwapFixB4Y(Handle<YieldTermStructure> h) : base(new Period(4, TimeUnit.Years), h)
		{
		}
	}

	//! 5-year %EuriborSwapFixB index
	public class EuriborSwapFixB5Y : EuriborSwapFixB
	{
		public EuriborSwapFixB5Y(Handle<YieldTermStructure> h) : base(new Period(5, TimeUnit.Years), h)
		{
		}
	}

	//! 6-year %EuriborSwapFixB index
	public class EuriborSwapFixB6Y : EuriborSwapFixB
	{
		public EuriborSwapFixB6Y(Handle<YieldTermStructure> h) : base(new Period(6, TimeUnit.Years), h)
		{
		}
	}

	//! 7-year %EuriborSwapFixB index
	public class EuriborSwapFixB7Y : EuriborSwapFixB
	{
		public EuriborSwapFixB7Y(Handle<YieldTermStructure> h) : base(new Period(7, TimeUnit.Years), h)
		{
		}
	}

	//! 8-year %EuriborSwapFixB index
	public class EuriborSwapFixB8Y : EuriborSwapFixB
	{
		public EuriborSwapFixB8Y(Handle<YieldTermStructure> h) : base(new Period(8, TimeUnit.Years), h)
		{
		}
	}

	//! 9-year %EuriborSwapFixB index
	public class EuriborSwapFixB9Y : EuriborSwapFixB
	{
		public EuriborSwapFixB9Y(Handle<YieldTermStructure> h) : base(new Period(9, TimeUnit.Years), h)
		{
		}
	}

	//! 10-year %EuriborSwapFixB index
	public class EuriborSwapFixB10Y : EuriborSwapFixB
	{
		public EuriborSwapFixB10Y(Handle<YieldTermStructure> h) : base(new Period(10, TimeUnit.Years), h)
		{
		}
	}

	//! 12-year %EuriborSwapFixB index
	public class EuriborSwapFixB12Y : EuriborSwapFixB
	{
		public EuriborSwapFixB12Y(Handle<YieldTermStructure> h) : base(new Period(12, TimeUnit.Years), h)
		{
		}
	}

	//! 15-year %EuriborSwapFixB index
	public class EuriborSwapFixB15Y : EuriborSwapFixB
	{
		public EuriborSwapFixB15Y(Handle<YieldTermStructure> h) : base(new Period(15, TimeUnit.Years), h)
		{
		}
	}

	//! 20-year %EuriborSwapFixB index
	public class EuriborSwapFixB20Y : EuriborSwapFixB
	{
		public EuriborSwapFixB20Y(Handle<YieldTermStructure> h) : base(new Period(20, TimeUnit.Years), h)
		{
		}
	}

	//! 25-year %EuriborSwapFixB index
	public class EuriborSwapFixB25Y : EuriborSwapFixB
	{
		public EuriborSwapFixB25Y(Handle<YieldTermStructure> h) : base(new Period(25, TimeUnit.Years), h)
		{
		}
	}

	//! 30-year %EuriborSwapFixB index
	public class EuriborSwapFixB30Y : EuriborSwapFixB
	{
		public EuriborSwapFixB30Y(Handle<YieldTermStructure> h) : base(new Period(30, TimeUnit.Years), h)
		{
		}
	}

}
