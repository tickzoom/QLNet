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
    //! Finite-differences pricing engine for BSM one asset options
    /*! The name is a misnomer as this is a base class for any finite
        difference scheme.  Its main job is to handle grid layout.

        \ingroup vanillaengines
    */
    public class FDVanillaEngine : IOptionPricingEngine {
        protected GeneralizedBlackScholesProcess process_;
        protected int timeSteps_, gridPoints_;
        protected bool timeDependent_;
        protected double requiredGridValue_;
        protected Date exerciseDate_;
        protected Payoff payoff_;
        protected TridiagonalOperator finiteDifferenceOperator_;
        protected SampledCurve intrinsicValues_;

        // typedef BoundaryCondition<TridiagonalOperator> bc_type;
        protected List<BoundaryCondition<IOperator>> BCs_;
        // temporaries
        protected double sMin_, center_, sMax_;

        // temporaries
        private double gridLogSpacing_;
        const double safetyZoneFactor_ = 1.1;

        // required for generics
        public FDVanillaEngine() { }
        public virtual IOptionPricingEngine factory(GeneralizedBlackScholesProcess process,
                                                    int timeSteps, int gridPoints, bool timeDependent) {
            return new FDVanillaEngine(process, timeSteps, gridPoints, timeDependent);
        }

        //public FDVanillaEngine(GeneralizedBlackScholesProcess process, int timeSteps, int gridPoints,
        //                       bool timeDependent = false)
        public FDVanillaEngine(GeneralizedBlackScholesProcess process, int timeSteps, int gridPoints, bool timeDependent) {
            process_ = process;
            timeSteps_ = timeSteps;
            gridPoints_ = gridPoints;
            timeDependent_ = timeDependent;
            intrinsicValues_ = new SampledCurve(gridPoints);
            BCs_ = new InitializedList<BoundaryCondition<IOperator>>(2);
        }


        public Vector grid() { return intrinsicValues_.grid(); }

        protected virtual void setGridLimits() {
            setGridLimits(process_.stateVariable().link.value(), getResidualTime());
            ensureStrikeInGrid();
        }

        protected void setGridLimits(double center, double t) {
            if (!(center > 0.0)) throw new ApplicationException("negative or null underlying given");
            center_ = center;
            int newGridPoints = safeGridPoints(gridPoints_, t);
            if (newGridPoints > intrinsicValues_.size()) {
                intrinsicValues_ = new SampledCurve(newGridPoints);
            }

            double volSqrtTime = Math.Sqrt(process_.blackVolatility().link.blackVariance(t, center_));

            // the prefactor fine tunes performance at small volatilities
            double prefactor = 1.0 + 0.02/volSqrtTime;
            double minMaxFactor = Math.Exp(4.0 * prefactor * volSqrtTime);
            sMin_ = center_/minMaxFactor;  // underlying grid min value
            sMax_ = center_*minMaxFactor;  // underlying grid max value
        }

        public void ensureStrikeInGrid() {
            // ensure strike is included in the grid
            StrikedTypePayoff striked_payoff = payoff_ as StrikedTypePayoff;
            if (striked_payoff == null) return;
            
            double requiredGridValue = striked_payoff.strike();

            if(sMin_ > requiredGridValue/safetyZoneFactor_){
                sMin_ = requiredGridValue/safetyZoneFactor_;
                // enforce central placement of the underlying
                sMax_ = center_/(sMin_/center_);
            }
            if(sMax_ < requiredGridValue*safetyZoneFactor_){
                sMax_ = requiredGridValue*safetyZoneFactor_;
                // enforce central placement of the underlying
                sMin_ = center_/(sMax_/center_);
            }
        }

        protected void initializeInitialCondition() {
            intrinsicValues_.setLogGrid(sMin_, sMax_);
            intrinsicValues_.sample(payoff_.value);
        }

        protected void initializeOperator() {
            finiteDifferenceOperator_ = OperatorFactory.getOperator(process_, intrinsicValues_.grid(),
                                             getResidualTime(), timeDependent_);
        }

        protected void initializeBoundaryConditions() {
            BCs_[0] = new NeumannBC(intrinsicValues_.value(1) - intrinsicValues_.value(0), NeumannBC.Side.Lower);
            BCs_[1] = new NeumannBC(intrinsicValues_.value(intrinsicValues_.size() - 1) -
                                    intrinsicValues_.value(intrinsicValues_.size()-2),
                                    NeumannBC.Side.Upper);
        }

        protected double getResidualTime() {
            return process_.time(exerciseDate_);
        }

        // safety check to be sure we have enough grid points.
        private int safeGridPoints(int gridPoints, double residualTime) {
            const int minGridPoints = 10;
            const int minGridPointsPerYear = 2;
            return Math.Max(gridPoints,
                            residualTime > 1 ?
                                (int)(minGridPoints + (residualTime-1.0) * minGridPointsPerYear)
                                : minGridPoints);
        }

        #region IOptionPricingEngine
        public virtual void setupArguments(IPricingEngineArguments a) {
            OneAssetOption.Arguments args = a as OneAssetOption.Arguments;
            if (args == null) throw new ApplicationException("incorrect argument type");

            exerciseDate_ = args.exercise.lastDate();
            payoff_ = args.payoff;
            requiredGridValue_ = ((StrikedTypePayoff)payoff_).strike();
        }
        public virtual void calculate(IPricingEngineResults r) { throw new NotSupportedException(); }
        #endregion
    }


    public interface IOptionPricingEngine {
        void calculate(IPricingEngineResults r);
        void setupArguments(IPricingEngineArguments a);
        IOptionPricingEngine factory(GeneralizedBlackScholesProcess process, int timeSteps, int gridPoints, bool timeDependent);
    }

    public class FDEngineAdapter<Base, Engine, ArgumentsType, ResultsType> : FDVanillaEngine
        where Base : IOptionPricingEngine, new()
        where Engine : IGenericEngine<ArgumentsType, ResultsType>
        where ArgumentsType : IPricingEngineArguments, new()
        where ResultsType : IPricingEngineResults, new() {

        Base optionBase;

        //public FDEngineAdapter(GeneralizedBlackScholesProcess process, Size timeSteps=100, Size gridPoints=100, bool timeDependent = false)
        public FDEngineAdapter(GeneralizedBlackScholesProcess process, int timeSteps, int gridPoints, bool timeDependent) {
            optionBase = (Base)new Base().factory(process, timeSteps, gridPoints, timeDependent);
            process.registerWith(update);
        }

        protected void calculate() {
            optionBase.setupArguments(arguments_);
            optionBase.calculate(results_);
        }

        #region IGenericEngine copy-cat
        protected IPricingEngineArguments arguments_ = new ArgumentsType();
        protected IPricingEngineResults results_ = new ResultsType();

        public IPricingEngineArguments getArguments() { return arguments_; }
        public IPricingEngineResults getResults() { return results_; }
        public void reset() { results_.reset(); }

        #region Observer & Observable
        // observable interface
        public event Callback notifyObserversEvent;
        public void registerWith(Callback handler) { notifyObserversEvent += handler; }
        public void unregisterWith(Callback handler) { notifyObserversEvent -= handler; }
        protected void notifyObservers() {
            Callback handler = notifyObserversEvent;
            if (handler != null) {
                handler();
            }
        }

        public void update() { notifyObservers(); }
        #endregion
        #endregion
    }
}
