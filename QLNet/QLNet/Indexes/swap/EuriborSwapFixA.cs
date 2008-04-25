/*
 Copyright (C) 2008 Toyin Akin (toyin_akin@hotmail.com)
 Copyright (C) 2008 Andrea Maggiulli
  
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

   /// <summary>
   /// %EuriborSwapFixA index base class
   /// EuriborSwapFixA indexes fixed by ISDA at 11:00AM FRANKFURT.
   /// Reuters page ISDAFIX2 or EURSFIXA=.
   /// \warning The 1Y swap's floating leg is based on Euribor3M; the
   /// floating legs of longer swaps are based on Euribor6M
   /// </summary>
	public class EuriborSwapFixA : SwapIndex
	{
        public EuriborSwapFixA(Period tenor)
            : base("EuriborSwapFixA", tenor, 2, new EURCurrency(), new TARGET(), new Period(1, TimeUnit.Years), BusinessDayConvention.ModifiedFollowing, new Thirty360(Thirty360.Thirty360Convention.BondBasis),
                tenor > new Period(1, TimeUnit.Years) ? 
                    new Euribor6M(new Handle<YieldTermStructure>()) as IborIndex : 
                        new Euribor3M(new Handle<YieldTermStructure>()) as IborIndex)
        {
        }
        public EuriborSwapFixA(Period tenor, Handle<YieldTermStructure> h)
            : base("EuriborSwapFixA", tenor, 2, new EURCurrency(), new TARGET(), new Period(1, TimeUnit.Years), BusinessDayConvention.ModifiedFollowing, new Thirty360(Thirty360.Thirty360Convention.BondBasis),
                tenor > new Period(1, TimeUnit.Years) ? 
                    new Euribor6M(h) as IborIndex : new Euribor3M(h) as IborIndex)
		{
		}
	}

   /// <summary>
   /// 1-year %EuriborSwapFixA index
   /// </summary>
	public class EuriborSwapFixA1Y : EuriborSwapFixA
	{
		public EuriborSwapFixA1Y(Handle<YieldTermStructure> h) : base(new Period(1,TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 2-year %EuriborSwapFixA index
   /// </summary>
	public class EuriborSwapFixA2Y : EuriborSwapFixA
	{
		public EuriborSwapFixA2Y(Handle<YieldTermStructure> h) : base(new Period(2 , TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 3-year %EuriborSwapFixA index
   /// </summary>
	public class EuriborSwapFixA3Y : EuriborSwapFixA
	{
		public EuriborSwapFixA3Y(Handle<YieldTermStructure> h) : base(new Period(3 , TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 4-year %EuriborSwapFixA index
   /// </summary>
	public class EuriborSwapFixA4Y : EuriborSwapFixA
	{
		public EuriborSwapFixA4Y(Handle<YieldTermStructure> h) : base(new Period(4 , TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 5-year %EuriborSwapFixA index
   /// </summary>
	public class EuriborSwapFixA5Y : EuriborSwapFixA
	{
		public EuriborSwapFixA5Y(Handle<YieldTermStructure> h) : base(new Period(5 , TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 6-year %EuriborSwapFixA index
   /// </summary>
	public class EuriborSwapFixA6Y : EuriborSwapFixA
	{
		public EuriborSwapFixA6Y(Handle<YieldTermStructure> h) : base(new Period(6 , TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 7-year %EuriborSwapFixA index
   /// </summary>
	public class EuriborSwapFixA7Y : EuriborSwapFixA
	{
		public EuriborSwapFixA7Y(Handle<YieldTermStructure> h) : base(new Period(7 , TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 8-year %EuriborSwapFixA index
   /// </summary>
	public class EuriborSwapFixA8Y : EuriborSwapFixA
	{
		public EuriborSwapFixA8Y(Handle<YieldTermStructure> h) : base(new Period(8 , TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 9-year %EuriborSwapFixA index
   /// </summary>
	public class EuriborSwapFixA9Y : EuriborSwapFixA
	{
		public EuriborSwapFixA9Y(Handle<YieldTermStructure> h) : base(new Period(9 , TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 10-year %EuriborSwapFixA index
   /// </summary>
	public class EuriborSwapFixA10Y : EuriborSwapFixA
	{
		public EuriborSwapFixA10Y(Handle<YieldTermStructure> h) : base(new Period(10 , TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 12-year %EuriborSwapFixA index
   /// </summary>
	public class EuriborSwapFixA12Y : EuriborSwapFixA
	{
		public EuriborSwapFixA12Y(Handle<YieldTermStructure> h) : base(new Period(12 , TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 15-year %EuriborSwapFixA index
   /// </summary>
	public class EuriborSwapFixA15Y : EuriborSwapFixA
	{
		public EuriborSwapFixA15Y(Handle<YieldTermStructure> h) : base(new Period(15 , TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 20-year %EuriborSwapFixA index
   /// </summary>
	public class EuriborSwapFixA20Y : EuriborSwapFixA
	{
		public EuriborSwapFixA20Y(Handle<YieldTermStructure> h) : base(new Period(20 , TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 25-year %EuriborSwapFixA index
   /// </summary>
	public class EuriborSwapFixA25Y : EuriborSwapFixA
	{
		public EuriborSwapFixA25Y(Handle<YieldTermStructure> h) : base(new Period(25 , TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 30-year %EuriborSwapFixA index
   /// </summary>
	public class EuriborSwapFixA30Y : EuriborSwapFixA
	{
		public EuriborSwapFixA30Y(Handle<YieldTermStructure> h) : base(new Period(30 , TimeUnit.Years), h)
		{
		}
	}

}
