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
using QLNet;

namespace QLNet
{

    public class Date : IComparable
    {
        private DateTime date;

        public Date() { }							//! Default constructor returning a null date.
        public Date(int serialNumber)
        {			//! Constructor taking a serial number as given by Excel. Serial numbers in Excel have a known problem with leap year 1900
            date = (new DateTime(1899, 12, 31)).AddDays(serialNumber - 1);
        }
        public Date(int y, Month m, int d) : this(y, (int)m, d) { }
        public Date(int y, int m, int d) :		//! More traditional constructor.
            this(new DateTime(y, m, d)) { }
        public Date(DateTime d)
        {				//! System DateTime constructor
            date = d;
        }

        public int serialNumber() { return (date - new DateTime(1899, 12, 31).Date).Days + 1; }

        public static int operator -(Date d1, Date d2) { return (d1.date - d2.date).Days; }
        public static Date operator +(Date d, int days) { DateTime t = d.date; return new Date(t.AddDays(days)); }
        public static Date operator -(Date d, int days) { DateTime t = d.date; return new Date(t.AddDays(-days)); }
        public static Date operator +(Date d, TimeUnit u) { return advance(d, 1, u); }
        public static Date operator -(Date d, TimeUnit u) { return advance(d, -1, u); }
        public static Date operator +(Date d, Period p) { return advance(d, p.length(), p.units()); }
        public static Date operator -(Date d, Period p) { return advance(d, -p.length(), p.units()); }
        public static Date operator ++(Date d) { d = d + 1; return d; }
        public static Date operator --(Date d) { d = d - 1; return d; }
        public static Date Min(Date d1, Date d2) { return d1 < d2 ? d1 : d2; }
        public static Date Max(Date d1, Date d2) { return d1 > d2 ? d1 : d2; }

        public static bool operator ==(Date d1, Date d2)
        {
            return ((Object)d1 == null || (Object)d2 == null) ?
                   ((Object)d1 == null && (Object)d2 == null) :
                   d1.date == d2.date;
        }
        public static bool operator !=(Date d1, Date d2) { return (!(d1 == d2)); }
        public static bool operator <(Date d1, Date d2) { return (d1.date < d2.date); }
        public static bool operator <=(Date d1, Date d2) { return (d1.date <= d2.date); }
        public static bool operator >(Date d1, Date d2) { return (d1.date > d2.date); }
        public static bool operator >=(Date d1, Date d2) { return (d1.date >= d2.date); }

        public int Day { get { return date.Day; } }
        public int Month { get { return date.Month; } }
        public int Year { get { return date.Year; } }
        public int DayOfYear { get { return date.DayOfYear; } }
        public DayOfWeek DayOfWeek { get { return date.DayOfWeek; } }
        public static Date MaxValue { get { return new Date(DateTime.MaxValue); } }
        public static Date MinValue { get { return new Date(DateTime.MinValue); } }
        public static Date Today { get { return new Date(DateTime.Today); } }

        public static bool IsLeapYear(int y) { return DateTime.IsLeapYear(y); }
        public static int DaysInMonth(int y, int m) { return DateTime.DaysInMonth(y, m); }

        //! next given weekday following or equal to the given date
        public Date nextWeekday(Date d, DayOfWeek dayOfWeek)
        {
            int wd = dayOfWeek - d.DayOfWeek;
            return d + (wd >= 0 ? wd : (7 + wd));
        }
        //! n-th given weekday in the given month and year, e.g., the 4th Thursday of March, 1998 was March 26th, 1998.
        public static Date nthWeekday(int nth, DayOfWeek dayOfWeek, int m, int y)
        {
            if (nth < 1 || nth > 5) throw new ArgumentException("Wrong n-th weekday in a given month/year: " + nth);
            DayOfWeek first = new DateTime(y, m, 1).DayOfWeek;
            int skip = nth - (dayOfWeek >= first ? 1 : 0);
            return new Date(y, m, 1) + (int)(dayOfWeek - first + skip * 7);
        }
        public static int monthOffset(int m, bool leapYear)
        {
            int[] MonthOffset = {
            0,  31,  59,  90, 120, 151,   // Jan - Jun
            181, 212, 243, 273, 304, 334,   // Jun - Dec
            365     // used in dayOfMonth to bracket day
        };
            return (MonthOffset[m - 1] + ((leapYear && m > 1) ? 1 : 0));
        }
        public Date endOfMonth(Date d)
        {
            return (d - d.Day + DaysInMonth(d.Year, d.Month));
        }
        public static bool isEndOfMonth(Date d) { return (d.Day == DaysInMonth(d.Year, d.Month)); }

        public static Date advance(Date d, int n, TimeUnit u)
        {
            switch (u)
            {
                case TimeUnit.Days:
                    return d + n;
                case TimeUnit.Weeks:
                    return d + 7 * n;
                case TimeUnit.Months:
                    { DateTime t = d.date; return new Date(t.AddMonths(n)); }
                case TimeUnit.Years:
                    { DateTime t = d.date; return new Date(t.AddYears(n)); }
                default:
                    throw Error.UnknownTimeUnit(u);
            }
        }
        public string ToLongDateString() { return date.ToLongDateString(); }
        public string ToShortDateString() { return date.ToShortDateString(); }
        public override string ToString() { return this.ToShortDateString(); }
        public override bool Equals(object o) { return (this == (Date)o); }
        public override int GetHashCode() { return 0; }

        // IComparable interface
        public int CompareTo(object obj)
        {
            if (this < (Date)obj)
                return -1;
            else if (this == (Date)obj)
                return 0;
            else return 1;
        }
    }
}

