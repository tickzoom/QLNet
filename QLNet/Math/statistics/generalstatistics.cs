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
using System.Linq;
using System.Text;

namespace QLNet {
    public interface IGeneralStatistics {
        int samples();
        double mean();
        double standardDeviation();
        double percentile(double percent);

        KeyValuePair<double, int> expectationValue(Func<KeyValuePair<double, double>, double> f,
                                           Func<KeyValuePair<double, double>, bool> inRange);
    }

    //! Statistics tool
    /*! This class accumulates a set of data and returns their
        statistics (e.g: mean, variance, skewness, kurtosis,
        error estimation, percentile, etc.) based on the empirical
        distribution (no gaussian assumption)

        It doesn't suffer the numerical instability problem of
        IncrementalStatistics. The downside is that it stores all
        samples, thus increasing the memory requirements.
    */
    public class GeneralStatistics : IGeneralStatistics {
        private List<KeyValuePair<double,double>> samples_;
        //! number of samples collected
        public int samples() { return samples_.Count; }
        //! collected data
        public List<KeyValuePair<double,double>> data() { return samples_; }

        private bool sorted_;


        public GeneralStatistics() { reset(); }


        /*! returns the error estimate on the mean value, defined as
            \f$ \epsilon = \sigma/\sqrt{N}. \f$ */
        public double errorEstimate() { return Math.Sqrt(variance()/samples()); }

        /*! returns the minimum sample value */
        public double min() {
            if (!(samples() > 0)) throw new ApplicationException("empty sample set");
            return samples_.Min<KeyValuePair<double,double>>(x => x.Key);
        }

        /*! returns the maximum sample value */
        public double max() {
            if (!(samples() > 0)) throw new ApplicationException("empty sample set");
            return samples_.Max<KeyValuePair<double, double>>(x => x.Key);
        }
        

        //! adds a datum to the set, possibly with a weight
        //public void add(double value, double weight = 1.0) {
        public void add(double value, double weight) {
            if (!(weight>=0.0)) throw new ApplicationException("negative weight not allowed");
            samples_.Add(new KeyValuePair<double,double>(value,weight));
            sorted_ = false;
        }

        //! resets the data to a null set
        public void reset(){
            samples_ = new List<KeyValuePair<double,double>>();
            sorted_ = true;
        }

        //! sort the data set in increasing order
        public void sort()  {
            if (!sorted_) {
                samples_.Sort();
                sorted_ = true;
            }
        }


        //! sum of data weights
        public double weightSum() {
            double result = 0.0;
            result = samples_.Sum<KeyValuePair<double, double>>(x => x.Value);
            return result;
        }

        /*! returns the mean, defined as
            \f[ \langle x \rangle = \frac{\sum w_i x_i}{\sum w_i}. \f] */
        public double mean() {
            int N = samples();
            if (!(samples() > 0)) throw new ApplicationException("empty sample set");
            // eat our own dog food
            return expectationValue(x => x.Key * x.Value, x => true).Key;
        }

        /*! returns the standard deviation \f$ \sigma \f$, defined as the
        square root of the variance. */
        public double standardDeviation() { return Math.Sqrt(variance()); }

        /*! returns the variance, defined as
            \f[ \sigma^2 = \frac{N}{N-1} \left\langle \left(
                x-\langle x \rangle \right)^2 \right\rangle. \f] */
        public double variance()  {
            int N = samples();
            if (!(N > 1)) throw new ApplicationException("sample number <=1, unsufficient");
            // Subtract the mean and square. Repeat on the whole range.
            // Hopefully, the whole thing will be inlined in a single loop.
            double s2 = expectationValue(x => Math.Pow(x.Key * x.Value - mean(), 2), x => true).Key;

                //compose(square<Real>(), std::bind2nd(std::minus<Real>(), mean())), () => true).Key;
            return s2*N/(N-1.0);
        }

        /*! returns the skewness, defined as
            \f[ \frac{N^2}{(N-1)(N-2)} \frac{\left\langle \left(
                x-\langle x \rangle \right)^3 \right\rangle}{\sigma^3}. \f]
            The above evaluates to 0 for a Gaussian distribution.
        */
        public double skewness() {
            int N = samples();
            if (!(N > 2)) throw new ApplicationException("sample number <=2, unsufficient");

            double x = expectationValue(y => Math.Pow(y.Key * y.Value - mean(), 3), y => true).Key;
            double sigma = standardDeviation();

            return (x/(sigma*sigma*sigma))*(N/(N-1.0))*(N/(N-2.0));
        }

        /*! returns the excess kurtosis, defined as
            \f[ \frac{N^2(N+1)}{(N-1)(N-2)(N-3)}
                \frac{\left\langle \left(x-\langle x \rangle \right)^4
                \right\rangle}{\sigma^4} - \frac{3(N-1)^2}{(N-2)(N-3)}. \f]
            The above evaluates to 0 for a Gaussian distribution.
        */
        public double kurtosis() {
            int N = samples();
            if (!(N > 3)) throw new ApplicationException("sample number <=3, unsufficient");

            double x = expectationValue(y => Math.Pow(y.Key * y.Value - mean(), 4), y => true).Key;
            double sigma2 = variance();

            double c1 = (N/(N-1.0)) * (N/(N-2.0)) * ((N+1.0)/(N-3.0));
            double c2 = 3.0 * ((N-1.0)/(N-2.0)) * ((N-1.0)/(N-3.0));

            return c1*(x/(sigma2*sigma2))-c2;
        }

        /*! Expectation value of a function \f$ f \f$ on a given range \f$ \mathcal{R} \f$, i.e.,
            \f[ \mathrm{E}\left[f \;|\; \mathcal{R}\right] =
                \frac{\sum_{x_i \in \mathcal{R}} f(x_i) w_i}{
                      \sum_{x_i \in \mathcal{R}} w_i}. \f]
            The range is passed as a boolean function returning
            <tt>true</tt> if the argument belongs to the range
            or <tt>false</tt> otherwise.

            The function returns a pair made of the result and the number of observations in the given range. */
        public KeyValuePair<double, int> expectationValue(Func<KeyValuePair<double, double>, double> f,
                                                          Func<KeyValuePair<double, double>, bool> inRange) {
            double num = 0.0, den = 0.0;
            int N = 0;

            foreach(KeyValuePair<double,double> x in samples_) {
                if (inRange(x)) {
                    num += f(x)*x.Value;
                    den += x.Value;
                    N += 1;                
                }
            }

            if (N == 0) return new KeyValuePair<double,int>(0,0);
            else return new KeyValuePair<double,int>(num/den,N);
        }

        /*! \f$ y \f$-th percentile, defined as the value \f$ \bar{x} \f$
            such that
            \f[ y = \frac{\sum_{x_i < \bar{x}} w_i}{
                          \sum_i w_i} \f]

            \pre \f$ y \f$ must be in the range \f$ (0-1]. \f$
        */
        public double percentile(double percent) {

            if (!(percent > 0.0 && percent <= 1.0))
                throw new ApplicationException("percentile (" + percent + ") must be in (0.0, 1.0]");

            double sampleWeight = weightSum();
            if (!(sampleWeight > 0)) throw new ApplicationException("empty sample set");

            sort();

            double integral = 0, target = percent*sampleWeight;
            int pos = samples_.Count<KeyValuePair<double, double>>(x => { integral += x.Value; return integral < target; } );
            return samples_[pos].Key;
        }

        /*! \f$ y \f$-th top percentile, defined as the value
            \f$ \bar{x} \f$ such that
            \f[ y = \frac{\sum_{x_i > \bar{x}} w_i}{
                          \sum_i w_i} \f]

            \pre \f$ y \f$ must be in the range \f$ (0-1]. \f$
        */
        public double topPercentile(double percent) {
            if (!(percent > 0.0 && percent <= 1.0))
                throw new ApplicationException("percentile (" + percent + ") must be in (0.0, 1.0]");

            double sampleWeight = weightSum();
            if (!(sampleWeight > 0)) throw new ApplicationException("empty sample set");

            sort();

            double integral = 0, target = 1 - percent*sampleWeight;
            int pos = samples_.Count<KeyValuePair<double, double>>(x => { integral += x.Value; return integral < target; } );
            return samples_[pos].Key;
        }

        ////! adds a sequence of data to the set, with default weight
        //template <class DataIterator>
        //void addSequence(DataIterator begin, DataIterator end) {
        //    for (;begin!=end;++begin)
        //        add(*begin);
        //}
        ////! adds a sequence of data to the set, each with its weight
        //template <class DataIterator, class WeightIterator>
        //void addSequence(DataIterator begin, DataIterator end,
        //                 WeightIterator wbegin) {
        //    for (;begin!=end;++begin,++wbegin)
        //        add(*begin, *wbegin);
        //}
    }
}
