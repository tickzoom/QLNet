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
   public class make_pair<T1, T2>
   {
      public T1 first
      {
         get { return this.t1; }
         set { this.t1 = value; }
      }

      public T2 second
      {
         get { return this.t2; }
         set { this.t2 = value; }
      }

      public make_pair(T1 type1, T2 type2)
      {
         this.t1 = type1;
         this.t2 = type2;
      }
      private T1 t1;
      private T2 t2;
   }
}
