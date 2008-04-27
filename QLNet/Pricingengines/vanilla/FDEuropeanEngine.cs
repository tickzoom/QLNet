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
    //! Pricing engine for European options using finite-differences
    /*! \ingroup vanillaengines

        \test the correctness of the returned value is tested by
              checking it against analytic results.
    */
    public class FDEuropeanEngine : OneAssetOption.Engine {
        private SampledCurve prices_;
        private FDVanillaEngine engine_;

        //public FDEuropeanEngine(GeneralizedBlackScholesProcess process,
        //                        Size timeSteps=100, Size gridPoints=100, bool timeDependent = false) {
        public FDEuropeanEngine(GeneralizedBlackScholesProcess process, int timeSteps, int gridPoints, bool timeDependent) {
            engine_ = new FDVanillaEngine(process, timeSteps, gridPoints, timeDependent);
            prices_ = new SampledCurve(gridPoints);

            process.registerWith(update);
        }

        //new private void calculate() {
        //    setupArguments(arguments_);
        //    setGridLimits();
        //    initializeInitialCondition();
        //    initializeOperator();
        //    initializeBoundaryConditions();

        //    var model = new FiniteDifferenceModel<CrankNicolson<TridiagonalOperator>>(finiteDifferenceOperator_, BCs_);

        //    prices_ = intrinsicValues_;

        //    model.rollback(prices_.values(), getResidualTime(), 0, timeSteps_);

        //    results_.value = prices_.valueAtCenter();
        //    results_.delta = prices_.firstDerivativeAtCenter();
        //    results_.gamma = prices_.secondDerivativeAtCenter();
        //    results_.theta = Utils.blackScholesTheta(process_,
        //                                       results_.value,
        //                                       results_.delta,
        //                                       results_.gamma);
        //    results_.additionalResults["priceCurve"] = prices_;
        //}
    }
}
