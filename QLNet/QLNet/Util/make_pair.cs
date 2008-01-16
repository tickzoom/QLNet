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

   public class Pair<T1, T2>
   {
      public Pair(T1 t1,T2 t2)
      {
         this.first = t1;
         this.second = t2;
      }

      public T1 first;
      public T2 second;
   } 
}
