/*
 This file is part of QLNet Project http://trac2.assembla.com/QLNet
 
 QLNet is a porting of QuantLib, a free-software/open-source library
 for financial quantitative analysts and developers - http://quantlib.org/
 The license is available online at http://quantlib.org/license.shtml.
 
 This program is distributed in the hope that it will be useful, but WITHOUT
 ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 FOR A PARTICULAR PURPOSE.  See the license for more details.
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace QLNet
{
   //! Frequency of events
   /*! \ingroup datetime */
   public enum Frequency
   {
      NoFrequency = -1,     //!< null frequency
      Once = 0,             //!< only once, e.g., a zero-coupon
      Annual = 1,           //!< once a year
      Semiannual = 2,       //!< twice a year
      EveryFourthMonth = 3, //!< every fourth month
      Quarterly = 4,        //!< every third month
      Bimonthly = 6,        //!< every second month
      Monthly = 12,         //!< once a month
      Biweekly = 26,        //!< every second week
      Weekly = 52,          //!< once a week
      Daily = 365           //!< once a day
   };
}
