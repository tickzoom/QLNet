using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QLNet;

namespace TestSuite {
    /// <summary>
    /// Summary description for LinearLeastSquaresRegression
    /// </summary>
    [TestClass]
    public class T_LinearLeastSquaresRegression {
        [TestMethod]
        public void testRegression() {
            //("Testing linear least-squares regression...");

            //SavedSettings backup;

            const double tolerance = 0.025;
            const int nr=100000;

            
            var rng = new InverseCumulativeRng<MersenneTwisterUniformRng,InverseCumulativeNormal>(
                            new MersenneTwisterUniformRng(1234u));

            List<Func<double, double>> v = new List<Func<double,double>>();
            v.Add(x => 1.0);
            v.Add(x => x);
            v.Add(x => x*x);
            v.Add(Math.Sin);

            List<Func<double, double>> w = new List<Func<double,double>>(v);
            w.Add(x => x*x);

            for (int k=0; k<3; ++k) {
                int i;
                double[] a = { rng.next().value,
                               rng.next().value,
                               rng.next().value,
                               rng.next().value};

                List<double> x = new InitializedList<double>(nr), y = new InitializedList<double>(nr);
                for (i=0; i<nr; ++i) {
                    x[i] = rng.next().value;

                    // regression in y = a_1 + a_2*x + a_3*x^2 + a_4*sin(x) + eps
                    y[i] =  a[0]*v[0](x[i]) + a[1]*v[1](x[i]) + a[2]*v[2](x[i])
                          + a[3]*v[3](x[i]) + rng.next().value;
                }

                LinearLeastSquaresRegression m = new LinearLeastSquaresRegression(x, y, v);

                for (i=0; i<v.Count; ++i) {
                    if (m.error()[i] > tolerance) {
                        Assert.Fail("Failed to reproduce linear regression coef."
                                    + "\n    error:     " + m.error()[i]
                                    + "\n    tolerance: " + tolerance);
                    }
                    if (Math.Abs(m.a()[i]-a[i]) > 3*m.error()[i]) {
                        Assert.Fail("Failed to reproduce linear regression coef."
                                    + "\n    calculated: " + m.a()[i]
                                    + "\n    error:      " + m.error()[i]
                                    + "\n    expected:   " + a[i]);
                    }
                }

                m = new LinearLeastSquaresRegression(x, y, w);

                double[] ma = {m.a()[0], m.a()[1], m.a()[2]+m.a()[4],m.a()[3]};
                double[] err = {m.error()[0], m.error()[1],
                                    Math.Sqrt( m.error()[2]*m.error()[2]
                                              +m.error()[4]*m.error()[4]),
                                    m.error()[3]};
                for (i=0; i<v.Count; ++i) {
                    if (Math.Abs(ma[i] - a[i]) > 3*err[i]) {
                        Assert.Fail("Failed to reproduce linear regression coef."
                                    + "\n    calculated: " + ma[i]
                                    + "\n    error:      " + err[i]
                                    + "\n    expected:   " + a[i]);
                    }
                }
            }

        }
    }
}
