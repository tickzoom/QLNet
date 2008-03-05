using System;
using System.Collections.Generic;
using System.Text;

namespace QLNet
{
   public class InterestRate
   {
      private Nullable<double> r_;
      private DayCounter dc_;
      private Compounding comp_;
      private bool freqMakesSense_;
      private double freq_;

      // Constructors

      /// <summary>
      /// Default constructor returning a null interest rate.
      /// </summary>
      public InterestRate()
      {
         r_ = new Nullable<double>();
      }

      /// <summary>
      /// Standard constructor
      /// </summary>
      /// <param name="r"></param>
      /// <param name="dc"></param>
      /// <param name="comp"></param>
      public InterestRate(double r) : this(r, new Actual365Fixed(), Compounding.Continuous, Frequency.Annual) { }
      public InterestRate(double r, DayCounter dc) : this(r, dc, Compounding.Continuous, Frequency.Annual) { }
      public InterestRate(double r, DayCounter dc, Compounding comp) : this(r, dc, comp, Frequency.Annual) { }
      public InterestRate(double r, DayCounter dc, Compounding comp, Frequency freq)
      {
         r_ = r;
         dc_ = dc;
         comp_ = comp;
         freqMakesSense_ = false;

         if (comp_ == Compounding.Compounded || comp_ == Compounding.SimpleThenCompounded)
         {
            freqMakesSense_ = true;
            if ( freq == Frequency.Once || freq == Frequency.NoFrequency)
               throw new Exception ("frequency not allowed for this interest rate");
            freq_ = (double)freq;
         }
      }

      public static implicit operator double(InterestRate ImpliedObject)
      {
         return ImpliedObject.r_.Value;
      }

      public double rate()
      {
         return r_.Value;
      }
      public DayCounter dayCounter()
      {
         return dc_;
      }
      public Compounding compounding()
      {
         return comp_;
      }
      public Frequency frequency()
      {
         return freqMakesSense_ ? (Frequency)((int)freq_) : Frequency.NoFrequency;
      }

      /// <summary>
      /// discount/compound factor calculations
      /// discount factor implied by the rate compounded at time t.
      /// <remarks>
      /// Time must be measured using InterestRate's own
      /// day counter.
      /// </remarks> 
      /// </summary>
      /// <param name="t"></param>
      /// <returns></returns>
      public double discountFactor(double t)
      {
         return 1.0 / compoundFactor(t);
      }

      /// <summary>
      /// discount factor implied by the rate compounded between two dates
      /// </summary>
      /// <param name="d1"></param>
      /// <param name="d2"></param>
      /// <param name="refStart"></param>
      /// <returns></returns>
      public double discountFactor(DDate d1, DDate d2, DDate refStart) 
      {
         return discountFactor(d1, d2, refStart, new DDate());
      }
      public double discountFactor(DDate d1, DDate d2)
      {
         return discountFactor(d1, d2, new DDate(), new DDate());
      }
      public double discountFactor(DDate d1, DDate d2, DDate refStart, DDate refEnd)
      {
         double t = dc_.yearFraction(d1, d2, refStart, refEnd);
         return discountFactor(t);
      }

      /// <summary>
      /// compound factor implied by the rate compounded at time t.
      /// returns the compound (a.k.a capitalization) factor
      /// implied by the rate compounded at time t.
      /// <remarks>
      /// Time must be measured using InterestRate's own
      /// day counter.
      /// </remarks> 
      /// </summary>
      /// <param name="t"></param>
      /// <returns></returns>
      public double compoundFactor(double t)
      {
         if ( t < 0 )
            throw new Exception ("negative time not allowed");
         if ( ! r_.HasValue )
            throw new Exception ("null interest rate");

         switch (comp_)
         {
            case Compounding.Simple:
               return 1.0 + r_.Value  * t;
            case Compounding.Compounded:
               return Math.Pow(1.0 + r_.Value  / freq_, freq_ * t);
            case Compounding.Continuous:
               return Math.Exp(r_.Value  * t);
            case Compounding.SimpleThenCompounded:
               if (t <= 1.0 / (double)freq_)
                  return 1.0 + r_.Value  * t;
               else
                  return Math.Pow(1.0 + r_.Value  / freq_, freq_ * t);
            default:
               throw new Exception ("unknown compounding convention");
         }
      }

      /// <summary>
      /// compound factor implied by the rate compounded between two dates
      /// returns the compound (a.k.a capitalization) factor
      /// implied by the rate compounded between two dates.
      /// </summary>
      /// <param name="d1"></param>
      /// <param name="d2"></param>
      /// <param name="refStart"></param>
      /// <returns></returns>
      public double compoundFactor(DDate d1, DDate d2, DDate refStart)
      {
         return compoundFactor(d1, d2, refStart, new DDate());
      }
      public double compoundFactor(DDate d1, DDate d2)
      {
         return compoundFactor(d1, d2, new DDate(), new DDate());
      }
      public double compoundFactor(DDate d1, DDate d2, DDate refStart, DDate refEnd)
      {
         double t = dc_.yearFraction(d1, d2, refStart, refEnd);
         return compoundFactor(t);
      }

      /// <summary>
      /// implied interest rate for a given compound factor at a given time.
      /// The resulting InterestRate has the day-counter provided as input.
      /// <remarks>
      /// Time must be measured using the day-counter provided
      /// as input.
      /// </remarks> 
      /// </summary>
      /// <param name="compound"></param>
      /// <param name="t"></param>
      /// <param name="resultDC"></param>
      /// <param name="comp"></param>
      /// <returns></returns>
      public static InterestRate impliedRate(double compound, double t, DayCounter resultDC, Compounding comp)
      {
         return impliedRate(compound, t, resultDC, comp, Frequency.Annual);
      }
      public static InterestRate impliedRate(double compound, double t, DayCounter resultDC, Compounding comp, Frequency freq)
      {

         if (compound <= 0)
            throw new Exception("positive compound factor required");
         if (t <= 0)
            throw new Exception("positive time required");

         double r;
         switch (comp)
         {
            case Compounding.Simple:
               r = (compound - 1.0) / t;
               break;
            case Compounding.Compounded:
               r = (Math.Pow(compound, 1.0 / ((double)freq * t)) - 1.0) * (double)freq;
               break;
            case Compounding.Continuous:
               r = Math.Log(compound) / t;
               break;
            case Compounding.SimpleThenCompounded:
               if (t <= 1.0 / (double)freq)
                  r = (compound - 1.0) / t;
               else
                  r = (Math.Pow(compound, 1.0 / ((double)freq * t)) - 1.0) * (double)freq;
               break;
            default:
               throw new Exception ("unknown compounding convention (" + (int)comp + ")");
         }
         return new InterestRate(r, resultDC, comp, freq);
      }

      /// <summary>
      /// implied rate for a given compound factor between two dates.
      /// The resulting rate is calculated taking the required
      /// day-counting rule into account.
      /// </summary>
      /// <param name="compound"></param>
      /// <param name="d1"></param>
      /// <param name="d2"></param>
      /// <param name="resultDC"></param>
      /// <param name="comp"></param>
      /// <returns></returns>
      public static InterestRate impliedRate(double compound, DDate d1, DDate d2, DayCounter resultDC, Compounding comp)
      {
         return impliedRate(compound, d1, d2, resultDC, comp, Frequency.Annual);
      }
      public static InterestRate impliedRate(double compound, DDate d1, DDate d2, DayCounter resultDC, Compounding comp, Frequency freq)
      {
         if (d2 <= d1) 
            throw new Exception ("d1 (" + d1 + ") " + "later than or equal to d2 (" + d2 + ")");
         double t = resultDC.yearFraction(d1, d2);
         return impliedRate(compound, t, resultDC, comp, freq);
      }

      /// <summary>
      /// equivalent interest rate for a compounding period t.
      /// The resulting InterestRate shares the same implicit
      /// day-counting rule of the original InterestRate instance.
      /// <remarks>
      /// Time must be measured using the InterestRate's
      /// own day counter.
      /// </remarks>
      /// </summary>
      /// <param name="t"></param>
      /// <param name="comp"></param>
      /// <returns></returns>
      public InterestRate equivalentRate(double t, Compounding comp)
      {
         return equivalentRate(t, comp, Frequency.Annual);
      }
      public InterestRate equivalentRate(double t, Compounding comp, Frequency freq)
      {
         return impliedRate(compoundFactor(t), t, dc_, comp, freq);
      }

      /// <summary>
      /// equivalent rate for a compounding period between two dates
      /// The resulting rate is calculated taking the required
      /// day-counting rule into account.
      /// </summary>
      /// <param name="d1"></param>
      /// <param name="d2"></param>
      /// <param name="resultDC"></param>
      /// <param name="comp"></param>
      /// <returns></returns>
      public InterestRate equivalentRate(DDate d1, DDate d2, DayCounter resultDC, Compounding comp)
      {
         return equivalentRate(d1, d2, resultDC, comp, Annual);
      }
      public InterestRate equivalentRate(DDate d1, DDate d2, DayCounter resultDC, Compounding comp, Frequency freq)
      {
         if ( d2 <= d1 )
            throw new Exception ("d1 (" + d1 + ") " + "later than or equal to d2 (" + d2 + ")");

            double t1 = dc_.yearFraction(d1, d2);
            double t2 = resultDC.yearFraction(d1, d2);
            return impliedRate(compoundFactor(t1), t2, resultDC, comp, freq);
        }

   }
}
