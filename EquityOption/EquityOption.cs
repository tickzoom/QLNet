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
using QLNet;

namespace EquityOption {
    class EquityOption {
        static void Main(string[] args) {

            DateTime timer = DateTime.Now;

            // set up dates
            Calendar calendar = new TARGET();
            Date todaysDate = new Date(15, Month.May, 1998);
            Date settlementDate = new Date (17, Month.May, 1998);
            Settings.setEvaluationDate(todaysDate);

            // our options
            Option.Type type = Option.Type.Put;
            double underlying = 36;
            double strike = 40;
            double dividendYield = 0.00;
            double riskFreeRate = 0.06;
            double volatility = 0.20;
            Date maturity = new Date(17, Month.May, 1999);
            DayCounter dayCounter = new Actual365Fixed();

            Console.WriteLine("Option type = " + type);
            Console.WriteLine("Maturity = "    + maturity);
            Console.WriteLine("Underlying price = " + underlying);
            Console.WriteLine("Strike = "      + strike);
            Console.WriteLine("Risk-free interest rate = " + riskFreeRate);
            Console.WriteLine("Dividend yield = " + dividendYield);
            Console.WriteLine("Volatility = " + volatility);
            Console.Write("\n");

            string method;

            Console.Write("\n");

            // write column headings
            int[] widths = new int[]{ 35, 14, 14, 14 };
            Console.WriteLine("{0," + widths[0] + "}" + "Method" +
                              "{0," + widths[1] + "}" + "European" +
                              "{0," + widths[2] + "}" + "Bermudan" +
                              "{0," + widths[3] + "}" + "American");

            List<Date> exerciseDates = new List<Date>(); ;
            for (int i = 1; i <= 4; i++)
                exerciseDates.Add(settlementDate + new Period(3 * i, TimeUnit.Months));

            Exercise europeanExercise = new EuropeanExercise(maturity);
            Exercise bermudanExercise = new BermudanExercise(exerciseDates);
            Exercise americanExercise = new AmericanExercise(settlementDate, maturity);

            Handle<Quote> underlyingH = new Handle<Quote>(new SimpleQuote(underlying));

            // bootstrap the yield/dividend/vol curves
            var flatTermStructure = new Handle<YieldTermStructure>(new FlatForward(settlementDate, riskFreeRate, dayCounter));
            var flatDividendTS = new Handle<YieldTermStructure>(new FlatForward(settlementDate, dividendYield, dayCounter));
            var flatVolTS = new Handle<BlackVolTermStructure>(new BlackConstantVol(settlementDate, calendar, volatility, dayCounter));
            StrikedTypePayoff payoff = new PlainVanillaPayoff(type, strike);
            BlackScholesMertonProcess bsmProcess = new BlackScholesMertonProcess(underlyingH, flatDividendTS,
                                                                                 flatTermStructure, flatVolTS);

            //// options
            //VanillaOption europeanOption(payoff, europeanExercise);
            //VanillaOption bermudanOption(payoff, bermudanExercise);
            //VanillaOption americanOption(payoff, americanExercise);

            //// Analytic formulas:

            //// Black-Scholes for European
            //method = "Black-Scholes";
            //europeanOption.setPricingEngine(boost::shared_ptr<PricingEngine>(
            //                             new AnalyticEuropeanEngine(bsmProcess)));
            //std::cout << std::setw(widths[0]) << std::left << method
            //          << std::fixed
            //          << std::setw(widths[1]) << std::left << europeanOption.NPV()
            //          << std::setw(widths[2]) << std::left << "N/A"
            //          << std::setw(widths[3]) << std::left << "N/A"
            //          << std::endl;

            //// Barone-Adesi and Whaley approximation for American
            //method = "Barone-Adesi/Whaley";
            //americanOption.setPricingEngine(boost::shared_ptr<PricingEngine>(
            //               new BaroneAdesiWhaleyApproximationEngine(bsmProcess)));
            //std::cout << std::setw(widths[0]) << std::left << method
            //          << std::fixed
            //          << std::setw(widths[1]) << std::left << "N/A"
            //          << std::setw(widths[2]) << std::left << "N/A"
            //          << std::setw(widths[3]) << std::left << americanOption.NPV()
            //          << std::endl;

            //// Bjerksund and Stensland approximation for American
            //method = "Bjerksund/Stensland";
            //americanOption.setPricingEngine(boost::shared_ptr<PricingEngine>(
            //              new BjerksundStenslandApproximationEngine(bsmProcess)));
            //std::cout << std::setw(widths[0]) << std::left << method
            //          << std::fixed
            //          << std::setw(widths[1]) << std::left << "N/A"
            //          << std::setw(widths[2]) << std::left << "N/A"
            //          << std::setw(widths[3]) << std::left << americanOption.NPV()
            //          << std::endl;

            //// Integral
            //method = "Integral";
            //europeanOption.setPricingEngine(boost::shared_ptr<PricingEngine>(
            //                                     new IntegralEngine(bsmProcess)));
            //std::cout << std::setw(widths[0]) << std::left << method
            //          << std::fixed
            //          << std::setw(widths[1]) << std::left << europeanOption.NPV()
            //          << std::setw(widths[2]) << std::left << "N/A"
            //          << std::setw(widths[3]) << std::left << "N/A"
            //          << std::endl;

            //// Finite differences
            //Size timeSteps = 801;
            //method = "Finite differences";
            //europeanOption.setPricingEngine(boost::shared_ptr<PricingEngine>(
            //             new FDEuropeanEngine(bsmProcess,timeSteps,timeSteps-1)));
            //bermudanOption.setPricingEngine(boost::shared_ptr<PricingEngine>(
            //             new FDBermudanEngine(bsmProcess,timeSteps,timeSteps-1)));
            //americanOption.setPricingEngine(boost::shared_ptr<PricingEngine>(
            //             new FDAmericanEngine(bsmProcess,timeSteps,timeSteps-1)));
            //std::cout << std::setw(widths[0]) << std::left << method
            //          << std::fixed
            //          << std::setw(widths[1]) << std::left << europeanOption.NPV()
            //          << std::setw(widths[2]) << std::left << bermudanOption.NPV()
            //          << std::setw(widths[3]) << std::left << americanOption.NPV()
            //          << std::endl;

            //// Binomial method: Jarrow-Rudd
            //method = "Binomial Jarrow-Rudd";
            //europeanOption.setPricingEngine(boost::shared_ptr<PricingEngine>(
            //        new BinomialVanillaEngine<JarrowRudd>(bsmProcess,timeSteps)));
            //bermudanOption.setPricingEngine(boost::shared_ptr<PricingEngine>(
            //        new BinomialVanillaEngine<JarrowRudd>(bsmProcess,timeSteps)));
            //americanOption.setPricingEngine(boost::shared_ptr<PricingEngine>(
            //        new BinomialVanillaEngine<JarrowRudd>(bsmProcess,timeSteps)));
            //std::cout << std::setw(widths[0]) << std::left << method
            //          << std::fixed
            //          << std::setw(widths[1]) << std::left << europeanOption.NPV()
            //          << std::setw(widths[2]) << std::left << bermudanOption.NPV()
            //          << std::setw(widths[3]) << std::left << americanOption.NPV()
            //          << std::endl;
            //method = "Binomial Cox-Ross-Rubinstein";
            //europeanOption.setPricingEngine(boost::shared_ptr<PricingEngine>(
            //              new BinomialVanillaEngine<CoxRossRubinstein>(bsmProcess,
            //                                                           timeSteps)));
            //bermudanOption.setPricingEngine(boost::shared_ptr<PricingEngine>(
            //              new BinomialVanillaEngine<CoxRossRubinstein>(bsmProcess,
            //                                                           timeSteps)));
            //americanOption.setPricingEngine(boost::shared_ptr<PricingEngine>(
            //              new BinomialVanillaEngine<CoxRossRubinstein>(bsmProcess,
            //                                                           timeSteps)));
            //std::cout << std::setw(widths[0]) << std::left << method
            //          << std::fixed
            //          << std::setw(widths[1]) << std::left << europeanOption.NPV()
            //          << std::setw(widths[2]) << std::left << bermudanOption.NPV()
            //          << std::setw(widths[3]) << std::left << americanOption.NPV()
            //          << std::endl;

            //// Binomial method: Additive equiprobabilities
            //method = "Additive equiprobabilities";
            //europeanOption.setPricingEngine(boost::shared_ptr<PricingEngine>(
            //        new BinomialVanillaEngine<AdditiveEQPBinomialTree>(bsmProcess,
            //                                                           timeSteps)));
            //bermudanOption.setPricingEngine(boost::shared_ptr<PricingEngine>(
            //        new BinomialVanillaEngine<AdditiveEQPBinomialTree>(bsmProcess,
            //                                                           timeSteps)));
            //americanOption.setPricingEngine(boost::shared_ptr<PricingEngine>(
            //        new BinomialVanillaEngine<AdditiveEQPBinomialTree>(bsmProcess,
            //                                                           timeSteps)));
            //std::cout << std::setw(widths[0]) << std::left << method
            //          << std::fixed
            //          << std::setw(widths[1]) << std::left << europeanOption.NPV()
            //          << std::setw(widths[2]) << std::left << bermudanOption.NPV()
            //          << std::setw(widths[3]) << std::left << americanOption.NPV()
            //          << std::endl;

            //// Binomial method: Binomial Trigeorgis
            //method = "Binomial Trigeorgis";
            //europeanOption.setPricingEngine(boost::shared_ptr<PricingEngine>(
            //        new BinomialVanillaEngine<Trigeorgis>(bsmProcess,timeSteps)));
            //bermudanOption.setPricingEngine(boost::shared_ptr<PricingEngine>(
            //        new BinomialVanillaEngine<Trigeorgis>(bsmProcess,timeSteps)));
            //americanOption.setPricingEngine(boost::shared_ptr<PricingEngine>(
            //        new BinomialVanillaEngine<Trigeorgis>(bsmProcess,timeSteps)));
            //std::cout << std::setw(widths[0]) << std::left << method
            //          << std::fixed
            //          << std::setw(widths[1]) << std::left << europeanOption.NPV()
            //          << std::setw(widths[2]) << std::left << bermudanOption.NPV()
            //          << std::setw(widths[3]) << std::left << americanOption.NPV()
            //          << std::endl;

            //// Binomial method: Binomial Tian
            //method = "Binomial Tian";
            //europeanOption.setPricingEngine(boost::shared_ptr<PricingEngine>(
            //              new BinomialVanillaEngine<Tian>(bsmProcess,timeSteps)));
            //bermudanOption.setPricingEngine(boost::shared_ptr<PricingEngine>(
            //              new BinomialVanillaEngine<Tian>(bsmProcess,timeSteps)));
            //americanOption.setPricingEngine(boost::shared_ptr<PricingEngine>(
            //              new BinomialVanillaEngine<Tian>(bsmProcess,timeSteps)));
            //std::cout << std::setw(widths[0]) << std::left << method
            //          << std::fixed
            //          << std::setw(widths[1]) << std::left << europeanOption.NPV()
            //          << std::setw(widths[2]) << std::left << bermudanOption.NPV()
            //          << std::setw(widths[3]) << std::left << americanOption.NPV()
            //          << std::endl;

            //// Binomial method: Binomial Leisen-Reimer
            //method = "Binomial Leisen-Reimer";
            //europeanOption.setPricingEngine(boost::shared_ptr<PricingEngine>(
            //      new BinomialVanillaEngine<LeisenReimer>(bsmProcess,timeSteps)));
            //bermudanOption.setPricingEngine(boost::shared_ptr<PricingEngine>(
            //      new BinomialVanillaEngine<LeisenReimer>(bsmProcess,timeSteps)));
            //americanOption.setPricingEngine(boost::shared_ptr<PricingEngine>(
            //      new BinomialVanillaEngine<LeisenReimer>(bsmProcess,timeSteps)));
            //std::cout << std::setw(widths[0]) << std::left << method
            //          << std::fixed
            //          << std::setw(widths[1]) << std::left << europeanOption.NPV()
            //          << std::setw(widths[2]) << std::left << bermudanOption.NPV()
            //          << std::setw(widths[3]) << std::left << americanOption.NPV()
            //          << std::endl;

            //// Binomial method: Binomial Joshi
            //method = "Binomial Joshi";
            //europeanOption.setPricingEngine(boost::shared_ptr<PricingEngine>(
            //            new BinomialVanillaEngine<Joshi4>(bsmProcess,timeSteps)));
            //bermudanOption.setPricingEngine(boost::shared_ptr<PricingEngine>(
            //            new BinomialVanillaEngine<Joshi4>(bsmProcess,timeSteps)));
            //americanOption.setPricingEngine(boost::shared_ptr<PricingEngine>(
            //            new BinomialVanillaEngine<Joshi4>(bsmProcess,timeSteps)));
            //std::cout << std::setw(widths[0]) << std::left << method
            //          << std::fixed
            //          << std::setw(widths[1]) << std::left << europeanOption.NPV()
            //          << std::setw(widths[2]) << std::left << bermudanOption.NPV()
            //          << std::setw(widths[3]) << std::left << americanOption.NPV()
            //          << std::endl;

            //// Monte Carlo Method: MC (crude)
            //timeSteps = 1;
            //method = "MC (crude)";
            //Size mcSeed = 42;
            //boost::shared_ptr<PricingEngine> mcengine1;
            //mcengine1 = MakeMCEuropeanEngine<PseudoRandom>(bsmProcess)
            //    .withSteps(timeSteps)
            //    .withTolerance(0.02)
            //    .withSeed(mcSeed);
            //europeanOption.setPricingEngine(mcengine1);
            //// Real errorEstimate = europeanOption.errorEstimate();
            //std::cout << std::setw(widths[0]) << std::left << method
            //          << std::fixed
            //          << std::setw(widths[1]) << std::left << europeanOption.NPV()
            //          << std::setw(widths[2]) << std::left << "N/A"
            //          << std::setw(widths[3]) << std::left << "N/A"
            //          << std::endl;

            //// Monte Carlo Method: QMC (Sobol)
            //method = "QMC (Sobol)";
            //Size nSamples = 32768;  // 2^15

            //boost::shared_ptr<PricingEngine> mcengine2;
            //mcengine2 = MakeMCEuropeanEngine<LowDiscrepancy>(bsmProcess)
            //    .withSteps(timeSteps)
            //    .withSamples(nSamples);
            //europeanOption.setPricingEngine(mcengine2);
            //std::cout << std::setw(widths[0]) << std::left << method
            //          << std::fixed
            //          << std::setw(widths[1]) << std::left << europeanOption.NPV()
            //          << std::setw(widths[2]) << std::left << "N/A"
            //          << std::setw(widths[3]) << std::left << "N/A"
            //          << std::endl;

            //// Monte Carlo Method: MC (Longstaff Schwartz)
            //method = "MC (Longstaff Schwartz)";
            //boost::shared_ptr<PricingEngine> mcengine3;
            //mcengine3 = MakeMCAmericanEngine<PseudoRandom>(bsmProcess)
            //    .withSteps(100)
            //    .withAntitheticVariate()
            //    .withCalibrationSamples(4096)
            //    .withTolerance(0.02)
            //    .withSeed(mcSeed);
            //americanOption.setPricingEngine(mcengine3);
            //std::cout << std::setw(widths[0]) << std::left << method
            //          << std::fixed
            //          << std::setw(widths[1]) << std::left << "N/A"
            //          << std::setw(widths[2]) << std::left << "N/A"
            //          << std::setw(widths[3]) << std::left << americanOption.NPV()
            //          << std::endl;

            //// End test

            Console.WriteLine(" \nRun completed in {0}", DateTime.Now - timer);
            Console.WriteLine();
        }
    }
}
