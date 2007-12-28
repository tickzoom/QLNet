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
   public abstract class Event : Observable
   {
		   public void Dispose()
		   {
		   }
		   //! \name Event interface
		   //@{
		   //! returns the date at which the event occurs
		   public abstract DDate date();

		   //! returns true if an event has already occurred before a date
         //        ! If QL_TODAYS_PAYMENT is true, then a payment event has not
         //            occurred if the input date is the same as the event date,
         //            and so includeToday should be defaulted to true.
         //
         //            This should be the only place in the code that is affected
         //            directly by QL_TODAYS_PAYMENT
         //        
		   public bool hasOccurred(DDate d)
		   {
			   return hasOccurred(d, true);
		   }
		   
         #if QL_TODAYS_PAYMENTS
		      public bool hasOccurred(Date d, bool includeToday)
		      public bool hasOccurred(Date d)
		      {
			      hasOccurred(d, false);
		      }
			#else
		      public bool hasOccurred(DDate d, bool includeToday)
			#endif
			    {
			   if (includeToday)
			   {
				   return date() < d;
			   }
			   else
			   {
				   return date() <= d;
			   }
		   }
		   //@}

		   //! \name Visitability
		   //@{

		   // inline definitions
   	
		   public void accept(ref AcyclicVisitor v)
		   {
            //Visitor<Event> v1 = v as Visitor<Event>;
			   //if (v1 != 0)
				//   v1.visit( this);
			   //'else
				throw new Exception("not an event visitor");
		   }
		   //@}
	   }
}
