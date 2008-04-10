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
    public class LogInterpolationImpl<Interpolator> : Interpolation.templateImpl
        where Interpolator : IInterpolationFactory, new() {

        private List<double> logY_;
        private Interpolation interpolation_;

        public LogInterpolationImpl(List<double> xBegin, int size, List<double> yBegin)
           : this(xBegin, size, yBegin, new Interpolator()) { }
        public LogInterpolationImpl(List<double> xBegin, int size, List<double> yBegin, IInterpolationFactory factory)
            : base(xBegin, size, yBegin) {
            logY_ = new Array<double>(size_);
            interpolation_ = factory.interpolate(xBegin_, size, logY_);
        }

        public override void update() {
            for (int i = 0; i < size_ - 1; ++i) {
                if (!(yBegin_[i] > 0.0))
                    throw new ArgumentException("invalid value (" + yBegin_[i] + ") at index " + i);
                logY_[i] = System.Math.Log(yBegin_[i]);
            }
            interpolation_.update();
        }

        public override double value(double x) {
            return System.Math.Exp(interpolation_.value(x, true));
        }
        public override double primitive(double x) {
            throw new NotImplementedException("LogInterpolation primitive not implemented");
        }
        public override double derivative(double x) {
            return value(x) * interpolation_.derivative(x, true);
        }
        public override double secondDerivative(double x) {
            return derivative(x) * interpolation_.derivative(x, true) +
                        value(x) * interpolation_.secondDerivative(x, true);
        }
    }

    //! log-linear interpolation factory and traits
    public class LogLinear : IInterpolationFactory {
        public Interpolation interpolate(List<double> xBegin, int size, List<double> yBegin) {
            return new LogLinearInterpolation(xBegin, size, yBegin);
        }
        public bool global { get { return false; } }
        public int requiredPoints { get { return 2; } }
    };

    //! %log-linear interpolation between discrete points
    public class LogLinearInterpolation : Interpolation {
        /*! \pre the \f$ x \f$ values must be sorted. */
        public LogLinearInterpolation(List<double> xBegin, int size, List<double> yBegin) {
            impl_ = new LogInterpolationImpl<Linear>(xBegin, size, yBegin);
            impl_.update();
        }
    };
}
