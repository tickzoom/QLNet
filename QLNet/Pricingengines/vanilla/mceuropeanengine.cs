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


    //! Monte Carlo European engine factory
    // template <class RNG = PseudoRandom, class S = Statistics>
    public class MakeMCEuropeanEngine<RNG> : MakeMCEuropeanEngine<RNG, Statistics> {
        public MakeMCEuropeanEngine(GeneralizedBlackScholesProcess process) : base(process) { }
    }

    public class MakeMCEuropeanEngine<RNG, S> where S : IGeneralStatistics, new() {
        private GeneralizedBlackScholesProcess process_;
        private bool antithetic_, controlVariate_;
        private int steps_, stepsPerYear_, samples_, maxSamples_;
        private double tolerance_;
        private bool brownianBridge_;
        private ulong seed_;

        public MakeMCEuropeanEngine(GeneralizedBlackScholesProcess process) {
            process_ = process;
        }

        // named parameters
        public MakeMCEuropeanEngine<RNG, S> withSteps(int steps) {
            steps_ = steps;
            return this;
        }
        public MakeMCEuropeanEngine<RNG, S> withStepsPerYear(int steps) {
            stepsPerYear_ = steps;
            return this;
        }
        //public MakeMCEuropeanEngine withBrownianBridge(bool b = true);
        public MakeMCEuropeanEngine<RNG, S> withBrownianBridge(bool brownianBridge) {
            brownianBridge_ = brownianBridge;
            return this;
        }
        public MakeMCEuropeanEngine<RNG, S> withSamples(int samples) {
            if(tolerance_ != 0)
                throw new ApplicationException("tolerance already set");
            samples_ = samples;
            return this;
        }
        public MakeMCEuropeanEngine<RNG, S> withTolerance(double tolerance) {
            if(samples_ != 0)
                throw new ApplicationException("number of samples already set");
            if (PseudoRandom.allowsErrorEstimate == 0)
                throw new ApplicationException("chosen random generator policy does not allow an error estimate");
            tolerance_ = tolerance;
            return this;
        }
        public MakeMCEuropeanEngine<RNG, S> withMaxSamples(int samples) {
            maxSamples_ = samples;
            return this;
        }
        public MakeMCEuropeanEngine<RNG, S> withSeed(ulong seed) {
            seed_ = seed;
            return this;
        }
        //public MakeMCEuropeanEngine withAntitheticVariate(bool b = true)
        public MakeMCEuropeanEngine<RNG, S> withAntitheticVariate(bool b) {
            antithetic_ = b;
            return this;
        }
        //public MakeMCEuropeanEngine withControlVariate(bool b = true)
        public MakeMCEuropeanEngine<RNG, S> withControlVariate(bool b) {
            controlVariate_ = b;
            return this;
        }

        // conversion to pricing engine
        public IPricingEngine value() {
            if (steps_ == 0 && stepsPerYear_ == 0)
                throw new ApplicationException("number of steps not given");
            if (!(steps_ == 0 || stepsPerYear_ == 0))
                throw new ApplicationException("number of steps overspecified");
            return new MCEuropeanEngine<RNG,S>(process_, steps_, stepsPerYear_, brownianBridge_, antithetic_,
                                               controlVariate_, samples_, tolerance_, maxSamples_, seed_);
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
