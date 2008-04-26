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
    //! Single-factor short-rate model abstract class
    /*! \ingroup shortrate */
    public abstract class OneFactorModel : ShortRateModel {
        public OneFactorModel(int nArguments) : base(nArguments) { }

        //! Base class describing the short-rate dynamics
        public abstract class ShortRateDynamics {                        
            private StochasticProcess1D process_;
            //! Returns the risk-neutral dynamics of the state variable
            public StochasticProcess1D process() { return process_; }

            public ShortRateDynamics(StochasticProcess1D process) {
                process_ = process;
            }

            //! Compute state variable from short rate
            public abstract double variable(double t, double r);

            //! Compute short rate from state variable
            public abstract double shortRate(double t, double variable);

        }

        // public class ShortRateTree;

        //! returns the short-rate dynamics
        public abstract ShortRateDynamics dynamics();

        //! Return by default a trinomial recombining tree
        public override Lattice tree(TimeGrid grid) {
            throw new NotImplementedException();
            // TrinomialTree trinomial(new TrinomialTree(dynamics().process(), grid));
            //return new ShortRateTree(trinomial, dynamics(), grid);
        }
    }
}
