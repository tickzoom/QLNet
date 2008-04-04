using System;
using System.Collections.Generic;
using System.Text;

namespace QLNet
{
    class Error
    {
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
