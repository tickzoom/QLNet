/*
 This file is part of QLNet Project http://trac2.assembla.com/QLNet
 
 QLNet is a porting of QuantLib, a free-software/open-source library
 for financial quantitative analysts and developers - http://quantlib.org/
 The license is available online at http://quantlib.org/license.shtml.
 
 This program is distributed in the hope that it will be useful, but WITHOUT
 ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 FOR A PARTICULAR PURPOSE.  See the license for more details.
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace QLNet
{
   public class Period
   {
      private int _length;
      private TimeUnit _units;

      public int length
      { 
         get
         {
            return _length; 
         }
      }
      public TimeUnit units 
      { 
         get 
         {
            return _units; 
         }
      }
       
      public Period()
      {
         _length = 0;
         _units = TimeUnit.Days;
      }
      public Period(int n, TimeUnit units)
      {
         _length = n;
         _units = units;

      }


      public Period(Frequency f)
      {
         switch (f)
         {
            case Frequency.Once:
            case Frequency.NoFrequency:
               // same as Period()
               _units = TimeUnit.Days;
               _length = 0;
               break;
            case Frequency.Annual:
               _units = TimeUnit.Years;
               _length = 1;
               break;
            case Frequency.Semiannual:
            case Frequency.EveryFourthMonth:
            case Frequency.Quarterly:
            case Frequency.Bimonthly:
            case Frequency.Monthly:
               _units = TimeUnit.Months;
               _length = 12 / (int)f;
               break;
            case Frequency.Biweekly:
            case Frequency.Weekly:
               _units = TimeUnit.Weeks;
               _length = 52 / (int)f;
               break;
            case Frequency.Daily:
               _units = TimeUnit.Days;
               _length = 1;
               break;
            default:
               throw new Exception ("unknown frequency (" + (int)(f));
         }

      }
      //Frequency frequency() const;
   
      public static Period operator-(Period p) 
      {
         return new Period(-p.length,p.units);
      }
      public static Period operator*(int n, Period p) 
      {
         return new Period(n*p.length,p.units);
      }
      public static Period operator*(Period p, int n) 
      {
         return new Period(n*p.length,p.units);
      }
      public static bool operator==(Period p1, Period p2) 
      {
         return !(p1 < p2 || p2 < p1);
      }
      public static bool operator!=(Period p1,Period p2) 
      {
         return !(p1 == p2);
      }
      public static bool operator>(Period p1,Period p2) 
      {
         return p2 < p1;
      }
      public static bool operator<=(Period p1,Period p2) 
      {
         return !(p1 > p2);
      }
      public static bool operator>=(Period p1,Period p2) 
      {
         return !(p1 < p2);
      }
      public static bool operator<(Period p1,Period p2) 
      {

      // special cases
      if (p1.length == 0)
         return p2.length > 0;
      if (p2.length == 0)
         return p1.length < 0;

      // exact comparisons
      if (p1.units == p2.units)
         return p1.length < p2.length;
      if (p1.units == TimeUnit.Months && p2.units == TimeUnit.Years)
         return p1.length < 12*p2.length;
      if (p1.units == TimeUnit.Years && p2.units == TimeUnit.Months)
         return 12*p1.length < p2.length;
      if (p1.units == TimeUnit.Days && p2.units == TimeUnit.Weeks)
         return p1.length < 7*p2.length;
      if (p1.units == TimeUnit.Weeks && p2.units == TimeUnit.Days)
         return 7*p1.length < p2.length;

      // inexact comparisons (handled by converting to days and using limits)
      make_pair<int, int> p1lim = daysMinMax(p1);
      make_pair<int, int> p2lim = daysMinMax(p2);

      if (p1lim.second < p2lim.first)
         return true;
      else if (p1lim.first > p2lim.second)
         return false;
      else
         throw new Exception("undecidable comparison between " + p1 + " and " + p2);
    }
      public static Period operator+(Period t, Period p) 
      {

        if (t._length == 0) 
        {
            t._length = p.length;
            t._units = p.units;
        } 
        else if (t._units==p.units) 
        {
            // no conversion needed
            t._length += p.length;
        } 
        else 
        {
            switch (t._units) 
            {

               case TimeUnit.Years:
                switch (p.units) 
                {
                   case TimeUnit.Months:
                      t._units = TimeUnit.Months;
                    t._length = t._length*12 + p.length;
                    break;
                 case TimeUnit.Weeks:
                 case TimeUnit.Days:
                      if ( p.length != 0 ) 
                      { 
                        throw new Exception ("impossible addition between " + t + " and " + p);
                      }
                    break;
                  default:
                    throw new Exception ("unknown units");
                }
                break;

             case TimeUnit.Months:
                switch (p.units) 
                {
                   case TimeUnit.Years:
                    t._length += p.length*12;
                    break;
                 case TimeUnit.Weeks:
                 case TimeUnit.Days:
                    if (p.length != 0)
                    {
                       throw new Exception("impossible addition between " + t + " and " + p);
                    }
                    break;
                  default:
                    throw new Exception ("unknown units");
                }
                break;

             case TimeUnit.Weeks:
                switch (p.units) 
                {
                   case TimeUnit.Days:
                      t._units = TimeUnit.Days;
                    t._length = t._length*7 + p.length;
                    break;
                 case TimeUnit.Years:
                 case TimeUnit.Months:
                    if (p.length != 0)
                    {
                       throw new Exception("impossible addition between " + t + " and " + p);
                    }
                    break;
                  default:
                    throw new Exception ("unknown units");
                }
                break;

             case TimeUnit.Days:
                switch (p.units) 
                {
                   case TimeUnit.Weeks:
                    t._length += p.length*7;
                    break;
                 case TimeUnit.Years:
                 case TimeUnit.Months:
                    if (p.length != 0)
                    {
                       throw new Exception("impossible addition between " + t + " and " + p);
                    }
                    break;
                  default:
                    throw new Exception ("unknown units");
                }
                break;

              default:
                throw new Exception ("unknown units");
            }
        }

        //this->normalize();
        return t;
    }
      public static Period operator-(Period t, Period p) 
      {
        return t + (-p);
      }
      public static Period operator/(Period t, int n) 
      {
         if (n == 0)
         {
            throw new Exception("cannot be divided by zero"); 
         }

         if (t._length % n == 0) 
         {
            // keep the original units. If the user created a
            // 24-months period, he'll probably want a 12-months one
            // when he halves it.
            t._length /= n;
         } 
         else 
         {
            // try
            TimeUnit units = t._units;
            int length = t._length;
            switch (units) {
               case TimeUnit.Years:
                length *= 12;
                units = TimeUnit.Months;
                break;
             case TimeUnit.Weeks:
                length *= 7;
                units = TimeUnit.Days;
                break;
                
            }
            if (length % n != 0)
            {
              throw new Exception (t + " cannot be divided by " + n);
            }

            t._length = length/n;
            t._units = units;
            // if normalization were possible, we wouldn't be
            // here---the "if" branch would have been executed
            // instead.
            // result.normalize();
        }
        return t;
    }



      public static make_pair<int, int> daysMinMax(Period p) 
      {
         switch (p.units) 
         {
            case TimeUnit.Days:
               return new make_pair<int, int>(p.length, p.length);
            case TimeUnit.Weeks:
               return new make_pair<int, int>(7*p.length, 7*p.length);
            case TimeUnit.Months:
               return new make_pair<int, int>(28*p.length, 31*p.length);
            case TimeUnit.Years:
               return new make_pair<int, int>(365 * p.length, 366 * p.length);
            default:
               throw new Exception ("Unknown units");
         }
      }

      public void normalize() 
      {
         switch (_units) 
         {
            case TimeUnit.Days:
               if ((_length%7) == 0) 
               {
                  _length/=7;
                  _units = TimeUnit.Weeks;
               }
               break;
            case TimeUnit.Months:
               if ((_length%12) == 0) 
               {
                  _length/=12;
                  _units = TimeUnit.Years;
               }
               break;
            case TimeUnit.Weeks:
            case TimeUnit.Years:
               break;
            default:
               throw new Exception ("unknown time unit (" + (int)_units);
        }
      }
      public Frequency frequency() 
      {
         // unsigned version
         int length = Math.Abs (_length);

         if (length==0) return  Frequency.NoFrequency;

         switch (_units) {
            case TimeUnit.Years:
               if (length != 1 )
                  throw new Exception ("cannot instantiate a Frequency from " + this);
               return Frequency.Annual;
            case TimeUnit.Months:
               if ((12%length)!=0 || length > 12)
                  throw new Exception ("cannot instantiate a Frequency from " + this);
               return (Frequency)(12/length);
         case TimeUnit.Weeks:
            if (length==1)
               return Frequency.Weekly;
            else if (length==2)
               return Frequency.Biweekly;
            else
                throw new Exception ("cannot instantiate a Frequency from " +this);
          case TimeUnit.Days:
            if (length != 1)
               throw new Exception("cannot instantiate a Frequency from " + this);
            return Frequency.Daily;
          default:
             throw new Exception("unknown time unit (" + (int)(_units));
        }
    }



   }
}
