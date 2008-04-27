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
    public interface IMixedScheme {
        void step(Vector a, double t);
        void setStep(double dt);
    }

    //! Mixed (explicit/implicit) scheme for finite difference methods
    /*! In this implementation, the passed operator must be derived
        from either TimeConstantOperator or TimeDependentOperator.
        Also, it must implement at least the following interface:

        \code
        typedef ... array_type;

        // copy constructor/assignment
        // (these will be provided by the compiler if none is defined)
        Operator(const Operator&);
        Operator& operator=(const Operator&);

        // inspectors
        Size size();

        // modifiers
        void setTime(Time t);

        // operator interface
        array_type applyTo(const array_type&);
        array_type solveFor(const array_type&);
        static Operator identity(Size size);

        // operator algebra
        Operator operator*(Real, const Operator&);
        Operator operator+(const Operator&, const Operator&);
        Operator operator+(const Operator&, const Operator&);
        \endcode

        \warning The differential operator must be linear for
                 this evolver to work.

        \todo
        - derive variable theta schemes
        - introduce multi time-level schemes.

        \ingroup findiff
    */
    public class MixedScheme<Operator> : IMixedScheme where Operator : IOperator, new() {
        protected Operator L_, I_, explicitPart_, implicitPart_;
        protected double dt_;
        protected double theta_;
        protected List<BoundaryCondition<Operator>> bcs_;

        // constructors
        public MixedScheme(Operator L, double theta, List<BoundaryCondition<Operator>> bcs) {
            L_ = L;
            I_ = (Operator)(new Operator().identity(L.size()));
            dt_ = 0.0;
            theta_ = theta;
            bcs_ = bcs;
        }

        public void step(Vector a, double t) {
            int i;
            for (i=0; i<bcs_.Count; i++)
                bcs_[i].setTime(t);
            if (theta_!=1.0) { // there is an explicit part
                if (L_.isTimeDependent()) {
                    L_.setTime(t);
                    explicitPart_ = (Operator)new Operator().subtract(I_, new Operator().multiply((1.0 - theta_) * dt_, L_));
                }
                for (i = 0; i < bcs_.Count; i++)
                    bcs_[i].applyBeforeApplying(explicitPart_);
                a = explicitPart_.applyTo(a);
                for (i = 0; i < bcs_.Count; i++)
                    bcs_[i].applyAfterApplying(a);
            }
            if (theta_!=0.0) { // there is an implicit part
                if (L_.isTimeDependent()) {
                    L_.setTime(t-dt_);
                    implicitPart_ = (Operator)new Operator().add(I_, new Operator().multiply(theta_ * dt_, L_));
                }
                for (i = 0; i < bcs_.Count; i++)
                    bcs_[i].applyBeforeSolving(implicitPart_,a);
                a = implicitPart_.solveFor(a);
                for (i = 0; i < bcs_.Count; i++)
                    bcs_[i].applyAfterSolving(a);
            }
        }

        public void setStep(double dt) {
            dt_ = dt;
            if (theta_!=1.0) // there is an explicit part
                explicitPart_ = (Operator)new Operator().subtract(I_, new Operator().multiply((1.0 - theta_) * dt_, L_));
            if (theta_!=0.0) // there is an implicit part
                implicitPart_ = (Operator)new Operator().add(I_, new Operator().multiply(theta_ * dt_, L_));
        }
    }
}
