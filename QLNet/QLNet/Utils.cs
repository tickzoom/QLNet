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
using QLNet.Time;

namespace QLNet
{
	// here are extensions to IList to accomodate some QL functionality as well as have useful things for .net
	public static class Utils
	{
		public static bool empty<T>(this IList<T> items) { return items.Count == 0; }

		// equivalent of ForEach but with the index
		public static void ForEach<T>(this IList<T> items, Action<int, T> action)
		{
			if (items != null && action != null)
				for (int idx = 0; idx < items.Count; idx++)
					action(idx, items[idx]);
		}

		// this is a version of element retrieval with some logic for default values
		public static T Get<T>(this List<T> v, int i)
		{
			return Get(v, i, default(T));
		}

		public static T Get<T>(this List<T> v, int i, T defval)
		{
			if (v == null || v.Count == 0) return defval;
			else if (i >= v.Count) return v.Last();
			else return v[i];
		}

		public static double effectiveFixedRate(List<double> spreads, List<double> caps, List<double> floors, int i)
		{
			double result = Get(spreads, i);
			double floor = Get(floors, i);
			double cap = Get(caps, i);
			if (floor != default(double)) result = Math.Max(floor, result);
			if (cap != default(double)) result = Math.Min(cap, result);
			return result;
		}

		public static bool noOption(List<double> caps, List<double> floors, int i)
		{
			return (Get(caps, i) == default(double)) && (Get(floors, i) == default(double));
		}

		public static void swap(ref double a1, ref double a2)
		{
			swap<double>(ref a1, ref a2);
		}

		public static void swap<T>(ref T a1, ref T a2)
		{
			T t = a2;
			a2 = a1;
			a1 = t;
		}

		// this is the overload for Pow with int power: much faster and more precise
		public static double Pow(double x, int y)
		{
			int n = Math.Abs(y);
			double retval = 1;
			for (; ; x *= x)
			{
				if ((n & 1) != 0) retval *= x;
				if ((n >>= 1) == 0) return y < 0 ? 1 / retval : retval;
			}
		}

		public static double unsafeSabrVolatility(double strike, double forward, double expiryTime, double alpha, double beta,
								   double nu, double rho)
		{
			double oneMinusBeta = 1.0 - beta;
			double A = Math.Pow(forward * strike, oneMinusBeta);
			double sqrtA = Math.Sqrt(A);
			double logM;

			if (!Utils.close(forward, strike))
				logM = Math.Log(forward / strike);
			else
			{
				double epsilon = (forward - strike) / strike;
				logM = epsilon - .5 * epsilon * epsilon;
			}
			double z = (nu / alpha) * sqrtA * logM;
			double B = 1.0 - 2.0 * rho * z + z * z;
			double C = oneMinusBeta * oneMinusBeta * logM * logM;
			double tmp = (Math.Sqrt(B) + z - rho) / (1.0 - rho);
			double xx = Math.Log(tmp);
			double D = sqrtA * (1.0 + C / 24.0 + C * C / 1920.0);
			double d = 1.0 + expiryTime *
						(oneMinusBeta * oneMinusBeta * alpha * alpha / (24.0 * A)
											+ 0.25 * rho * beta * nu * alpha / sqrtA
												+ (2.0 - 3.0 * rho * rho) * (nu * nu / 24.0));

			double multiplier;
			// computations become precise enough if the square of z worth slightly more than the precision machine (hence the m)
			const double m = 10;

			if (Math.Abs(z * z) > Const.QL_Epsilon * m)
				multiplier = z / xx;
			else
			{
				alpha = (0.5 - rho * rho) / (1.0 - rho);
				beta = alpha - .5;
				double gamma = rho / (1 - rho);
				multiplier = 1.0 - beta * z + (gamma - alpha + beta * beta * .5) * z * z;
			}
			return (alpha / D) * multiplier * d;
		}

		public static void validateSabrParameters(double alpha, double beta, double nu, double rho)
		{
			if (!(alpha > 0.0))
				throw new ApplicationException("alpha must be positive: " + alpha + " not allowed");
			if (!(beta >= 0.0 && beta <= 1.0))
				throw new ApplicationException("beta must be in (0.0, 1.0): " + beta + " not allowed");
			if (!(nu >= 0.0))
				throw new ApplicationException("nu must be non negative: " + nu + " not allowed");
			if (!(rho * rho < 1.0))
				throw new ApplicationException("rho square must be less than one: " + rho + " not allowed");
		}

		public static double sabrVolatility(double strike, double forward, double expiryTime, double alpha, double beta,
									 double nu, double rho)
		{
			if (!(strike > 0.0))
				throw new ApplicationException("strike must be positive: " + strike + " not allowed");
			if (!(forward > 0.0))
				throw new ApplicationException("at the money forward rate must be: " + forward + " not allowed");
			if (!(expiryTime >= 0.0))
				throw new ApplicationException("expiry time must be non-negative: " + expiryTime + " not allowed");
			validateSabrParameters(alpha, beta, nu, rho);
			return unsafeSabrVolatility(strike, forward, expiryTime, alpha, beta, nu, rho);
		}

		public static double? toNullable(double val)
		{
			if (val == double.MinValue)
				return null;
			else
				return val;
		}

		public static void setCouponPricer(List<CashFlow> leg, FloatingRateCouponPricer pricer)
		{
			PricerSetter setter = new PricerSetter(pricer);
			foreach (CashFlow cf in leg)
			{
				cf.accept(setter);
			}
		}

		public static void setCouponPricers(List<CashFlow> leg, List<FloatingRateCouponPricer> pricers)
		{
			throw new NotImplementedException();
			//int nCashFlows = leg.Count;
			//if (!(nCashFlows > 0))
			//    throw new ApplicationException("no cashflows");

			//int nPricers = pricers.Count;
			//if (!(nCashFlows >= nPricers))
			//    throw new ApplicationException("mismatch between leg size (" + nCashFlows + ") and number of pricers (" + nPricers + ")");

			//for (int i = 0; i < nCashFlows; ++i)
			//{
			//    PricerSetter[] setter = new PricerSetter[i](i < nPricers ? pricers : pricers[nPricers - 1]);
			//    leg[i].accept(setter);
			//}
		}

		//! helper function building a sequence of fixed dividends
		public static List<Dividend> DividendVector(List<Date> dividendDates, List<double> dividends)
		{
			if (dividendDates.Count != dividends.Count)
				throw new ApplicationException("size mismatch between dividend dates and amounts");

			List<Dividend> items = new List<Dividend>(dividendDates.Count);
			for (int i = 0; i < dividendDates.Count; i++)
				items.Add(new FixedDividend(dividends[i], dividendDates[i]));
			return items;
		}

		public static Vector CenteredGrid(double center, double dx, int steps)
		{
			Vector result = new Vector(steps + 1);
			for (int i = 0; i < steps + 1; i++)
				result[i] = center + (i - steps / 2.0) * dx;
			return result;
		}

		public static Vector BoundedGrid(double xMin, double xMax, int steps)
		{
			return new Vector(steps + 1, xMin, (xMax - xMin) / steps);
		}

		public static Vector BoundedLogGrid(double xMin, double xMax, int steps)
		{
			Vector result = new Vector(steps + 1);
			double gridLogSpacing = (Math.Log(xMax) - Math.Log(xMin)) /
				(steps);
			double edx = Math.Exp(gridLogSpacing);
			result[0] = xMin;
			for (int j = 1; j < steps + 1; j++)
			{
				result[j] = result[j - 1] * edx;
			}
			return result;
		}

		public static Date previousWednesday(Date date)
		{
			int w = date.weekday();
			if (w >= 4) // roll back w-4 days
				return date - new Period((w - 4), TimeUnit.Days);
			else // roll forward 4-w days and back one week
				return date + new Period((4 - w - 7), TimeUnit.Days);
		}

		public static Date nextWednesday(Date date)
		{
			return previousWednesday(date + 7);
		}

		public static BusinessDayConvention euriborConvention(Period p)
		{
			switch (p.units())
			{
				case TimeUnit.Days:
				case TimeUnit.Weeks:
					return BusinessDayConvention.Following;
				case TimeUnit.Months:
				case TimeUnit.Years:
					return BusinessDayConvention.ModifiedFollowing;
				default:
					throw new ArgumentException("Unknown TimeUnit: " + p.units());
			}
		}

		public static bool euriborEOM(Period p)
		{
			switch (p.units())
			{
				case TimeUnit.Days:
				case TimeUnit.Weeks:
					return false;
				case TimeUnit.Months:
				case TimeUnit.Years:
					return true;
				default:
					throw new ArgumentException("Unknown TimeUnit: " + p.units());
			}
		}

		public static BusinessDayConvention eurliborConvention(Period p)
		{
			switch (p.units())
			{
				case TimeUnit.Days:
				case TimeUnit.Weeks:
					return BusinessDayConvention.Following;
				case TimeUnit.Months:
				case TimeUnit.Years:
					return BusinessDayConvention.ModifiedFollowing;
				default:
					throw new ArgumentException("Unknown TimeUnit: " + p.units());
			}
		}

		public static bool eurliborEOM(Period p)
		{
			switch (p.units())
			{
				case TimeUnit.Days:
				case TimeUnit.Weeks:
					return false;
				case TimeUnit.Months:
				case TimeUnit.Years:
					return true;
				default:
					throw new ArgumentException("Unknown TimeUnit: " + p.units());
			}
		}

		public static double dirtyPriceFromYield(double faceAmount, List<CashFlow> cashflows, double yield, DayCounter dayCounter,
					 Compounding compounding, Frequency frequency, Date settlement)
		{

			if (frequency == Frequency.NoFrequency || frequency == Frequency.Once)
				frequency = Frequency.Annual;

			InterestRate y = new InterestRate(yield, dayCounter, compounding, frequency);

			double price = 0.0;
			double discount = 1.0;
			Date lastDate = null;

			for (int i = 0; i < cashflows.Count - 1; ++i)
			{
				if (cashflows[i].hasOccurred(settlement))
					continue;

				Date couponDate = cashflows[i].Date;
				double amount = cashflows[i].amount();
				if (lastDate == null)
				{
					// first not-expired coupon
					if (i > 0)
					{
						lastDate = cashflows[i - 1].Date;
					}
					else
					{
						if (cashflows[i].GetType().IsSubclassOf(typeof(Coupon)))
							lastDate = ((Coupon)cashflows[i]).accrualStartDate();
						else
							lastDate = couponDate - new Period(1, TimeUnit.Years);
					}
					discount *= y.discountFactor(settlement, couponDate, lastDate, couponDate);
				}
				else
				{
					discount *= y.discountFactor(lastDate, couponDate);
				}
				lastDate = couponDate;

				price += amount * discount;
			}

			CashFlow redemption = cashflows.Last();
			if (!redemption.hasOccurred(settlement))
			{
				Date redemptionDate = redemption.Date;
				double amount = redemption.amount();
				if (lastDate == null)
				{
					// no coupons
					lastDate = redemptionDate - new Period(1, TimeUnit.Years);
					discount *= y.discountFactor(settlement, redemptionDate, lastDate, redemptionDate);
				}
				else
				{
					discount *= y.discountFactor(lastDate, redemptionDate);
				}

				price += amount * discount;
			}

			return price / faceAmount * 100.0;
		}

		public static double dirtyPriceFromZSpreadFunction(double faceAmount, List<CashFlow> cashflows, double zSpread,
														   DayCounter dc, Compounding comp, Frequency freq, Date settlement,
														   Handle<YieldTermStructure> discountCurve)
		{

			if (!(freq != Frequency.NoFrequency && freq != Frequency.Once))
				throw new ApplicationException("invalid frequency:" + freq);

			Quote zSpreadQuoteHandle = new SimpleQuote(zSpread);

			var spreadedCurve = new ZeroSpreadedTermStructure(discountCurve, zSpreadQuoteHandle, comp, freq, dc);

			double price = 0.0;
			foreach (CashFlow cf in cashflows.FindAll(x => !x.hasOccurred(settlement)))
			{
				Date couponDate = cf.Date;
				double amount = cf.amount();
				price += amount * spreadedCurve.discount(couponDate);
			}
			price /= spreadedCurve.discount(settlement);
			return price / faceAmount * 100.0;
		}

		public static double betaFunction(double z, double w)
		{
			return Math.Exp(GammaFunction.logValue(z) +
							GammaFunction.logValue(w) -
							GammaFunction.logValue(z + w));
		}

		public static double betaContinuedFraction(double a, double b, double x)
		{
			return betaContinuedFraction(a, b, x, 1e-16, 100);
		}

		public static double betaContinuedFraction(double a, double b, double x, double accuracy, int maxIteration)
		{
			double aa, del;
			double qab = a + b;
			double qap = a + 1.0;
			double qam = a - 1.0;
			double c = 1.0;
			double d = 1.0 - qab * x / qap;
			if (Math.Abs(d) < Const.QL_Epsilon)
				d = Const.QL_Epsilon;
			d = 1.0 / d;
			double result = d;

			int m, m2;
			for (m = 1; m <= maxIteration; m++)
			{
				m2 = 2 * m;
				aa = m * (b - m) * x / ((qam + m2) * (a + m2));
				d = 1.0 + aa * d;
				if (Math.Abs(d) < Const.QL_Epsilon) d = Const.QL_Epsilon;
				c = 1.0 + aa / c;
				if (Math.Abs(c) < Const.QL_Epsilon) c = Const.QL_Epsilon;
				d = 1.0 / d;
				result *= d * c;
				aa = -(a + m) * (qab + m) * x / ((a + m2) * (qap + m2));
				d = 1.0 + aa * d;
				if (Math.Abs(d) < Const.QL_Epsilon) d = Const.QL_Epsilon;
				c = 1.0 + aa / c;
				if (Math.Abs(c) < Const.QL_Epsilon) c = Const.QL_Epsilon;
				d = 1.0 / d;
				del = d * c;
				result *= del;
				if (Math.Abs(del - 1.0) < accuracy)
					return result;
			}
			throw new ApplicationException("a or b too big, or maxIteration too small in betacf");
		}

		/*! Incomplete Beta function

			The implementation of the algorithm was inspired by
			"Numerical Recipes in C", 2nd edition,
			Press, Teukolsky, Vetterling, Flannery, chapter 6
		*/
		public static double incompleteBetaFunction(double a, double b, double x)
		{
			return incompleteBetaFunction(a, b, x, 1e-16, 100);
		}
		public static double incompleteBetaFunction(double a, double b, double x, double accuracy, int maxIteration)
		{

			if (!(a > 0.0)) throw new ApplicationException("a must be greater than zero");
			if (!(b > 0.0)) throw new ApplicationException("b must be greater than zero");


			if (x == 0.0)
				return 0.0;
			else if (x == 1.0)
				return 1.0;
			else
				if (!(x > 0.0 && x < 1.0)) throw new ApplicationException("x must be in [0,1]");

			double result = Math.Exp(GammaFunction.logValue(a + b) -
				GammaFunction.logValue(a) - GammaFunction.logValue(b) +
				a * Math.Log(x) + b * Math.Log(1.0 - x));

			if (x < (a + 1.0) / (a + b + 2.0))
				return result *
					betaContinuedFraction(a, b, x, accuracy, maxIteration) / a;
			else
				return 1.0 - result *
					betaContinuedFraction(b, a, 1.0 - x, accuracy, maxIteration) / b;
		}

		/*! Follows somewhat the advice of Knuth on checking for floating-point
			equality. The closeness relationship is:
			\f[
			\mathrm{close}(x,y,n) \equiv |x-y| \leq \varepsilon |x|
								  \wedge |x-y| \leq \varepsilon |y|
			\f]
			where \f$ \varepsilon \f$ is \f$ n \f$ times the machine accuracy;
			\f$ n \f$ equals 42 if not given.  */
		public static bool close(double x, double y) { return close(x, y, 42); }
		public static bool close(double x, double y, int n)
		{
			double diff = System.Math.Abs(x - y), tolerance = n * Const.QL_Epsilon;
			return diff <= tolerance * System.Math.Abs(x) && diff <= tolerance * System.Math.Abs(y);
		}

		public static bool close(Money m1, Money m2)
		{
			return close(m1, m2, 42);
		}

		public static bool close_enough(double x, double y)
		{
			return close_enough(x, y, 42);
		}

		public static bool close_enough(double x, double y, int n)
		{
			double diff = Math.Abs(x - y), tolerance = n * Const.QL_Epsilon;
			return diff <= tolerance * Math.Abs(x) || diff <= tolerance * Math.Abs(y);
		}

		public static bool close(Money m1, Money m2, int n)
		{
			if (m1.currency == m2.currency)
			{
				return close(m1.value, m2.value, n);
			}
			else if (Money.conversionType == Money.ConversionType.BaseCurrencyConversion)
			{
				Money tmp1 = m1;
				Money.convertToBase(ref tmp1);
				Money tmp2 = m2;
				Money.convertToBase(ref tmp2);
				return close(tmp1, tmp2, n);
			}
			else if (Money.conversionType == Money.ConversionType.AutomatedConversion)
			{
				Money tmp = m2;
				Money.convertTo(ref tmp, m1.currency);
				return close(m1, tmp, n);
			}
			else
			{
				throw new Exception("currency mismatch and no conversion specified");
			}
		}

		/*! Given an odd integer n and a real number z it returns p such that:
1 - CumulativeBinomialDistribution((n-1)/2, n, p) =
					   CumulativeNormalDistribution(z)

\pre n must be odd
*/
		public static double PeizerPrattMethod2Inversion(double z, int n)
		{

			if (!(n % 2 == 1)) throw new ApplicationException("n must be an odd number: " + n + " not allowed");

			double result = (z / (n + 1.0 / 3.0 + 0.1 / (n + 1.0)));
			result *= result;
			result = Math.Exp(-result * (n + 1.0 / 6.0));
			result = 0.5 + (z > 0 ? 1 : -1) * Math.Sqrt((0.25 * (1.0 - result)));
			return result;
		}

		public static double binomialCoefficientLn(int n, int k)
		{
			if (!(n >= k)) throw new ApplicationException("n<k not allowed");

			return Factorial.ln(n) - Factorial.ln(k) - Factorial.ln(n - k);
		}

		public static double binomialCoefficient(int n, int k)
		{
			return Math.Floor(0.5 + Math.Exp(Utils.binomialCoefficientLn(n, k)));
		}

		// Computes the size of the simplex
		public static double computeSimplexSize(InitializedList<Vector> vertices)
		{
			Vector center = new Vector(vertices[0].Count, 0);
			for (int i = 0; i < vertices.Count; ++i)
				center += vertices[i];
			center *= 1 / (double)(vertices.Count);
			double result = 0;
			for (int i = 0; i < vertices.Count; ++i)
			{
				Vector temp = vertices[i] - center;
				result += Math.Sqrt(Vector.DotProduct(temp, temp));
			}
			return result / (double)(vertices.Count);
		}

		private static void checkParameters(double strike, double forward, double displacement)
		{
			if (!(strike >= 0.0))
				throw new ApplicationException("strike (" + strike + ") must be non-negative");
			if (!(forward > 0.0))
				throw new ApplicationException("forward (" + forward + ") must be positive");
			if (!(displacement >= 0.0))
				throw new ApplicationException("displacement (" + displacement + ") must be non-negative");
		}

		public static double blackFormula(Option.Type optionType, double strike, double forward, double stdDev)
		{
			return blackFormula(optionType, strike, forward, stdDev, 1.0, 0.0);
		}

		public static double blackFormula(Option.Type optionType, double strike, double forward, double stdDev, double discount)
		{
			return blackFormula(optionType, strike, forward, stdDev, discount, 0.0);
		}

		public static double blackFormula(Option.Type optionType, double strike, double forward, double stdDev, double discount, double displacement)
		{
			checkParameters(strike, forward, displacement);

			if (!(stdDev >= 0.0))
				throw new ApplicationException("stdDev (" + stdDev + ") must be non-negative");

			if (!(discount > 0.0))
				throw new ApplicationException("discount (" + discount + ") must be positive");

			if (stdDev == 0.0)
				return Math.Max((forward - strike) * (int)optionType, 0.0) * discount;

			forward = forward + displacement;
			strike = strike + displacement;

			// since displacement is non-negative strike==0 iff displacement==0
			// so returning forward*discount is OK 
			if (strike == 0.0)
				return (optionType == Option.Type.Call ? forward * discount : 0.0);

			double d1 = Math.Log(forward / strike) / stdDev + 0.5 * stdDev;
			double d2 = d1 - stdDev;
			CumulativeNormalDistribution phi = new CumulativeNormalDistribution();
			double nd1 = phi.value((int)optionType * d1);
			double nd2 = phi.value((int)optionType * d2);
			double result = discount * (int)optionType * (forward * nd1 - strike * nd2);

			if (!(result >= 0.0))
				throw new ApplicationException("negative value (" + result + ") for " +
					  stdDev + " stdDev, " +
					  optionType + " option, " +
					  strike + " strike , " +
					  forward + " forward");

			return result;
		}

		/// <summary>
		/// Black 1976 formula for standard deviation derivative
		/// \warning instead of volatility it uses standard deviation, i.e.
		/// volatility*sqrt(timeToMaturity), and it returns the
		/// derivative with respect to the standard deviation.
		/// If T is the time to maturity Black vega would be
		/// blackStdDevDerivative(strike, forward, stdDev)*sqrt(T)
		/// </summary>
		public static double blackFormulaStdDevDerivative(double strike, double forward, double stdDev)
		{
			return blackFormulaStdDevDerivative(strike, forward, stdDev, 1.0, 0.0);
		}

		public static double blackFormulaStdDevDerivative(double strike, double forward, double stdDev, double discount)
		{
			return blackFormulaStdDevDerivative(strike, forward, stdDev, discount, 0.0);
		}

		public static double blackFormulaStdDevDerivative(double strike, double forward, double stdDev, double discount, double displacement)
		{
			checkParameters(strike, forward, displacement);

			if (stdDev < 0.0)
				throw new ArgumentException("stdDev (" + stdDev + ") must be non-negative");

			if (discount <= 0.0)
				throw new ArgumentException("discount (" + discount + ") must be positive");

			forward = forward + displacement;
			strike = strike + displacement;

			if (stdDev == 0.0)
			{
				if (forward > strike)
					return discount * forward;
				else
					return 0.0;
			}

			double d1 = Math.Log(forward / strike) / stdDev + .5 * stdDev;
			CumulativeNormalDistribution phi = new CumulativeNormalDistribution();
			return discount * forward * phi.derivative(d1);
		}

		/*! Black style formula when forward is normal rather than
				log-normal. This is essentially the model of Bachelier.

				\warning Bachelier model needs absolute volatility, not
						 percentage volatility. Standard deviation is
						 absoluteVolatility*sqrt(timeToMaturity)
		*/
		public static double bachelierBlackFormula(Option.Type optionType, double strike, double forward, double stdDev)
		{
			return bachelierBlackFormula(optionType, strike, forward, stdDev, 1);
		}

		public static double bachelierBlackFormula(Option.Type optionType, double strike, double forward, double stdDev, double discount)
		{
			if (stdDev < 0.0)
				throw new ArgumentException("stdDev (" + stdDev + ") must be non-negative");

			if (discount <= 0.0)
				throw new ArgumentException("discount (" + discount + ") must be positive");

			double d = (forward - strike) * (int)optionType;
			double h = d / stdDev;

			if (stdDev == 0.0)
				return discount * Math.Max(d, 0.0);

			CumulativeNormalDistribution phi = new CumulativeNormalDistribution();
			double result = discount * (stdDev * phi.derivative(h) + d * phi.value(h));

			if (!(result >= 0.0))
				throw new ApplicationException("negative value (" + result + ") for " +
					  stdDev + " stdDev, " +
					  optionType + " option, " +
					  strike + " strike , " +
					  forward + " forward");

			return result;
		}

		public static double bachelierBlackFormula(PlainVanillaPayoff payoff, double forward, double stdDev, double discount)
		{
			return bachelierBlackFormula(payoff.optionType(), payoff.strike(), forward, stdDev, discount);
		}

		//! default theta calculation for Black-Scholes options
		public static double blackScholesTheta(GeneralizedBlackScholesProcess p, double value, double delta, double gamma)
		{

			double u = p.stateVariable().currentLink().value();
			double r = p.riskFreeRate().currentLink().zeroRate(0.0, Compounding.Continuous).rate();
			double q = p.dividendYield().currentLink().zeroRate(0.0, Compounding.Continuous).rate();
			double v = p.localVolatility().currentLink().localVol(0.0, u, false);

			return r * value - (r - q) * u * delta - 0.5 * v * v * u * u * gamma;
		}

		//! default theta-per-day calculation
		public static double defaultThetaPerDay(double theta)
		{
			return theta / 365.0;
		}

		//! utility function giving the inflation period for a given date
		public static KeyValuePair<Date, Date> inflationPeriod(Date d, Frequency frequency)
		{
			Month month = (Month)d.Month;
			int year = d.Year;

			Month startMonth;
			Month endMonth;
			switch (frequency)
			{
				case Frequency.Annual:
					startMonth = Month.January;
					endMonth = Month.December;
					break;
				case Frequency.Semiannual:
					startMonth = (Month)(6 * ((int)month - 1) / 6 + 1);
					endMonth = (Month)(startMonth + 5);
					break;
				case Frequency.Quarterly:
					startMonth = (Month)(3 * ((int)month - 1) / 3 + 1);
					endMonth = (Month)(startMonth + 2);
					break;
				case Frequency.Monthly:
					startMonth = endMonth = month;
					break;
				default:
					throw new ApplicationException("Frequency not handled: " + frequency);
			}

			Date startDate = new Date(1, startMonth, year);
			Date endDate = Date.endOfMonth(new Date(1, endMonth, year));

			return new KeyValuePair<Date, Date>(startDate, endDate);
		}

		public static double inflationYearFraction(Frequency f, bool indexIsInterpolated,
									 DayCounter dayCounter,
									 Date d1, Date d2)
		{
			double t = 0;
			if (indexIsInterpolated)
			{
				// N.B. we do not use linear interpolation between flat
				// fixing forecasts for forecasts.  This avoids awkwardnesses
				// when bootstrapping the inflation curve.
				t = dayCounter.yearFraction(d1, d2);
			}
			else
			{
				// I.e. fixing is constant for the whole inflation period.
				// Use the value for half way along the period.
				// But the inflation time is the time between period starts
				KeyValuePair<Date, Date> limD1 = inflationPeriod(d1, f);
				KeyValuePair<Date, Date> limD2 = inflationPeriod(d2, f);
				t = dayCounter.yearFraction(limD1.Key, limD2.Key);
			}
			return t;
		}

		public static BusinessDayConvention liborConvention(Period p)
		{
			switch (p.units())
			{
				case TimeUnit.Days:
				case TimeUnit.Weeks:
					return BusinessDayConvention.Following;
				case TimeUnit.Months:
				case TimeUnit.Years:
					return BusinessDayConvention.ModifiedFollowing;
				default:
					throw new ApplicationException("invalid time units");
			}
		}

		public static bool liborEOM(Period p)
		{
			switch (p.units())
			{
				case TimeUnit.Days:
				case TimeUnit.Weeks:
					return false;
				case TimeUnit.Months:
				case TimeUnit.Years:
					return true;
				default:
					throw new ApplicationException("invalid time units");
			}
		}
	}
}
