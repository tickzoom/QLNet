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
    public static class LsmBasisSystem {
        public enum PolynomType { Monomial, Laguerre, Hermite, Hyperbolic,
                                  Legendre, Chebyshev, Chebyshev2th };

        public static List<Func<double, double>> pathBasisSystem(int order, PolynomType polynomType) {
            List<Func<double, double>> ret = new List<Func<double,double>>();
            for (int i=0; i<=order; ++i) {
                //switch (polynomType) {
                //  case PolynomType.Monomial:
                //      ret.Add(MonomialFct(i));
                //    break;
                //  case PolynomType.Laguerre:
                //    ret.Add(
                //        bind(&GaussianOrthogonalPolynomial::weightedValue,
                //                    GaussLaguerrePolynomial(), i, _1));
                //    break;
                //  case PolynomType.Hermite:
                //    ret.Add(
                //        bind(&GaussianOrthogonalPolynomial::weightedValue,
                //                    GaussHermitePolynomial(), i, _1));
                //    break;
                //  case PolynomType.Hyperbolic:
                //    ret.Add(
                //        bind(&GaussianOrthogonalPolynomial::weightedValue,
                //                    GaussHyperbolicPolynomial(), i, _1));
                //    break;
                //  case PolynomType.Legendre:
                //    ret.Add(
                //        bind(&GaussianOrthogonalPolynomial::weightedValue,
                //                    GaussLegendrePolynomial(), i, _1));
                //    break;
                //  case PolynomType.Chebyshev:
                //    ret.Add(
                //        bind(&GaussianOrthogonalPolynomial::weightedValue,
                //                    GaussChebyshevPolynomial(), i, _1));
                //    break;
                //  case PolynomType.Chebyshev2th:
                //    ret.Add(
                //        bind(&GaussianOrthogonalPolynomial::weightedValue,
                //                    GaussChebyshev2thPolynomial(), i, _1));
                //    break;
                //  default:
                //    QL_FAIL("unknown regression type");
                //}
            }

        return ret;
    }

        //public static List<Func<double, Vector>> multiPathBasisSystem(int dim, int order, PolynomType polynomType);

        //private static List<Func<double, Vector>>
        //    w(int dim, int order, PolynomType polynomType, List<Func<double, double>> basis);
    }


    public class MonomialFct : IValue {
        private int order_;

        public MonomialFct(int order) {
            order_ = order;
        }

        public double value(double x) {
            double ret = 1.0;
            for (int i=0; i<order_; ++i) {
                ret*=x;
            }
            return ret;
        }
    }
}
