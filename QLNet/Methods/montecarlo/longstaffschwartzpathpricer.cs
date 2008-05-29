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
    //! Longstaff-Schwarz path pricer for early exercise options
    /*! References:

        Francis Longstaff, Eduardo Schwartz, 2001. Valuing American Options
        by Simulation: A Simple Least-Squares Approach, The Review of
        Financial Studies, Volume 14, No. 1, 113-147

        \ingroup mcarlo

        \test the correctness of the returned value is tested by
              reproducing results available in web/literature
    */
    public abstract class LongstaffSchwartzPathPricer<PathType> : PathPricer<PathType> where PathType : Path {
        protected bool  calibrationPhase_;
        protected EarlyExercisePathPricer<PathType> pathPricer_;

        protected List<Vector> coeff_;
        protected List<double> dF_;

        protected List<PathType> paths_;
        protected List<IValue> v_;

        
        public void calibrate() {
            //const int n = paths_.size();
            //Vector prices = new Vector(n), exercise = new Vector(n);
            //const int len = EarlyExerciseTraits<PathType>::pathLength(paths_[0]);

            //std::transform(paths_.begin(), paths_.end(), prices.begin(),
            //               boost::bind(&EarlyExercisePathPricer<PathType>
            //                             ::operator(),
            //                           pathPricer_.get(), _1, len-1));

            //for (Size i=len-2; i>0; --i) {
            //    std::vector<Real>      y;
            //    std::vector<StateType> x;

            //    //roll back step
            //    for (Size j=0; j<n; ++j) {
            //        exercise[j]=(*pathPricer_)(paths_[j], i);

            //        if (exercise[j]>0.0) {
            //            x.push_back(pathPricer_->state(paths_[j], i));
            //            y.push_back(dF_[i]*prices[j]);
            //        }
            //    }

            //    if (v_.size() <=  x.size()) {
            //        coeff_[i]
            //            = LinearLeastSquaresRegression<StateType>(x, y, v_).a();
            //    }
            //    else {
            //    // if number of itm paths is smaller then the number of
            //    // calibration functions -> no early exercise
            //        coeff_[i] = Array(v_.size(), 0.0);
            //    }

            //    for (Size j=0, k=0; j<n; ++j) {
            //        prices[j]*=dF_[i];
            //        if (exercise[j]>0.0) {
            //            Real continuationValue = 0.0;
            //            for (Size l=0; l<v_.size(); ++l) {
            //                continuationValue += coeff_[i][l] * v_[l](x[k]);
            //            }
            //            if (continuationValue < exercise[j]) {
            //                prices[j] = exercise[j];
            //            }
            //            ++k;
            //        }
            //    }
            //}

            //// remove calibration paths
            //paths_.clear();
            //// entering the calculation phase
            //calibrationPhase_ = false;
        }
    }
}
