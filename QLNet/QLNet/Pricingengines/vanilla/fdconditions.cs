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
    public class FDAmericanCondition<baseEngine> : FDVanillaEngine, IOptionPricingEngine 
            where baseEngine : IOptionPricingEngine, new() {
        baseEngine engine_;

        // required for generics
        public FDAmericanCondition() { }
        public override IOptionPricingEngine factory(GeneralizedBlackScholesProcess process,
                                            int timeSteps, int gridPoints, bool timeDependent) {
            engine_ = (baseEngine)new baseEngine().factory(process, timeSteps, gridPoints, timeDependent);
            return engine_;
        }

        //public FDAmericanCondition(GeneralizedBlackScholesProcess process,
        //     int timeSteps = 100, int gridPoints = 100, bool timeDependent = false)
        public FDAmericanCondition(GeneralizedBlackScholesProcess process, int timeSteps, int gridPoints, bool timeDependent) {
        }

        protected void initializeStepCondition() {
            // stepCondition_ = new AmericanCondition(intrinsicValues_.values());
        }

        public override void setupArguments(IPricingEngineArguments a) {
            engine_.setupArguments(a);
        }
        public override void calculate(IPricingEngineResults r) {
            engine_.calculate(r);
        }
    }
}
