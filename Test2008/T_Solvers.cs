using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QLNet;

namespace TestSuite {
    [TestClass()]
    public class Solver1DTest {
        class Foo : ISolver1d {
            public override double value(double x) { return x * x - 1.0; }
            public override double derivative(double x) { return 2.0 * x; }
        };

        public void test(Solver1D solver, string name) {
            double[] accuracy = new double[] { 1.0e-4, 1.0e-6, 1.0e-8 };
            double expected = 1.0;
            for (int i = 0; i < accuracy.Length; i++) {
                double root = solver.solve(new Foo(), accuracy[i], 1.5, 0.1);
                if (Math.Abs(root - expected) > accuracy[i]) {
                    Assert.Fail(name + " solver:\n"
                               + "    expected:   " + expected + "\n"
                               + "    calculated: " + root + "\n"
                               + "    accuracy:   " + accuracy[i]);
                }
                root = solver.solve(new Foo(), accuracy[i], 1.5, 0.0, 1.0);
                if (Math.Abs(root - expected) > accuracy[i]) {
                    Assert.Fail(name + " solver (bracketed):\n"
                               + "    expected:   " + expected + "\n"
                               + "    calculated: " + root + "\n"
                               + "    accuracy:   " + accuracy[i]);
                }
            }
        }

        [TestMethod()]
        public void testBrent() {
            test(new Brent(), "Brent");
        }
        [TestMethod()]
        public void testNewton() {
            test(new Newton(), "Newton");
        }

        public void suite() {
            testBrent();
            testNewton();
        }
    }
}
