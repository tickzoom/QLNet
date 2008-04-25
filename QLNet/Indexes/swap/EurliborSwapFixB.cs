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
   /// %EurliborSwapFixB index base class
   /// EurliborSwapFixB indexes fixed by ISDA in cooperation with
   /// Reuters and Intercapital Brokers at 11:00AM London.
   /// Reuters page ISDAFIX2 or EURSFIXLB=.
   /// Further info can be found at: <http://www.isda.org/fix/isdafix.html>.
   /// \warning The 1Y swap's floating leg is based on Euribor3M; the
   /// floating legs of longer swaps are based on Euribor6M
   /// </summary>
	public class EurliborSwapFixB : SwapIndex
	{
        public EurliborSwapFixB(Period tenor)
            : base("EurliborSwapFixB", tenor, 2, new EURCurrency(), new TARGET(), new Period(1, TimeUnit.Years), BusinessDayConvention.ModifiedFollowing, new Thirty360(Thirty360.Thirty360Convention.BondBasis),
                tenor > new Period(1, TimeUnit.Years) ? 
                    new Euribor6M(new Handle<YieldTermStructure>()) as IborIndex : 
                        new Euribor3M(new Handle<YieldTermStructure>()) as IborIndex)
        {
        }
        public EurliborSwapFixB(Period tenor, Handle<YieldTermStructure> h)
            : base("EurliborSwapFixB", tenor, 2, new EURCurrency(), new TARGET(), new Period(1, TimeUnit.Years), BusinessDayConvention.ModifiedFollowing, new Thirty360(Thirty360.Thirty360Convention.BondBasis),
                tenor > new Period(1, TimeUnit.Years) ? 
                    new Euribor6M(h) as IborIndex : new Euribor3M(h) as IborIndex)
		{
		}
	}

   /// <summary>
   /// 1-year %EurliborSwapFixB index
   /// </summary>
	public class EurliborSwapFixB1Y : EurliborSwapFixB
	{
		public EurliborSwapFixB1Y(Handle<YieldTermStructure> h) : base(new Period(1, TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 2-year %EurliborSwapFixB index
   /// </summary>
	public class EurliborSwapFixB2Y : EurliborSwapFixB
	{
		public EurliborSwapFixB2Y(Handle<YieldTermStructure> h) : base(new Period(2, TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 3-year %EurliborSwapFixB index
   /// </summary>
	public class EurliborSwapFixB3Y : EurliborSwapFixB
	{
		public EurliborSwapFixB3Y(Handle<YieldTermStructure> h) : base(new Period(3, TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 4-year %EurliborSwapFixB index
   /// </summary>
	public class EurliborSwapFixB4Y : EurliborSwapFixB
	{
		public EurliborSwapFixB4Y(Handle<YieldTermStructure> h) : base(new Period(4, TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 5-year %EurliborSwapFixB index
   /// </summary>
	public class EurliborSwapFixB5Y : EurliborSwapFixB
	{
		public EurliborSwapFixB5Y(Handle<YieldTermStructure> h) : base(new Period(5, TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 6-year %EurliborSwapFixB index
   /// </summary>
	public class EurliborSwapFixB6Y : EurliborSwapFixB
	{
		public EurliborSwapFixB6Y(Handle<YieldTermStructure> h) : base(new Period(6, TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 7-year %EurliborSwapFixB index
   /// </summary>
	public class EurliborSwapFixB7Y : EurliborSwapFixB
	{
		public EurliborSwapFixB7Y(Handle<YieldTermStructure> h) : base(new Period(7, TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 8-year %EurliborSwapFixB index
   /// </summary>
	public class EurliborSwapFixB8Y : EurliborSwapFixB
	{
		public EurliborSwapFixB8Y(Handle<YieldTermStructure> h) : base(new Period(8, TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 9-year %EurliborSwapFixB index
   /// </summary>
	public class EurliborSwapFixB9Y : EurliborSwapFixB
	{
		public EurliborSwapFixB9Y(Handle<YieldTermStructure> h) : base(new Period(9, TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 10-year %EurliborSwapFixB index
   /// </summary>
	public class EurliborSwapFixB10Y : EurliborSwapFixB
	{
		public EurliborSwapFixB10Y(Handle<YieldTermStructure> h) : base(new Period(10, TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 12-year %EurliborSwapFixB index
   /// </summary>
	public class EurliborSwapFixB12Y : EurliborSwapFixB
	{
		public EurliborSwapFixB12Y(Handle<YieldTermStructure> h) : base(new Period(12, TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 15-year %EurliborSwapFixB index
   /// </summary>
	public class EurliborSwapFixB15Y : EurliborSwapFixB
	{
		public EurliborSwapFixB15Y(Handle<YieldTermStructure> h) : base(new Period(15, TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 20-year %EurliborSwapFixB index
   /// </summary>
	public class EurliborSwapFixB20Y : EurliborSwapFixB
	{
		public EurliborSwapFixB20Y(Handle<YieldTermStructure> h) : base(new Period(20, TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 25-year %EurliborSwapFixB index
   /// </summary>
	public class EurliborSwapFixB25Y : EurliborSwapFixB
	{
		public EurliborSwapFixB25Y(Handle<YieldTermStructure> h) : base(new Period(25, TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 30-year %EurliborSwapFixB index
   /// </summary>
	public class EurliborSwapFixB30Y : EurliborSwapFixB
	{
		public EurliborSwapFixB30Y(Handle<YieldTermStructure> h) : base(new Period(30, TimeUnit.Years), h)
		{
		}
	}

}
