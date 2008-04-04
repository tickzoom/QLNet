using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QLNet;

namespace TestSuite {
    public class Solver1DTest {
        class Foo : ISolver1d {
            public override double value(double x) { return x * x - 1.0; }
            public override double derivative(double x) { return 2.0 * x; }
        };

        static void test(Solver1D solver, string name) {
            double[] accuracy = new double[] { 1.0e-4, 1.0e-6, 1.0e-8 };
            double expected = 1.0;
            for (int i = 0; i < accuracy.Length; i++) {
                double root = solver.solve(new Foo(), accuracy[i], 1.5, 0.1);
                if (Math.Abs(root - expected) > accuracy[i]) {
                    throw new ApplicationException(name + " solver:\n"
                               + "    expected:   " + expected + "\n"
                               + "    calculated: " + root + "\n"
                               + "    accuracy:   " + accuracy[i]);
                }
                root = solver.solve(new Foo(), accuracy[i], 1.5, 0.0, 1.0);
                if (Math.Abs(root - expected) > accuracy[i]) {
                    throw new ApplicationException(name + " solver (bracketed):\n"
                               + "    expected:   " + expected + "\n"
                               + "    calculated: " + root + "\n"
                               + "    accuracy:   " + accuracy[i]);
                }
            }
        }

        public void suite() {
            test(new Brent(), "Brent");
            test(new Newton(), "Newton");
        }
    }
}
