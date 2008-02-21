/*
 Copyright (C) 2008 Andrea Maggiulli
  
 This file is part of QLNet Project http://trac2.assembla.com/QLNet

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
using System.Text;

namespace QLNet
{
   /// <summary>
   /// U.S. dollar
   /// The ISO three-letter code is USD; the numeric code is 840.
   /// It is divided in 100 cents.
   /// </summary>
   public class USDCurrency : Currency
   {
      public USDCurrency()
      {
         _data = new Data("U.S. dollar", "USD", 840,"$", "\xA2", 100,new Rounding(),"%3% %1$.2f");
      }

   }

   /// <summary>
   /// Peruvian nuevo sol
   /// The ISO three-letter code is PEN; the numeric code is 604.
   /// It is divided in 100 centimos.
   /// </summary>
   public class PENCurrency : Currency
   {
      public PENCurrency()
      {
         _data = new Data("Peruvian nuevo sol", "PEN", 604,"S/.", "", 100,new Rounding(),"%3% %1$.2f");
      }

   }

   /// <summary>
   /// Peruvian inti
   /// The ISO three-letter code was PEI.
   /// It was divided in 100 centimos. A numeric code is not available;
   /// as per ISO 3166-1, we assign 998 as a user-defined code.
   /// Obsoleted by the nuevo sol since July 1991.
   /// </summary>
   public class PEICurrency : Currency
   {
      public PEICurrency()
      {
         _data = new Data("Peruvian inti", "PEI", 998,"I/.", "", 100,new Rounding(),"%3% %1$.2f");
      }

   }

   /// <summary>
   /// Peruvian sol
   /// The ISO three-letter code was PEH. A numeric code is not available;
   /// as per ISO 3166-1, we assign 999 as a user-defined code.
   /// It was divided in 100 centavos.
   /// Obsoleted by the inti since February 1985
   /// </summary>
   public class PEHCurrency : Currency
   {
      public PEHCurrency()
      {
         _data = new Data("Peruvian sol", "PEH", 999,"S./", "", 100,new Rounding(),"%3% %1$.2f");
      }

   }

}
