﻿/*
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
    //! Pricing engine for vanilla options using Monte Carlo simulation
    /*! \ingroup vanillaengines */
    public abstract class MCVanillaEngine<MC, RNG, S> : MCVanillaEngine<MC, RNG, S, VanillaOption>
            where RNG : IRSG, new()
            where S : IGeneralStatistics, new() {
        protected MCVanillaEngine(StochasticProcess process, int timeSteps, int timeStepsPerYear, bool brownianBridge,
                                  bool antitheticVariate, bool controlVariate, int requiredSamples, double requiredTolerance,
                                  int maxSamples, ulong seed)
            : base(process, timeSteps, timeStepsPerYear, brownianBridge, antitheticVariate, controlVariate, requiredSamples,
                   requiredTolerance, maxSamples, seed) { }
    }

    public abstract class MCVanillaEngine<MC, RNG, S, Inst> : McSimulation<MC, RNG, S>, IGenericEngine
            where RNG : IRSG, new() where S : IGeneralStatistics, new() {
        //typedef typename McSimulation<MC,RNG,S>::path_generator_type path_generator_type;
        //typedef typename McSimulation<MC,RNG,S>::path_pricer_type path_pricer_type;
        //typedef typename McSimulation<MC,RNG,S>::stats_type stats_type;
        //typedef typename McSimulation<MC,RNG,S>::result_type result_type;

        protected StochasticProcess process_;
        protected int timeSteps_, timeStepsPerYear_;
        protected int requiredSamples_, maxSamples_;
        protected double requiredTolerance_;
        protected bool brownianBridge_;
        protected ulong seed_;


        protected MCVanillaEngine(StochasticProcess process, int timeSteps, int timeStepsPerYear, bool brownianBridge,
                                  bool antitheticVariate, bool controlVariate, int requiredSamples, double requiredTolerance,
                                  int maxSamples, ulong seed) : base(antitheticVariate, controlVariate) {
            process_ = process;
            timeSteps_ = timeSteps;
            timeStepsPerYear_ = timeStepsPerYear;
            requiredSamples_ = requiredSamples;
            maxSamples_ = maxSamples;
            requiredTolerance_ = requiredTolerance;
            brownianBridge_ = brownianBridge;
            seed_ = seed;

            if (!(timeSteps != 0 || timeStepsPerYear != 0))
                throw new ApplicationException("no time steps provided");
            if (!(timeSteps == 0 || timeStepsPerYear == 0))
                throw new ApplicationException("both time steps and time steps per year were provided");
            if (!(timeSteps != 0))
                throw new ApplicationException("timeSteps must be positive, " + timeSteps + " not allowed");
            //if (!(timeStepsPerYear != 0))
            //    throw new ApplicationException("timeStepsPerYear must be positive, " + timeStepsPerYear + " not allowed");

            process_.registerWith(update);
        }


        public void calculate() {
            base.calculate(requiredTolerance_, requiredSamples_, maxSamples_);
            results_.value = mcModel_.sampleAccumulator().mean();
            if (new RNG().allowsErrorEstimate != 0)
                results_.errorEstimate = mcModel_.sampleAccumulator().errorEstimate();
        }

        // McSimulation implementation
        protected override TimeGrid timeGrid() {
            Date lastExerciseDate = arguments_.exercise.lastDate();
            double t = process_.time(lastExerciseDate);
            if (timeSteps_ != 0) {
                return new TimeGrid(t, timeSteps_);
            } else if (timeStepsPerYear_ != 0) {
                int steps = (int)(timeStepsPerYear_*t);
                return new TimeGrid(t, Math.Max(steps, 1));
            } else {
                throw new ApplicationException("time steps not specified");
            }
        }

        protected override PathGenerator<IRNG> pathGenerator() {
            int dimensions = process_.factors();
            TimeGrid grid = timeGrid();
            IRNG generator = (IRNG)new RNG().make_sequence_generator(dimensions * (grid.size() - 1), seed_);
            return new PathGenerator<IRNG>(process_, grid, generator, brownianBridge_);
        }

        protected override double controlVariateValue() {
            IPricingEngine controlPE = controlPricingEngine();
            if (controlPE == null)
                throw new ApplicationException("engine does not provide control variation pricing engine");

            OneAssetOption.Arguments controlArguments = controlPE.getArguments() as VanillaOption.Arguments;
            if (controlArguments == null)
                throw new ApplicationException("engine is using inconsistent arguments");

            controlArguments = arguments_;
            controlPE.calculate();

            OneAssetOption.Results controlResults = controlPE.getResults() as VanillaOption.Results;
            if (controlResults == null)
                throw new ApplicationException("engine returns an inconsistent result type");

            return controlResults.value.GetValueOrDefault();
        }

        #region PricingEngine
        protected OneAssetOption.Arguments arguments_ = new OneAssetOption.Arguments();
        protected OneAssetOption.Results results_ = new OneAssetOption.Results();

        public IPricingEngineArguments getArguments() { return arguments_; }
        public IPricingEngineResults getResults() { return results_; }
        public void reset() { results_.reset(); }

        public void update() { notifyObservers(); }

    	#endregion
    }
}
