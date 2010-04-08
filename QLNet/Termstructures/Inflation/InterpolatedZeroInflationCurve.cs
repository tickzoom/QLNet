

using System;
using System.Collections.Generic;

namespace QLNet
{

   /* public abstract class InterpolatedZeroInflationCurve : ZeroInflationTermStructure
    {
        #region InterpolatedCurve
        public List<double> times_ { get; set; }
        public List<double> times() { calculate(); return times_; }
        
        public List<double> data_ { get; set; }
        public List<double> data()
        {
            calculate();
            return data();
        }

        public List<Date> dates_ { get; set; }
        public List<Date> dates()
        {
            calculate();
            List<Date> results = new List<Date>();
            dates_.ForEach((i, x) => results.Add(dates_[i]));
            return results;
        }

        public Interpolation interpolation_ { get; set; }
        public IInterpolationFactory interpolator_
        {
            get;
            set;
        }

        public Dictionary<Date, double> nodes()
        {
            calculate();
            Dictionary<Date, double> results = new Dictionary<Date, double>();
            dates_.ForEach((i, x) => results.Add(x, data_[i]));
            return results;
        }

        public void setupInterpolation()
        {
            interpolation_ = interpolator_.interpolate(times_, times_.Count, data_);
        }

        public object Clone()
        {
            InterpolatedCurve copy = this.MemberwiseClone() as InterpolatedCurve;
            copy.times_ = new List<double>(times_);
            copy.data_ = new List<double>(data_);
            copy.dates_ = new List<Date>(dates_);
            copy.interpolator_ = interpolator_;
            copy.setupInterpolation();
            return copy;
        }
        #endregion

        protected override double zeroRateImpl(double t)
        {
            setupInterpolation();
            return interpolation_.value(t, true);
        }

        public override Date baseDate()
        {
            return dates_[0];
        }

        public override Date maxDate()
        {
            return dates_[dates_.Count - 1];
        }
        
        public InterpolatedZeroInflationCurve(Date referenceDate,
                                            Calendar calendar,
                                            DayCounter dayCounter,
                                            Period lag,
                                            Frequency frequency,
                                            double baseZeroRate,
                                            Handle<YieldTermStructure> yTS)
            : base(referenceDate, calendar, dayCounter, baseZeroRate, lag, frequency, true, yTS) {}

    }*/

    public class InterpolatedZeroInflationCurve<Interpolator> : ZeroInflationTermStructure
        where Interpolator : IInterpolationFactory
    {

        public InterpolatedZeroInflationCurve(Date referenceDate,
                                       Calendar calendar,
                                       DayCounter dayCounter,
                                       Period lag,
                                       Frequency frequency,
                                       bool indexIsInterpolated,
                                       Handle<YieldTermStructure> yTS,
                                       List<Date> dates,
                                       List<double> rates,
                                       Interpolator interpolator)
            : base(referenceDate, calendar, dayCounter, rates[0],
                                 lag, frequency, indexIsInterpolated, yTS) {
            dates_ = dates;
            data_ = rates;
            interpolator_ = interpolator;
            if (!(dates_.Count > 1))
                throw new ApplicationException("too few dates: "+dates_.Count);      

            // check that the data starts from the beginning,
            // i.e. referenceDate - lag, at least must be in the relevant
            // period
            KeyValuePair<Date, Date> lim =
                Utils.inflationPeriod(yTS.link.referenceDate() - observationLag(), frequency);
            if (!(lim.Key <= dates_[0] && dates_[0] <= lim.Value))
                throw new ApplicationException("first data date is not in base period, date: " 
                                            + (dates_[0])+ " not within [" + lim.Key + "," + lim.Value + "]");

            // by convention, if the index is not interpolated we pull all the dates
            // back to the start of their inflationPeriods
            // otherwise the time calculations will be inconsistent
            if (!indexIsInterpolated_)
            {
                for (int i = 0; i < dates_.Count; i++)
                {
                    dates_[i] = Utils.inflationPeriod(dates_[i], frequency).Key;
                }
            }


            if(!(data_.Count == dates_.Count))
                throw new ApplicationException("indices/dates count mismatch: "
                                                + data_.Count + " vs " + dates_.Count);
            //times_.resize(dates_.Count);
            times_.Capacity = dates_.Count;
            times_[0] = timeFromReference(dates_[0]);
            for (int i = 1; i < dates_.Count; i++)
            {
                if(!(dates_[i] > dates_[i-1]))
                    throw new ApplicationException("dates not sorted");
                
                // but must be greater than -1
                if(!(data_[i] > -1.0))
                    throw new ApplicationException("zero inflation data < -100 %");

                // this can be negative
                times_[i] = timeFromReference(dates_[i]);
                if(Utils.close(times_[i],times_[i-1]))
                    throw new ApplicationException("two dates correspond to the same time "
                           +"under this curve's day count convention");
                 
            }
            interpolation_ = interpolator_.interpolate(times_,times_.Count,data_);
            interpolation_.update();
        }

        public InterpolatedZeroInflationCurve(Date referenceDate,
                                       Calendar calendar,
                                       DayCounter dayCounter,
                                       Period lag,
                                       Frequency frequency,
                                       bool indexIsInterpolated,
                                       double baseZeroRate,
                                       Handle<YieldTermStructure> yTS,
                                       Interpolator interpolator)
            : base(referenceDate, calendar, dayCounter, baseZeroRate, lag, frequency, indexIsInterpolated, yTS)
        {
           interpolator_ = interpolator;
        }

        #region InterpolatedCurve
        public List<double> times_ { get; set; }
        public virtual List<double> times() { calculate(); return times_; }
        
        public List<double> data_ { get; set; }
        public virtual List<double> data()
        {
            calculate();
            return data_;
        }

        public List<Date> dates_ { get; set; }
        public List<Date> dates()
        {
            calculate();
            return dates_;
        }

        public Interpolation interpolation_ { get; set; }
        public IInterpolationFactory interpolator_{get;set;}

        public Dictionary<Date, double> nodes()
        {
            calculate();
            Dictionary<Date, double> results = new Dictionary<Date, double>();
            dates_.ForEach((i, x) => results.Add(x, data_[i]));
            return results;
        }

        public void setupInterpolation()
        {
            
            interpolation_ = interpolator_.interpolate(times_, times_.Count, data_);
        }

        public object Clone()
        {
            InterpolatedCurve copy = this.MemberwiseClone() as InterpolatedCurve;
            copy.times_ = new List<double>(times_);
            copy.data_ = new List<double>(data_);
            copy.dates_ = new List<Date>(dates_);
            copy.interpolator_ = interpolator_;
            copy.setupInterpolation();
            return copy;
        }
        #endregion

        protected override double zeroRateImpl(double t)
        {
            setupInterpolation();
            return interpolation_.value(t, true);
        }

        public override Date baseDate(){return dates_[0];}

        public override Date maxDate(){
            Date d;
            if (indexIsInterpolated_)
            {
                d = dates_[dates_.Count - 1];
            }
            else
            {
                d = Utils.inflationPeriod(dates_[dates_.Count - 1], frequency()).Value;
            }
            return d;
            //return dates_[dates_.Count - 1];
        }
        
    }
}
