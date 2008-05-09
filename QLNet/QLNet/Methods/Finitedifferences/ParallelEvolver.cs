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
    //! Parallel evolver for multiple arrays
    /*! \ingroup findiff */

    public class StepConditionSet<array_type> : List<IStepCondition<array_type>>, IStepCondition<array_type>
        where array_type : Vector {
        public void applyTo(object o, double t) {
            List<IStepCondition<array_type>> a = o as List<IStepCondition<array_type>>;
            for (int i=0; i < Count; i++) {
                this[i].applyTo(a[i], t);
            }
        }
    }

    public class BoundaryConditionSet : List<List<BoundaryCondition<IOperator>>> { }

    public class ParallelEvolver<Evolver> : IMixedScheme, ISchemeFactory where Evolver : IMixedScheme, ISchemeFactory, new() {
        private List<IMixedScheme> evolvers_;

        // required for generics
        public ParallelEvolver() { }
        public ParallelEvolver(List<IOperator> L, BoundaryConditionSet bcs) {
            evolvers_ = new List<IMixedScheme>(L.Count);
            for (int i = 0; i < L.Count; i++) {
                evolvers_.Add(new Evolver().factory(L[i], bcs[i]));
            }
        }

        public void step(ref object o, double t) {
            List<Vector> a = o as List<Vector>;
            for (int i=0; i < evolvers_.Count; i++) {
                object temp = a[i];
                evolvers_[i].step(ref temp, t);
                a[i] = temp as Vector;
            }
        }

        public void setStep(double dt) {
            for (int i = 0; i < evolvers_.Count; i++) {
                evolvers_[i].setStep(dt);
            }
        }

        public IMixedScheme factory(object L, object bcs) {
            return new ParallelEvolver<Evolver>((List<IOperator>)L, (BoundaryConditionSet)bcs);
        }
    }
}
