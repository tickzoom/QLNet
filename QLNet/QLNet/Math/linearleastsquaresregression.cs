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
    //! general linear least squares regression
    /*! References:
       "Numerical Recipes in C", 2nd edition,
        Press, Teukolsky, Vetterling, Flannery,

        \test the correctness of the returned values is tested by
              checking their properties.
    */
    public class LinearLeastSquaresRegression<ArgumentType> {
        private Vector a_;
        private Vector err_;

        public LinearLeastSquaresRegression(List<ArgumentType> x, List<double> y, List<Func<ArgumentType, double>> v) {
            a_ = new Vector(v.Count, 0.0);
            err_ = new Vector(v.Count, 0.0);

            if (x.Count != y.Count)
                throw new ApplicationException("sample set need to be of the same size");
            if (!(x.Count >= v.Count))
                throw new ApplicationException("sample set is too small");

            int i;
            int n = x.Count;
            int m = v.Count;

            Matrix A = new Matrix(n, m);
            for (i=0; i<m; ++i)
                for(int j=0;j<x.Count; j++)
                    A[j,i] = v[i](x[i]);

            SVD svd = new SVD(A);
            Matrix V = svd.V();
            Matrix U = svd.U();
            Vector  w = svd.singularValues();
            double threshold = n*Const.QL_Epsilon;

            for (i=0; i<m; ++i) {
                if (w[i] > threshold) {
                    double u = 0;
                    U.column(i).ForEach((ii, vv) => u += vv * y[ii]);
                    u /= w[i];

                    for (int j=0; j<m; ++j) {
                        a_[j]  +=u*V[j,i];
                        err_[j]+=V[j,i]*V[j,i]/(w[i]*w[i]);
                    }
                }
            }
            err_ = Vector.Sqrt(err_);
        }

        public Vector a() { return a_; }
        public Vector error() { return err_; }
    }
}
