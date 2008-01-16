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
   public interface IObservable
   {
      void notifyObservers();
      void registerObserver(IObserver o);
      void unregisterObserver(IObserver o);
   }

   public interface IObserver
   {
      void registerWith(IObservable o);
      void unregisterWith(IObservable o);
      void update();
   }

   public class Observable : IObservable
   {

      private List<IObserver> _observers = new List<IObserver>();

      public Observable()
      {
      }
      public Observable(Observable o) 
      {
         // the observer list is not copied; no observer asked to
         // register with this object
      }

      #region Membri di IObservable

      public void notifyObservers()
      {
         bool successful = true;
         foreach ( IObserver i in _observers)
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
            if ( !successful ) throw new Exception ("could not notify one or more observers");
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
   }

   public class Observer : IObserver
   {
      private List<IObservable> _observables = new List<IObservable>();
      public Observer() { }

      public Observer(Observer o)  
      {
          _observables = o._observables;

          foreach (IObservable i in _observables)
            i.registerObserver(this);
      }

      ~ Observer()
      {
         foreach (IObservable i in _observables)
            i.unregisterObserver(this);
	   }

      #region Membri di IObserver

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
         if (o != null )
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

      public virtual void update()
      {
         throw new Exception("The method or operation is not implemented.");
      }

      #endregion
   }

   public class ObservableValue<T> : Observable 
   {
      private T value_;

      public ObservableValue(T t)
      {
         value_ = t;
      }

      public T value()
      {
         return value_ ;
      }

   }

   public class ObserverObservable : IObservable, IObserver
   {
      private List<IObserver> _observers = new List<IObserver>();
      private List<IObservable> _observables = new List<IObservable>();

      #region IObservable Membri di

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

      #region IObserver Membri di

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

      public virtual void update()
      {
         throw new Exception("The method or operation is not implemented.");
      }

      #endregion

      public ObserverObservable()
      {
      }

      public ObserverObservable(Observable o) 
      {
         // the observer list is not copied; no observer asked to
         // register with this object
      }

      public ObserverObservable(ObserverObservable o)  
      {
          _observables = o._observables;

          foreach (IObservable i in _observables)
            i.registerObserver(this);
      }
   }

}
