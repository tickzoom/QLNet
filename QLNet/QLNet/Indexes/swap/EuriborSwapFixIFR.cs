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
   /// %EuriborSwapFixIFR index base class
   /// EuriborSwapFixIFR indexes published by IFR Markets and
   /// distributed by Reuters page TGM42281 and by Telerate.
   /// For more info see <http://www.ifrmarkets.com>.
   /// \warning The 1Y swap's floating leg is based on Eurlibor3M; the
   /// floating legs of longer swaps are based on Eurlibor6M
   /// </summary>
	public class EuriborSwapFixIFR : SwapIndex
	{
        public EuriborSwapFixIFR(Period tenor)
            : base("EuriborSwapFixIFR", tenor, 2, new EURCurrency(), new TARGET(), new Period(1, TimeUnit.Years), BusinessDayConvention.ModifiedFollowing, new Thirty360(Thirty360.Thirty360Convention.BondBasis),
                tenor > new Period(1, TimeUnit.Years) ? 
                    new Euribor6M(new Handle<YieldTermStructure>()) as IborIndex : 
                        new Euribor3M(new Handle<YieldTermStructure>()) as IborIndex)
        {
        }
        public EuriborSwapFixIFR(Period tenor, Handle<YieldTermStructure> h)
            : base("EuriborSwapFixIFR", tenor, 2, new EURCurrency(), new TARGET(), new Period(1, TimeUnit.Years), BusinessDayConvention.ModifiedFollowing, new Thirty360(Thirty360.Thirty360Convention.BondBasis),
                tenor > new Period(1, TimeUnit.Years) ? 
                    new Euribor6M(h) as IborIndex : new Euribor3M(h) as IborIndex)
		{
		}
	}

   /// <summary>
   /// 1-year %EuriborSwapFixIFR3M index
   /// </summary>
	public class EuriborSwapFixIFR1Y : EuriborSwapFixIFR
	{
		public EuriborSwapFixIFR1Y(Handle<YieldTermStructure> h) : base(new Period(1, TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 2-year %EuriborSwapFixIFR index
   /// </summary>
	public class EuriborSwapFixIFR2Y : EuriborSwapFixIFR
	{
		public EuriborSwapFixIFR2Y(Handle<YieldTermStructure> h) : base(new Period(2, TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 3-year %EuriborSwapFixIFR index
   /// </summary>
	public class EuriborSwapFixIFR3Y : EuriborSwapFixIFR
	{
		public EuriborSwapFixIFR3Y(Handle<YieldTermStructure> h) : base(new Period(3, TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 4-year %EuriborSwapFixIFR index
   /// </summary>
	public class EuriborSwapFixIFR4Y : EuriborSwapFixIFR
	{
		public EuriborSwapFixIFR4Y(Handle<YieldTermStructure> h) : base(new Period(4, TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 5-year %EuriborSwapFixIFR index
   /// </summary>
	public class EuriborSwapFixIFR5Y : EuriborSwapFixIFR
	{
		public EuriborSwapFixIFR5Y(Handle<YieldTermStructure> h) : base(new Period(5, TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 6-year %EuriborSwapFixIFR index
   /// </summary>
	public class EuriborSwapFixIFR6Y : EuriborSwapFixIFR
	{
		public EuriborSwapFixIFR6Y(Handle<YieldTermStructure> h) : base(new Period(6, TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 7-year %EuriborSwapFixIFR index
   /// </summary>
	public class EuriborSwapFixIFR7Y : EuriborSwapFixIFR
	{
		public EuriborSwapFixIFR7Y(Handle<YieldTermStructure> h) : base(new Period(7, TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 8-year %EuriborSwapFixIFR index
   /// </summary>
	public class EuriborSwapFixIFR8Y : EuriborSwapFixIFR
	{
		public EuriborSwapFixIFR8Y(Handle<YieldTermStructure> h) : base(new Period(8, TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 9-year %EuriborSwapFixIFR index
   /// </summary>
	public class EuriborSwapFixIFR9Y : EuriborSwapFixIFR
	{
		public EuriborSwapFixIFR9Y(Handle<YieldTermStructure> h) : base(new Period(9, TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 10-year %EuriborSwapFixIFR index
   /// </summary>
	public class EuriborSwapFixIFR10Y : EuriborSwapFixIFR
	{
		public EuriborSwapFixIFR10Y(Handle<YieldTermStructure> h) : base(new Period(10, TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 12-year %EuriborSwapFixIFR index
   /// </summary>
	public class EuriborSwapFixIFR12Y : EuriborSwapFixIFR
	{
		public EuriborSwapFixIFR12Y(Handle<YieldTermStructure> h) : base(new Period(12, TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 15-year %EuriborSwapFixIFR index
   /// </summary>
	public class EuriborSwapFixIFR15Y : EuriborSwapFixIFR
	{
		public EuriborSwapFixIFR15Y(Handle<YieldTermStructure> h) : base(new Period(15, TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 20-year %EuriborSwapFixIFR index
   /// </summary>
	public class EuriborSwapFixIFR20Y : EuriborSwapFixIFR
	{
		public EuriborSwapFixIFR20Y(Handle<YieldTermStructure> h) : base(new Period(20, TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 25-year %EuriborSwapFixIFR index
   /// </summary>
	public class EuriborSwapFixIFR25Y : EuriborSwapFixIFR
	{
		public EuriborSwapFixIFR25Y(Handle<YieldTermStructure> h) : base(new Period(25, TimeUnit.Years), h)
		{
		}
	}

   /// <summary>
   /// 30-year %EuriborSwapFixIFR index
   /// </summary>
	public class EuriborSwapFixIFR30Y : EuriborSwapFixIFR
	{
		public EuriborSwapFixIFR30Y(Handle<YieldTermStructure> h) : base(new Period(30, TimeUnit.Years), h)
		{
		}
	}

}
