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
using System;
using System.Collections.Generic;
using QLNet;

namespace QLNet {
    //! global repository for past index fixings
    public static class IndexManager {
        private static Dictionary<string, TimeSeries<double>> data_ = new Dictionary<string, TimeSeries<double>>();
		
		//! returns whether historical fixings were stored for the index
        public static bool hasHistory(string name) {
			return data_.ContainsKey(name);
		}
		
        //! returns the (possibly empty) history of the index fixings
        public static TimeSeries<double> getHistory(string name) {
            return data_[name];
		}
		
        //! stores the historical fixings of the index
        public static void setHistory(string name, TimeSeries<double> history) {
            data_[name] = history;
		}

        //! observer notifying of changes in the index fixings
        public static TimeSeries<double> notifier(string name) {
            return data_[name];
        }

        //! returns all names of the indexes for which fixings were stored
        public static List<string> histories() {
            List<string> t = new List<string>();
            foreach (string s in data_.Keys)
                t.Add(s);
	        return t;
	    }
	
        //! clears the historical fixings of the index
        public static void clearHistory(string name) {
			data_[name].Clear();
		}

        //! clears all stored fixings
        public static void clearHistories() {
			data_.Clear();
		}
	}
}