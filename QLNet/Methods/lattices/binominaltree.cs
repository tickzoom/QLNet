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
    //! Binomial tree base class
    /*! \ingroup lattices */
    public class BinomialTree<T> : Tree<T> {
        public enum Branches { branches = 2 };

        protected double x0_, driftPerStep_;
        protected double dt_;

        public BinomialTree(StochasticProcess1D process, double end, int steps)
            : base(steps+1) {
            x0_ = process.x0();
            dt_ = end/steps;
            driftPerStep_ = process.drift(0.0, x0_) * dt_;
        }

        public int size(int i) { return i+1; }
        public int descendant(int x, int index, int branch) { return index + branch; }
    }

    //! Base class for equal probabilities binomial tree
    /*! \ingroup lattices */
    public class EqualProbabilitiesBinomialTree<T> : BinomialTree<T> {
        protected double up_;

        public EqualProbabilitiesBinomialTree(StochasticProcess1D process, double end, int steps)
            : base(process, end, steps) {}

        public double underlying(int i, int index) {
            long j = 2*index - i;
            // exploiting the forward value tree centering
            return this.x0_*Math.Exp(i*driftPerStep_ + j*up_);
        }
        public double probability(int x, int y, int z) { return 0.5; }
    }

    //! Base class for equal jumps binomial tree
    /*! \ingroup lattices */
    public class EqualJumpsBinomialTree<T> : BinomialTree<T> {
        protected double dx_, pu_, pd_;

        public EqualJumpsBinomialTree(StochasticProcess1D process, double end, int steps)
            : base(process, end, steps) {}

        public double underlying(int i, int index) {
            long j = 2*index - i;
            // exploiting equal jump and the x0_ tree centering
            return x0_*Math.Exp(j*dx_);
        }
        public double probability(int x, int y, int branch) { return (branch == 1 ? pu_ : pd_); }
    }


    //! Jarrow-Rudd (multiplicative) equal probabilities binomial tree
    /*! \ingroup lattices */
    public class JarrowRudd : EqualProbabilitiesBinomialTree<JarrowRudd> {
        public JarrowRudd(StochasticProcess1D process, double end, int steps, double strike) 
            : base(process, end, steps) {
            // drift removed
            up_ = process.stdDeviation(0.0, x0_, dt_);
        }
    }

    //! Cox-Ross-Rubinstein (multiplicative) equal jumps binomial tree
    /*! \ingroup lattices */
    public class CoxRossRubinstein : EqualJumpsBinomialTree<CoxRossRubinstein> {
        public CoxRossRubinstein(StochasticProcess1D process, double end, int steps, double strike)    
            : base(process, end, steps) {

            dx_ = process.stdDeviation(0.0, x0_, dt_);
            pu_ = 0.5 + 0.5*driftPerStep_/dx_;;
            pd_ = 1.0 - pu_;

            if (!(pu_<=1.0)) throw new ApplicationException("negative probability");
            if (!(pu_ >= 0.0)) throw new ApplicationException("negative probability");
        }
    }

    //! Additive equal probabilities binomial tree
    /*! \ingroup lattices */
    public class AdditiveEQPBinomialTree : EqualProbabilitiesBinomialTree<AdditiveEQPBinomialTree> {
        public AdditiveEQPBinomialTree(StochasticProcess1D process, double end, int steps, double strike)    
            : base(process, end, steps) {
            up_ = - 0.5 * driftPerStep_ + 0.5 * Math.Sqrt(4.0*process.variance(0.0, x0_, dt_)- 3.0*driftPerStep_*driftPerStep_);
        }
    }

    //! %Trigeorgis (additive equal jumps) binomial tree
    /*! \ingroup lattices */
    public class Trigeorgis : EqualJumpsBinomialTree<Trigeorgis> {
        public Trigeorgis(StochasticProcess1D process, double end, int steps, double strike)    
            : base(process, end, steps) {

            dx_ = Math.Sqrt(process.variance(0.0, x0_, dt_)+ driftPerStep_*driftPerStep_);
            pu_ = 0.5 + 0.5*driftPerStep_/dx_;;
            pd_ = 1.0 - pu_;

            if (!(pu_<=1.0)) throw new ApplicationException("negative probability");
            if (!(pu_ >= 0.0)) throw new ApplicationException("negative probability");
        }
    }

    //! %Tian tree: third moment matching, multiplicative approach
    /*! \ingroup lattices */
    public class Tian : BinomialTree<Tian> {
        protected double up_, down_, pu_, pd_;

        public Tian(StochasticProcess1D process, double end, int steps, double strike)    
            : base(process, end, steps) {

            double q = Math.Exp(process.variance(0.0, x0_, dt_));
            double r = Math.Exp(driftPerStep_)*Math.Sqrt(q);

            up_ = 0.5 * r * q * (q + 1 + Math.Sqrt(q * q + 2 * q - 3));
            down_ = 0.5 * r * q * (q + 1 - Math.Sqrt(q * q + 2 * q - 3));

            pu_ = (r - down_) / (up_ - down_);
            pd_ = 1.0 - pu_;

            // doesn't work
            //     treeCentering_ = (up_+down_)/2.0;
            //     up_ = up_-treeCentering_;

            if (!(pu_<=1.0)) throw new ApplicationException("negative probability");
            if (!(pu_ >= 0.0)) throw new ApplicationException("negative probability");
        }

        public double underlying(int i, int index) {
            return x0_ * Math.Pow(down_, i-index) * Math.Pow(up_, index);
        }
        public double probability(int i, int j, int branch) { return (branch == 1 ? pu_ : pd_); }
    }

    //! Leisen & Reimer tree: multiplicative approach
    /*! \ingroup lattices */
    public class LeisenReimer : BinomialTree<LeisenReimer> {
        protected double up_, down_, pu_, pd_;
        public LeisenReimer(StochasticProcess1D process, double end, int steps, double strike)    
            : base(process, end, (steps%2 != 0 ? steps : steps+1)) {

            if (!(strike>0.0)) throw new ApplicationException("strike must be positive");
            int oddSteps = (steps%2 != 0 ? steps : steps+1);
            double variance = process.variance(0.0, x0_, end);
            double ermqdt = Math.Exp(driftPerStep_ + 0.5*variance/oddSteps);
            double d2 = (Math.Log(x0_/strike) + driftPerStep_*oddSteps ) / Math.Sqrt(variance);
            pu_ = Utils.PeizerPrattMethod2Inversion(d2, oddSteps);
            pd_ = 1.0 - pu_;
            double pdash = Utils.PeizerPrattMethod2Inversion(d2 + Math.Sqrt(variance), oddSteps);
            up_ = ermqdt * pdash / pu_;
            down_ = (ermqdt - pu_ * up_) / (1.0 - pu_);

        }

        public double underlying(int i, int index) {
            return x0_ * Math.Pow(down_, i-index) * Math.Pow(up_, index); 
        }
        public double probability(int i, int j, int branch) {
            return (branch == 1 ? pu_ : pd_);
        }
    }

    public class Joshi4 : BinomialTree<Joshi4> {
        protected double up_, down_, pu_, pd_;

        public Joshi4(StochasticProcess1D process, double end, int steps, double strike)    
            : base(process, end, (steps%2 != 0 ? steps : steps+1)) {

            if (!(strike>0.0)) throw new ApplicationException("strike must be positive");

            int oddSteps = (steps%2 != 0 ? steps : steps+1);
            double variance = process.variance(0.0, x0_, end);
            double ermqdt = Math.Exp(driftPerStep_ + 0.5*variance/oddSteps);
            double d2 = (Math.Log(x0_/strike) + driftPerStep_*oddSteps ) / Math.Sqrt(variance);
            pu_ = computeUpProb((oddSteps-1.0)/2.0,d2 );
            pd_ = 1.0 - pu_;
            double pdash = computeUpProb((oddSteps-1.0)/2.0,d2+Math.Sqrt(variance));
            up_ = ermqdt * pdash / pu_;
            down_ = (ermqdt - pu_ * up_) / (1.0 - pu_);
        }

        public double underlying(int i, int index) {
            return x0_ * Math.Pow(down_, i-index) * Math.Pow(up_, index);
        }
        public double probability(int x, int y, int branch) { return (branch == 1 ? pu_ : pd_); }

        protected double computeUpProb(double k, double dj) {
            double alpha = dj / (Math.Sqrt(8.0));
            double alpha2 = alpha * alpha;
            double alpha3 = alpha * alpha2;
            double alpha5 = alpha3 * alpha2;
            double alpha7 = alpha5 * alpha2;
            double beta = -0.375 * alpha - alpha3;
            double gamma = (5.0 / 6.0) * alpha5 + (13.0 / 12.0) * alpha3
                +(25.0/128.0)*alpha;
            double delta = -0.1025 * alpha - 0.9285 * alpha3
                -1.43 *alpha5 -0.5 *alpha7;
            double p = 0.5;
            double rootk = Math.Sqrt(k);
            p+= alpha/rootk;
            p+= beta /(k*rootk);
            p+= gamma/(k*k*rootk);
            // delete next line to get results for j three tree
            p+= delta/(k*k*k*rootk);
            return p;
        }

    }
}
