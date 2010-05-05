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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace QLNet {
	//! Payment schedule
    public class Schedule {
        #region properties
		private bool fullInterface_;
        private Period tenor_;
        public Period tenor() { CheckInterface(); return tenor_; }

        private Calendar calendar_;
        public Calendar calendar() { return calendar_; }

        private BusinessDayConvention convention_, terminationDateConvention_;
        public BusinessDayConvention businessDayConvention() { return convention_; }
        public BusinessDayConvention terminationDateBusinessDayConvention { get { CheckInterface(); return terminationDateConvention_; } }
        
        private DateGeneration.Rule rule_;
        public DateGeneration.Rule rule { get { CheckInterface(); return rule_; } }

        private bool endOfMonth_;
        public bool endOfMonth { get { CheckInterface(); return endOfMonth_; } }

        private Date firstDate_, nextToLastDate_;

        // list of payment dates, i.e. adjusted to calendar and convention
        private List<Date> adjustedDates_ = new List<Date>();   
        // list of originally generated dates with the flag of regular periods, relevant only for Actual/Acutal conventions
        private List<Date> originalDates_ = new List<Date>();
        // list of regular periods, relevant only for Actual/Actual conventions
        // since it is in periods, its size is 1 element less than the dates size, see isRegular
        private List<bool> isRegular_ = new List<bool>();

        public List<Date> dates() { return new List<Date>(adjustedDates_); }
        public int Count { get { return adjustedDates_.Count; } }

        public Date date(int i) { return this[i]; }
        public Date this[int i] {
            get {
                CheckInterface();
                if (i >= adjustedDates_.Count)
                    throw new ArgumentException("i (" + i + ") must be less than or equal to " + (adjustedDates_.Count - 1));
                return adjustedDates_[i];
            }
        }
        public Date originalDate(int i) {
            CheckInterface();
            if (i >= originalDates_.Count)
                throw new ArgumentException("i (" + i + ") must be less than or equal to " + (originalDates_.Count - 1));
            return originalDates_[i];
        }
        // here i refers to the index on dates collection; isRegular collection is by 1 element smaller than dates
        public bool isRegular(int i) {
            CheckInterface();
            if (!(i <= isRegular_.Count && i > 0))
                throw new ArgumentException("index (" + i + ") must be in [1, " + isRegular_.Count + "]");
            return isRegular_[i-1];
        }
        public Date startDate() { return adjustedDates_.First(); }
        public Date endDate() { return adjustedDates_.Last(); }
        #endregion

        #region Constructors
        public Schedule() { }
        public Schedule(List<Date> dates__, Calendar calendar__, BusinessDayConvention convention__) {
            fullInterface_ = false;
            tenor_ = new Period();
            calendar_ = calendar__;
            convention_ = convention__;
            terminationDateConvention_ = convention__;
            rule_ = DateGeneration.Rule.Forward;
            endOfMonth_ = false;
            adjustedDates_ = dates__;
        }
        public Schedule(Date effectiveDate__, Date terminationDate__, Period tenor__, Calendar calendar__,
                 BusinessDayConvention convention__, BusinessDayConvention terminationDateConvention__,
                 DateGeneration.Rule rule__, bool endOfMonth__)
            : this(effectiveDate__, terminationDate__, tenor__, calendar__,
                 convention__, terminationDateConvention__, rule__, endOfMonth__, null, null) { }
        public Schedule(Date effectiveDate__, Date terminationDate__, Period tenor__, Calendar calendar__,
                 BusinessDayConvention convention__, BusinessDayConvention terminationDateConvention__,
                 DateGeneration.Rule rule__, bool endOfMonth__,
                 Date firstDate__, Date nextToLastDate__) {
            // first save the properties
            fullInterface_ = true;
            tenor_ = tenor__;
            calendar_ = calendar__;
            convention_ = convention__;
            terminationDateConvention_ = terminationDateConvention__;
            rule_ = rule__;
            endOfMonth_ = endOfMonth__;
            firstDate_ = firstDate__;
            nextToLastDate_ = nextToLastDate__;

            // sanity checks
            if (effectiveDate__ == null) throw new ArgumentException("Null effective date");
            if (terminationDate__ == null) throw new ArgumentException("Null termination  date");
            if (effectiveDate__ >= terminationDate__) throw new ArgumentException("Effective date (" + effectiveDate__ +
                       ") is later than or equal to termination date (" + terminationDate__ + ")");

            if (tenor_.length() == 0)
                rule_ = DateGeneration.Rule.Zero;
            else if (tenor_.length() < 0)
                throw new ArgumentException("Non positive tenor (" + tenor_ + ") is not allowed");

            if (firstDate_ != null) {
                switch (rule_) {
                    case DateGeneration.Rule.Backward:
                    case DateGeneration.Rule.Forward:
                        if (!(firstDate_ > effectiveDate__ && firstDate_ < terminationDate__))
                            throw new ArgumentException("First date (" + firstDate_ + ") is out of range [effective date (" + effectiveDate__
                                                        + "), termination date (" + terminationDate__ + ")]");
                        // we should ensure that the above condition is still verified after adjustment
                        break;
                    case DateGeneration.Rule.ThirdWednesday:
                        if (!IMM.isIMMdate(firstDate_, false))
                            throw new ArgumentException("first date (" + firstDate_ + ") is not an IMM date");
                        break;
                    case DateGeneration.Rule.Zero:
                    case DateGeneration.Rule.Twentieth:
                    case DateGeneration.Rule.TwentiethIMM:
                    case DateGeneration.Rule.OldCDS:
                    case DateGeneration.Rule.CDS:
                        throw new ArgumentException("First date is incompatible with " + rule_ + " date generation rule");
                    default:
                        throw new ArgumentException("Unknown DateGeneration rule: " + rule_);
                }
            }

            if (nextToLastDate_ != null) {
                switch (rule_) {
                    case DateGeneration.Rule.Backward:
                    case DateGeneration.Rule.Forward:
                        if (!(nextToLastDate_ > effectiveDate__ && nextToLastDate_ < terminationDate__))
                            throw new ArgumentException("Next to last date (" + nextToLastDate_ + ") out of range [effective date (" + effectiveDate__
                               + "), termination date (" + terminationDate__ + ")]");
                        // we should ensure that the above condition is still verified after adjustment
                        break;
                    case DateGeneration.Rule.ThirdWednesday:
                        if (!IMM.isIMMdate(firstDate_, false))
                            throw new ArgumentException("first date (" + firstDate_ + ") is not an IMM date");
                        break;
                    case DateGeneration.Rule.Zero:
                    case DateGeneration.Rule.Twentieth:
                    case DateGeneration.Rule.TwentiethIMM:
                    case DateGeneration.Rule.OldCDS:
                    case DateGeneration.Rule.CDS:
                        throw new ArgumentException("next to last is incompatible with " + rule_ + " date generation rule");
                    default:
                        throw new ArgumentException("Unknown DateGeneration rule: " + rule_);
                }
            }

            // calendar needed for endOfMonth adjustment
            Calendar nullCalendar = new NullCalendar();
            int periods = 1;
            Date seed = new Date(), exitDate;
            switch (rule_) {
                case DateGeneration.Rule.Zero:
                    tenor_ = new Period(0, TimeUnit.Days);
                    originalDates_.Add(effectiveDate__);
                    originalDates_.Add(terminationDate__);
                    isRegular_.Add(true);
                    break;

                case DateGeneration.Rule.Backward:
                    originalDates_.Add(terminationDate__);
                    seed = terminationDate__;
                    if (nextToLastDate_ != null) {
                        originalDates_.Insert(0, nextToLastDate_);
                        Date temp = nullCalendar.advance(seed, -periods * tenor_, convention_, endOfMonth_);
                        isRegular_.Insert(0, temp == nextToLastDate_);
                        seed = nextToLastDate_;
                    }
                    exitDate = effectiveDate__;
                    if (firstDate_ != null)
                        exitDate = firstDate_;
                    while (true) {
                        Date temp = nullCalendar.advance(seed, -periods * tenor_, convention_, endOfMonth_);
                        if (temp < exitDate) {
                            if (firstDate_ != null && (calendar_.adjust(originalDates_.First(), convention_) !=
                                 calendar_.adjust(firstDate_, convention_))) {
                                originalDates_.Insert(0, firstDate_);
                                isRegular_.Insert(0, false);
                            }
                            break;
                        } else {
                            originalDates_.Insert(0, temp);
                            isRegular_.Insert(0, true);
                            ++periods;
                        }
                    }
                    if (endOfMonth_ && calendar_.isEndOfMonth(seed))
                        convention_ = BusinessDayConvention.Preceding;
                    if (calendar_.adjust(originalDates_[0], convention_) != calendar_.adjust(effectiveDate__, convention_)) {
                        originalDates_.Insert(0, effectiveDate__);
                        isRegular_.Insert(0, false);
                    }
                    break;

                case DateGeneration.Rule.Twentieth:
                case DateGeneration.Rule.TwentiethIMM:
                case DateGeneration.Rule.ThirdWednesday:
                case DateGeneration.Rule.OldCDS:
                case DateGeneration.Rule.CDS:
                    if (endOfMonth_) 
                        throw new ArgumentException("endOfMonth convention is incompatible with " + rule_ + " date generation rule");
                    goto case DateGeneration.Rule.Forward;			// fall through

                case DateGeneration.Rule.Forward:
                    if (rule_ == DateGeneration.Rule.CDS) {
                       originalDates_.Add(previousTwentieth(effectiveDate__,DateGeneration.Rule.CDS));
                    } else {
                       originalDates_.Add(effectiveDate__);
                    }
                    
                    seed = effectiveDate__;
                    if (firstDate_ != null) {
                        originalDates_.Add(firstDate_);
                        Date temp = nullCalendar.advance(seed, periods * tenor_, convention_, endOfMonth_);
                        isRegular_.Add(temp == firstDate_);
                        seed = firstDate_;
                    } else if (rule_ == DateGeneration.Rule.Twentieth ||
                               rule_ == DateGeneration.Rule.TwentiethIMM ||
                               rule_ == DateGeneration.Rule.OldCDS ||
                               rule_ == DateGeneration.Rule.CDS)
                    {
                        Date next20th = nextTwentieth(effectiveDate__, rule_);
                        if (rule_ == DateGeneration.Rule.OldCDS) {
                           // distance rule inforced in natural days
                           long stubDays = 30;
                           if (next20th - effectiveDate__ < stubDays) {
                              // +1 will skip this one and get the next
                              next20th = nextTwentieth(next20th + 1, rule_);
                           }
                        }
                        if (next20th != effectiveDate__) {
                            originalDates_.Add(next20th);
                            isRegular_.Add(false);
                            seed = next20th;
                        }
                    }

                    exitDate = terminationDate__;
                    if (nextToLastDate_ != null)
                        exitDate = nextToLastDate_;
                    while (true) {
                        Date temp = nullCalendar.advance(seed, periods * tenor_, convention_, endOfMonth_);
                        if (temp > exitDate) {
                            if (nextToLastDate_ != null &&
                                (calendar_.adjust(originalDates_.Last(), convention_) !=
                                 calendar_.adjust(nextToLastDate_, convention_))) {
                                originalDates_.Add(nextToLastDate_);
                                isRegular_.Add(false);
                            }
                            break;
                        } else {
                            originalDates_.Add(temp);
                            isRegular_.Add(true);
                            ++periods;
                        }
                    }
                    if (endOfMonth_ && calendar_.isEndOfMonth(seed))
                        convention_ = BusinessDayConvention.Preceding;

                    if (calendar_.adjust(originalDates_.Last(), terminationDateConvention_) != calendar_.adjust(terminationDate__, terminationDateConvention_)) {
                        if (rule_ == DateGeneration.Rule.Twentieth || 
                            rule_ == DateGeneration.Rule.TwentiethIMM ||
                            rule_ == DateGeneration.Rule.OldCDS ||
                            rule_ == DateGeneration.Rule.CDS ) {
                            originalDates_.Add(nextTwentieth(terminationDate__, rule_));
                            isRegular_.Add(true);
                        } else {
                            originalDates_.Add(terminationDate__);
                            isRegular_.Add(false);
                        }
                    }
                    break;

                default:
                    throw new ArgumentException("Unknown DateGeneration rule: " + rule_);
            }

            // adjustments to holidays, etc.
            if (rule_ == DateGeneration.Rule.ThirdWednesday)
                for (int i = 1; i < originalDates_.Count; ++i)
                    originalDates_[i] = Date.nthWeekday(3, DayOfWeek.Wednesday, originalDates_[i].Month, originalDates_[i].Year);

            if (endOfMonth && calendar_.isEndOfMonth(seed))
            {
               // adjust to end of month
               if (convention_ == BusinessDayConvention.Unadjusted)
               {
                  for (int i = 0; i < originalDates_.Count; ++i)
                     originalDates_[i] = Date.endOfMonth(originalDates_[i]);
               }
               else
               {
                  for (int i = 0; i < originalDates_.Count; ++i)
                     originalDates_[i] = calendar_.endOfMonth(originalDates_[i]);
               }
               if (terminationDateConvention_ == BusinessDayConvention.Unadjusted)
                  originalDates_[originalDates_.Count - 1] = Date.endOfMonth(originalDates_.Last());
               else
                  originalDates_[originalDates_.Count - 1] = calendar_.endOfMonth(originalDates_.Last());
            }
            else
            {
               // first date not adjusted for CDS schedules
               if (rule_ != DateGeneration.Rule.OldCDS)
                  originalDates_[0] = calendar_.adjust(originalDates_[0], convention_);
               for (int i = 1; i < originalDates_.Count; ++i)
                  originalDates_[i] = calendar_.adjust(originalDates_[i], convention_);

               foreach (Date d in originalDates_)
                  adjustedDates_.Add(d);

               // termination date is NOT adjusted as per ISDA specifications, unless otherwise specified in the
               // confirmation of the deal or unless we're creating a CDS schedule
               if (terminationDateConvention_ != BusinessDayConvention.Unadjusted
                   || rule_ == DateGeneration.Rule.Twentieth
                   || rule_ == DateGeneration.Rule.TwentiethIMM
                   || rule_ == DateGeneration.Rule.OldCDS
                   || rule_ == DateGeneration.Rule.CDS)
                  adjustedDates_[adjustedDates_.Count - 1] = calendar_.adjust(originalDates_.Last(), terminationDateConvention_);
            }
        } 
        #endregion

		private void CheckInterface() {	if (!fullInterface_) throw new ArgumentException("full interface not available"); }
		
        // returns the period of the Schedule during which Date d happens
        public int periodOfDate(Date d) {
            int result = adjustedDates_.BinarySearch(d);
            if (result < 0)         // not found directly
                result = ~result;   // the next element larger than the search value
            return result;
        }

        //// looks up the date of the previous period point relative to Date d
        //public Date previousDate(Date d) {
        //    int i = periodOfDate(d);
        //    return this[i - 1];
        //}

        //// looks up the date of the next period point relative to Date d
        //public Date nextDate(Date d) {
        //    int i = periodOfDate(d);
        //    return this[i + 1];
        //}

        // Iterator interface
        private int i_ = 0;
        public int i { get { return i_; } }
        public bool isLast() { return i >= Count - 1; }     // returns true when interator is at the end of collection
        public IEnumerator GetEnumerator() {
            i_ = 0;
            foreach (Date d in adjustedDates_) {
                yield return d;
                i_++;
            }
        }


        Date nextTwentieth(Date d, DateGeneration.Rule rule) {
            Date result = new Date(20, d.month(), d.year());
            if (result < d)
                result += new Period(1, TimeUnit.Months);
            if (rule == DateGeneration.Rule.TwentiethIMM ||
                rule == DateGeneration.Rule.OldCDS ||
                rule == DateGeneration.Rule.CDS)
            {
                int m = result.month();
                if (m % 3 != 0) { // not a main IMM nmonth
                    int skip = 3 - m % 3;
                    result += new Period(skip, TimeUnit.Months);
                }
            }
            return result;
        }

       Date previousTwentieth(Date d, DateGeneration.Rule rule) {
            Date result = new Date(20, d.month(), d.year());
            if (result > d)
                result -= new Period(1,TimeUnit.Months);
            if (rule == DateGeneration.Rule.TwentiethIMM ||
                rule == DateGeneration.Rule.OldCDS ||
                rule == DateGeneration.Rule.CDS)
            {
                int m = result.month();
                if (m % 3 != 0) { // not a main IMM nmonth
                    int skip = m%3;
                    result -= new Period(skip,TimeUnit.Months);
                }
            }
            return result;
        }
	}

    //! helper class
    /*! This class provides a more comfortable interface to the argument list of Schedule's constructor. */
    public class MakeSchedule {
        private Calendar calendar_;
        private Date effectiveDate_, terminationDate_;
        private Period tenor_;
        private BusinessDayConvention convention_, terminationDateConvention_;
        private DateGeneration.Rule rule_;
        private bool endOfMonth_;
        private Date firstDate_, nextToLastDate_;

        public MakeSchedule() { rule_ = DateGeneration.Rule.Backward; endOfMonth_ = false; }

        public MakeSchedule from(Date effectiveDate)
        {
           effectiveDate_ = effectiveDate;
           return this;
        }

        public MakeSchedule to(Date terminationDate) 
        {
           terminationDate_ = terminationDate;
           return this;
        }

        public MakeSchedule withTenor(Period tenor) 
        {
           tenor_ = tenor;
           return this;
        }

        public MakeSchedule withFrequency(Frequency frequency) 
        {
           tenor_ = new Period(frequency);
           return this;
        }

        public MakeSchedule withCalendar(Calendar calendar) 
        {
           calendar_ = calendar;
           return this;
        }

        public MakeSchedule withConvention(BusinessDayConvention conv) 
        {
           convention_ = conv;
           return this;
        }

        public MakeSchedule withTerminationDateConvention(BusinessDayConvention conv)
        {
           terminationDateConvention_ = conv;
           return this;
        }

        public MakeSchedule withRule(DateGeneration.Rule r)
        {
           rule_ = r;
           return this;
        }

        public MakeSchedule forwards()
        {
           rule_ = DateGeneration.Rule.Forward;
           return this;
        }

        public MakeSchedule backwards()
        {
           rule_ = DateGeneration.Rule.Backward;
           return this;
        }

        public MakeSchedule endOfMonth(bool flag)
        {
           endOfMonth_ = flag;
           return this;
        }

        public MakeSchedule withFirstDate(Date d)
        {
           firstDate_ = d;
           return this;
        }

        public MakeSchedule withNextToLastDate(Date d)
        {
           nextToLastDate_ = d;
           return this;
        }

        //public MakeSchedule(Date effectiveDate, Date terminationDate, Period tenor, Calendar calendar,
        //                    BusinessDayConvention convention) {
        //    calendar_ = calendar;
        //    effectiveDate_ = effectiveDate;
        //    terminationDate_ = terminationDate;
        //    tenor_ = tenor;
        //    convention_ = convention;
        //    terminationDateConvention_ = convention;
        //    rule_ = DateGeneration.Rule.Backward;
        //    endOfMonth_ = false;
        //    firstDate_ = nextToLastDate_ = null;
        //}


        public Schedule value() 
        {

           // check for mandatory arguments
           if (effectiveDate_ == null)
              throw new ApplicationException("effective date not provided");
           if (terminationDate_ == null)
              throw new ApplicationException("termination date not provided");
           if ((object)tenor_ == null)
              throw new ApplicationException("tenor/frequency not provided");

           // set dynamic defaults:
           BusinessDayConvention convention;
           // if a convention was set, we use it.
           if (convention_ != null )
           {
              convention = convention_;
           }
           else
           {
              if (!calendar_.empty())
              {
                 // ...if we set a calendar, we probably want it to be used;
                 convention = BusinessDayConvention.Following;
              }
              else
              {
                 // if not, we don't care.
                 convention = BusinessDayConvention.Unadjusted;
              }
           }

           BusinessDayConvention terminationDateConvention;
           // if set explicitly, we use it;
           if (terminationDateConvention_ != null )
           {
              terminationDateConvention = terminationDateConvention_;
           }
           else
           {
              // Unadjusted as per ISDA specification
              terminationDateConvention = convention;
           }

           Calendar calendar = calendar_;
           // if no calendar was set...
           if (calendar.empty())
           {
              // ...we use a null one.
              calendar = new NullCalendar();
           }

            return new Schedule(effectiveDate_, terminationDate_, tenor_, calendar_,
                                convention_, terminationDateConvention_,
                                rule_, endOfMonth_, firstDate_, nextToLastDate_);
        }
    }
}