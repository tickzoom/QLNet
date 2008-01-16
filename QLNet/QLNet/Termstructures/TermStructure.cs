/*
 Copyright (C) 2008 Andrea Maggiulli
  
 This file is part of QLNet Project http://trac2.assembla.com/QLNet

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
using System.Text;

namespace QLNet
{
   public class TermStructure : Extrapolator ,IObservable ,IObserver
   {

      #region Membri di IObservable

      private List<IObserver> _observers = new List<IObserver>();

      public void notifyObservers()
      {
         bool successful = true;
         foreach (IObserver i in _observers)
         {
            try
            {
               i.update();
            }
            catch (Exception e)
            {
               // quite a dilemma. If we don't catch the exception,
               // other observers will not receive the notification
               // and might be left in an incorrect state. If we do
               // catch it and continue the loop (as we do here) we
               // lose the exception. The least evil might be to try
               // and notify all observers, while raising an
               // exception if something bad happened.
               successful = false;
            }
            if (!successful) throw new Exception("could not notify one or more observers");
         }
      }

      public void registerObserver(IObserver o)
      {
         _observers.Add(o);
      }

      public void unregisterObserver(IObserver o)
      {
         _observers.Remove(o);
      }

      #endregion
      #region Membri di IObserver
      private List<IObservable> _observables = new List<IObservable>();

      public void registerWith(IObservable o)
      {
         if (o != null)
         {
            _observables.Add(o);
            o.registerObserver(this);
         }
      }

      public void unregisterWith(IObservable o)
      {
         if (o != null)
         {
            foreach (IObservable i in _observables)
            {
               if (i == o)
               {
                  i.unregisterObserver(this);
                  _observables.Remove(i);
                  return;
               }
            }
         }
      }

      public void update()
      {
         if (_moving)
            _updated = false;
         notifyObservers();
      }

      #endregion

      protected bool _moving;
      protected Calendar _calendar;
      private DDate _referenceDate;
      private bool _updated;
      private int _settlementDays;
      private DayCounter _dayCounter;
   
      DayCounter dayCounter()  {return _dayCounter;}
      public virtual double maxTime()  {return timeFromReference(maxDate());}
      public virtual Calendar calendar() { return _calendar; }
      public virtual Nullable <int> settlementDays()  {return _settlementDays;}
      protected double timeFromReference(DDate d)  {return dayCounter().yearFraction(referenceDate(),d);}
      protected void checkRange(DDate d,bool extrapolate) { checkRange(timeFromReference(d), extrapolate);}
      protected void checkRange(double t, bool extrapolate)
      {
         if (t < 0.0)
            throw new Exception("negative time (" + t + ") given");
         
         if (!extrapolate && !allowsExtrapolation() && t > maxTime())
            throw new Exception("time (" + t + ") is past max curve time (" + maxTime() + ")");
      }


      public TermStructure() : this(new DayCounter()){}

      public TermStructure(DayCounter dc)
      {
	      _moving = false;
	      _updated = true;
	      _settlementDays = 0;
	      _dayCounter = dc;
      }
      public TermStructure(DDate referenceDate, Calendar cal, DayCounter dc)
      {
	      _moving = false;
	      _referenceDate = referenceDate;
	      _updated = true;
         _settlementDays = 0;
	      _calendar = cal;
	      _dayCounter = dc;
      }
      public TermStructure(int settlementDays, Calendar cal, DayCounter dc)
      {
	      _moving = true;
	      _updated = false;
	      _settlementDays = settlementDays;
	      _calendar = cal;
	      _dayCounter = dc;
         
         this.registerWith(Settings.Instance.evaluationDate());
	      // verify immediately if calendar and settlementDays are ok
	      DDate today = Settings.Instance.evaluationDate();
         _referenceDate = calendar().advance(today, _settlementDays, TimeUnit.Days);
      }

      public virtual DDate maxDate() {throw new Exception ("Not implemtend");}

      public virtual DDate referenceDate() 
      {
        if (!_updated) 
        {
            DDate today = Settings.Instance.evaluationDate();
            _referenceDate = calendar().advance(today, _settlementDays, TimeUnit.Days);
            _updated = true;
        }
        return _referenceDate;
      }

   }
}
