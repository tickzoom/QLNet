/*
 Copyright (C) 2008 Siarhei Novik (snovik@gmail.com)
  
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
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QLNet {
    public static partial class Utils {
        public static T Get<T>(this List<T> v, int i) { return Get(v, i, default(T)); }
        public static T Get<T>(this List<T> v, int i, T defval) {
            if (v == null || v.Count == 0)    return defval;
            else if (i >= v.Count)            return v.Last();
            else                              return v[i];  
        }

        public static double effectiveFixedRate(List<double> spreads, List<double> caps, List<double> floors, int i) {
            double result = Get(spreads, i);
            double floor = Get(floors, i);
            double cap = Get(caps, i);
            if (floor != default(double)) result = System.Math.Max(floor, result);
            if (cap != default(double)) result = System.Math.Min(cap, result);
            return result;
        }

        public static bool noOption(List<double> caps, List<double> floors, int i) {
            return (Get(caps, i) == default(double)) && (Get(floors, i) == default(double));
        }
    }

    // this is a redefined collection class to emulate array-type behaviour at initialisation
    public class Array<T> : List<T> {
        public Array() : base() { }
        public Array(int size) : base(size) {
            for(int i=0; i<this.Capacity; i++)
                this.Add(default(T));
        }
    }
}
