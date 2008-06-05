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
    //! Localised-term-structure bootstrapper for most curve types.
    /*! This algorithm enables a localised fitting for non-local
        interpolation methods.

        As in the similar class (IterativeBootstrap) the input term
        structure is solved on a number of market instruments which
        are passed as a vector of handles to BootstrapHelper
        instances. Their maturities mark the boundaries of the
        interpolated segments.

        Unlike the IterativeBootstrap class, the solution for each
        interpolated segment is derived using a local
        approximation. This restricts the risk profile s.t.  the risk
        is localised. Therefore, we obtain a local IR risk profile
        whilst using a smoother interpolation method. Particularly
        good for the convex-monotone spline method.
    */
    public class LocalBootstrap : IBootStrap {
        //typedef typename Curve::traits_type Traits;
        //typedef typename Curve::interpolator_type Interpolator;
      
        private bool validCurve_;
        private object tsContainer_; // yes, it is a workaround
        int localisation_;
        bool forcePositive_;
        
        //public LocalBootstrap(Size localisation = 2, bool forcePositive = true) {
        public LocalBootstrap() : this(2, true) { }
        public LocalBootstrap(int localisation, bool forcePositive) {
            localisation_ = localisation;
            forcePositive_ = forcePositive;
        }

        public void setup<T, I, B>(PiecewiseYieldCurve<T, I, B> ts)
            where T : ITraits, new()
            where I : IInterpolationFactory, new()
            where B : IBootStrap, new() {
            
            tsContainer_ = ts;
            PiecewiseYieldCurve<T, I, B> ts_ = tsContainer_ as PiecewiseYieldCurve<T, I, B>;

            int n = ts_.instruments_.Count;
            if (!(n >= ts_.interpolator_.requiredPoints))
                throw new ArgumentException("not enough instruments: " + n + " provided, " +
                       (ts_.interpolator_.requiredPoints) + " required");

            if (!(n > localisation_))
                throw new ApplicationException("not enough instruments: " + n + " provided, " + localisation_ + " required.");

            ts_.instruments_.ForEach(i => i.registerWith(ts_.update));
        }

        public void calculate<T, I, B>()
            where T : ITraits, new()
            where I : IInterpolationFactory, new()
            where B : IBootStrap, new() {

            PiecewiseYieldCurve<T, I, B> ts_ = tsContainer_ as PiecewiseYieldCurve<T, I, B>;

            validCurve_ = false;
            int n = ts_.instruments_.Count;

            // ensure rate helpers are sorted
            ts_.instruments_.Sort((x, y) => x.latestDate().CompareTo(y.latestDate()));

            // check that there is no instruments with the same maturity
            for (int i = 1; i < n; ++i) {
                Date m1 = ts_.instruments_[i - 1].latestDate(),
                     m2 = ts_.instruments_[i].latestDate();
                if (m1 == m2) throw new ArgumentException("two instruments have the same maturity (" + m1 + ")");
            }

            // check that there is no instruments with invalid quote
            for (int i = 0; i < n; ++i)
                if (!ts_.instruments_[i].quoteIsValid())
                    throw new ArgumentException("instrument " + i + " (maturity: " + ts_.instruments_[i].latestDate() +
                           ") has an invalid quote");

            // setup instruments and register with them
            for (int i = 0; i < n; ++i) {
                // There is a significant interaction with observability.
                ts_.instruments_[i].setTermStructure(ts_);
            }

            // calculate dates and times
            ts_.dates_ = new InitializedList<Date>(n + 1);
            ts_.times_ = new InitializedList<double>(n + 1);
            ts_.dates_[0] = ts_.initialDate(ts_);
            ts_.times_[0] = ts_.timeFromReference(ts_.dates_[0]);
            for (int i = 0; i < n; ++i) {
                ts_.dates_[i + 1] = ts_.instruments_[i].latestDate();
                ts_.times_[i + 1] = ts_.timeFromReference(ts_.dates_[i + 1]);
                if (!validCurve_)
                    ts_.data_[i+1] = ts_.data_[i];
            }

            // set initial guess only if the current curve cannot be used as guess
            if (validCurve_) {
                if (ts_.data_.Count != n + 1)
                    throw new ArgumentException("dimension mismatch: expected " + n + 1 + ", actual " + ts_.data_.Count);
            } else {
                ts_.data_ = new InitializedList<double>(n + 1);
                ts_.data_[0] = ts_.initialValue(ts_);
            }

            throw new NotImplementedException();

            LevenbergMarquardt solver = new LevenbergMarquardt(ts_.accuracy_, ts_.accuracy_, ts_.accuracy_);
            EndCriteria endCriteria = new EndCriteria(100, 10, 0.00, ts_.accuracy_, 0.00);
            //PositiveConstraint posConstraint;
            //NoConstraint noConstraint;
            //Constraint& solverConstraint = forcePositive_ ?
            //    static_cast<Constraint&>(posConstraint) :
            //    static_cast<Constraint&>(noConstraint);

            //// now start the bootstrapping.
            //Size iInst = localisation_-1;

            //Size dataAdjust = Curve::interpolator_type::dataSizeAdjustment;

            //do {
            //    Size initialDataPt = iInst+1-localisation_+dataAdjust;
            //    Array startArray(localisation_+1-dataAdjust);
            //    for (Size j = 0; j < startArray.size()-1; ++j)
            //        startArray[j] = ts_->data_[initialDataPt+j];

            //    // here we are extending the interpolation a point at a
            //    // time... but the local interpolator can make an
            //    // approximation for the final localisation period.
            //    // e.g. if the localisation is 2, then the first section
            //    // of the curve will be solved using the first 2
            //    // instruments... with the local interpolator making
            //    // suitable boundary conditions.
            //    ts_->interpolation_ =
            //        ts_->interpolator_.localInterpolate(
            //                                      ts_->times_.begin(),
            //                                      ts_->times_.begin()+(iInst + 2),
            //                                      ts_->data_.begin(),
            //                                      localisation_,
            //                                      ts_->interpolation_,
            //                                      nInsts+1);

            //    if (iInst >= localisation_) {
            //        startArray[localisation_-dataAdjust] =
            //            Traits::guess(ts_, ts_->dates_[iInst]);
            //    } else {
            //        startArray[localisation_-dataAdjust] = ts_->data_[0];
            //    }

            //    PenaltyFunction<Curve> currentCost(
            //                ts_,
            //                initialDataPt,
            //                ts_->instruments_.begin() + (iInst - localisation_+1),
            //                ts_->instruments_.begin() + (iInst+1));

            //    Problem toSolve(currentCost, solverConstraint, startArray);

            //    EndCriteria::Type endType = solver.minimize(toSolve, endCriteria);

            //    // check the end criteria
            //    QL_REQUIRE(endType == EndCriteria::StationaryFunctionAccuracy ||
            //               endType == EndCriteria::StationaryFunctionValue,
            //               "Unable to strip yieldcurve to required accuracy " );
            //    ++iInst;
            //} while ( iInst < nInsts );
            //validCurve_ = true;
        }
    }
}
