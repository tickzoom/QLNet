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
   /// %EurliborSwapFixA index base class
   /// EurliborSwapFixA indexes fixed by ISDA in cooperation with
   /// Reuters and Intercapital Brokers at 10:00 AM London.
   /// Reuters page ISDAFIX2 or EURSFIXLA=.
   /// Further info can be found at: <http://www.isda.org/fix/isdafix.html>.
   /// \warning The 1Y swap's floating leg is based on Euribor3M; the
   /// floating legs of longer swaps are based on Euribor6M
   /// </summary>
	public class EurliborSwapFixA : SwapIndex
	{
        public EurliborSwapFixA(Period tenor)
            : base("EurliborSwapFixA", tenor, 2, new EURCurrency(), new TARGET(), new Period(1, TimeUnit.Years), BusinessDayConvention.ModifiedFollowing, new Thirty360(Thirty360.Thirty360Convention.BondBasis),
                tenor > new Period(1, TimeUnit.Years) ? 
                    new Euribor6M(new Handle<YieldTermStructure>()) as IborIndex : 
                        new Euribor3M(new Handle<YieldTermStructure>()) as IborIndex)
        {
        }
        public EurliborSwapFixA(Period tenor, Handle<YieldTermStructure> h)
            : base("EurliborSwapFixA", tenor, 2, new EURCurrency(), new TARGET(), new Period(1, TimeUnit.Years), BusinessDayConvention.ModifiedFollowing, new Thirty360(Thirty360.Thirty360Convention.BondBasis),
                tenor > new Period(1, TimeUnit.Years) ? 
                    new Euribor6M(h) as IborIndex : new Euribor3M(h) as IborIndex)
		{
		}
	}

   /// <summary>
   /// 1-year %EurliborSwapFixA index
   /// </summary>
	public class EurliborSwapFixA1Y : EurliborSwapFixA
	{
		public EurliborSwapFixA1Y(Handle<YieldTermStructure> h) : base(new Period(1, TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 2-year %EurliborSwapFixA index
   /// </summary>
	public class EurliborSwapFixA2Y : EurliborSwapFixA
	{
		public EurliborSwapFixA2Y(Handle<YieldTermStructure> h) : base(new Period(2, TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 3-year %EurliborSwapFixA index
   /// </summary>
	public class EurliborSwapFixA3Y : EurliborSwapFixA
	{
		public EurliborSwapFixA3Y(Handle<YieldTermStructure> h) : base(new Period(3, TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 4-year %EurliborSwapFixA index
   /// </summary>
	public class EurliborSwapFixA4Y : EurliborSwapFixA
	{
		public EurliborSwapFixA4Y(Handle<YieldTermStructure> h) : base(new Period(4, TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 5-year %EurliborSwapFixA index
   /// </summary>
	public class EurliborSwapFixA5Y : EurliborSwapFixA
	{
		public EurliborSwapFixA5Y(Handle<YieldTermStructure> h) : base(new Period(5, TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 6-year %EurliborSwapFixA index
   /// </summary>
	public class EurliborSwapFixA6Y : EurliborSwapFixA
	{
		public EurliborSwapFixA6Y(Handle<YieldTermStructure> h) : base(new Period(6, TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 7-year %EurliborSwapFixA index
   /// </summary>
	public class EurliborSwapFixA7Y : EurliborSwapFixA
	{
		public EurliborSwapFixA7Y(Handle<YieldTermStructure> h) : base(new Period(7, TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 8-year %EurliborSwapFixA index
   /// </summary>
	public class EurliborSwapFixA8Y : EurliborSwapFixA
	{
		public EurliborSwapFixA8Y(Handle<YieldTermStructure> h) : base(new Period(8, TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 9-year %EurliborSwapFixA index
   /// </summary>
	public class EurliborSwapFixA9Y : EurliborSwapFixA
	{
		public EurliborSwapFixA9Y(Handle<YieldTermStructure> h) : base(new Period(9, TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 10-year %EurliborSwapFixA index
   /// </summary>
	public class EurliborSwapFixA10Y : EurliborSwapFixA
	{
		public EurliborSwapFixA10Y(Handle<YieldTermStructure> h) : base(new Period(10, TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 12-year %EurliborSwapFixA index
   /// </summary>
	public class EurliborSwapFixA12Y : EurliborSwapFixA
	{
		public EurliborSwapFixA12Y(Handle<YieldTermStructure> h) : base(new Period(12, TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 15-year %EurliborSwapFixA index
   /// </summary>
	public class EurliborSwapFixA15Y : EurliborSwapFixA
	{
		public EurliborSwapFixA15Y(Handle<YieldTermStructure> h) : base(new Period(15, TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 20-year %EurliborSwapFixA index
   /// </summary>
	public class EurliborSwapFixA20Y : EurliborSwapFixA
	{
		public EurliborSwapFixA20Y(Handle<YieldTermStructure> h) : base(new Period(20, TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 25-year %EurliborSwapFixA index
   /// </summary>
	public class EurliborSwapFixA25Y : EurliborSwapFixA
	{
		public EurliborSwapFixA25Y(Handle<YieldTermStructure> h) : base(new Period(25, TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 30-year %EurliborSwapFixA index
   /// </summary>
	public class EurliborSwapFixA30Y : EurliborSwapFixA
	{
		public EurliborSwapFixA30Y(Handle<YieldTermStructure> h) : base(new Period(30, TimeUnit.Years), h)
		{
		}
	}

}
