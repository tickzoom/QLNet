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
    //! European option pricing engine using Monte Carlo simulation
    /*! \ingroup vanillaengines

        \test the correctness of the returned value is tested by
              checking it against analytic results.
    */
    public class MCEuropeanEngine<RNG, S> : MCVanillaEngine<SingleVariate, RNG, S> 
        where S : IGeneralStatistics, new() {

        // constructor
        public MCEuropeanEngine(GeneralizedBlackScholesProcess process, int timeSteps, int timeStepsPerYear,
                                bool brownianBridge, bool antitheticVariate, bool controlVariate,
                                int requiredSamples, double requiredTolerance, int maxSamples, ulong seed)
            : base(process, timeSteps, timeStepsPerYear, brownianBridge, antitheticVariate, controlVariate,
                   requiredSamples, requiredTolerance, maxSamples, seed) { }

        protected override PathPricer<Path> pathPricer() {
            PlainVanillaPayoff payoff = arguments_.payoff as PlainVanillaPayoff;
            if (payoff == null)
                throw new ApplicationException("non-plain payoff given");

            GeneralizedBlackScholesProcess process = process_ as GeneralizedBlackScholesProcess;
            if (process == null)
                throw new ApplicationException("Black-Scholes process required");

            return new EuropeanPathPricer(payoff.optionType(), payoff.strike(),
                                          process.riskFreeRate().link.discount(timeGrid().Last()));
        }
    }

    public class EuropeanPathPricer : PathPricer<Path> {
        private PlainVanillaPayoff payoff_;
        private double discount_;

        public EuropeanPathPricer(Option.Type type, double strike, double discount) {
            payoff_ = new PlainVanillaPayoff(type, strike);
            discount_ = discount;
            if (!(strike>=0.0))
                throw new ApplicationException("strike less than zero not allowed");
        }

        public override double value(Path path) {
            if (!(path.length() > 0))
                throw new ApplicationException("the path cannot be empty");
            return payoff_.value(path.back()) * discount_;
        }
    }
}
