/*
 Copyright (C) 2008 Siarhei Novik (snovik@gmail.com)
  
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
using System.Text;

namespace QLNet {
    public class Error {
		public static ArgumentException UnknownTimeUnit(TimeUnit u) {
			return new ArgumentException("Unknown TimeUnit: " + u); }
		public static ArgumentException UnknownFrequency(Frequency f) {
			return new ArgumentException("Unknown frequency: " + f); }
		public static ArgumentException UnknownBusinessDayConvention(BusinessDayConvention c) {
			return new ArgumentException("Unknown business-day convention: " + c); }
		public static ArgumentException UnknownDateGenerationRule(DateGeneration.Rule r) {
			return new ArgumentException("Unknown DateGeneration rule: " + r); }

		public static ApplicationException MissingImplementation() {
			return new ApplicationException("No implementation provided"); }

		public static ArgumentException CannotInitiateFrequency(Period p) {
			return new ArgumentException("Cannot instantiate Frequency for " + p.ToString()); }
    }
}
