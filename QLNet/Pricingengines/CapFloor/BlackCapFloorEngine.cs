using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QLNet
{
   /// <summary>
   /// Black-formula cap/floor engine
   /// \ingroup capfloorengines
   /// </summary>
   public class BlackCapFloorEngine : CapFloorEngine
   {
      public BlackCapFloorEngine(Handle<YieldTermStructure> termStructure, double vol)
         : this(termStructure, vol, new Actual365Fixed()) { }
      public BlackCapFloorEngine(Handle<YieldTermStructure> termStructure,
                                 double vol, DayCounter dc )
      {
      }

      public BlackCapFloorEngine(Handle<YieldTermStructure> termStructure, Handle<Quote> vol)
         : this(termStructure, vol, new Actual365Fixed()) { }

      public BlackCapFloorEngine(Handle<YieldTermStructure> termStructure,
                                 Handle<Quote> vol, DayCounter dc)
      {

      }

      public BlackCapFloorEngine(Handle<YieldTermStructure> discountCurve,
                                 Handle<OptionletVolatilityStructure> vol)
      {
      }
   }
}
