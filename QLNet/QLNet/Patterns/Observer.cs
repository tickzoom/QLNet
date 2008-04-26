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
    public delegate void Callback();

    // Subjects, i.e. observables, should define interface internally like follows.
    //public event Callback notifyObserversEvent;
    //// this method is required for calling from derived classes
    //protected void notifyObservers() {
    //    Callback handler = notifyObserversEvent;
    //    if (handler != null) {
    //        handler();
    //    }
    //}
    //public void registerWith(Callback handler) { notifyObserversEvent += handler; }
    //public void unregisterWith(Callback handler) { notifyObserversEvent -= handler; }

    public interface IObservable {
        event Callback notifyObserversEvent;
        //void notifyObservers();
        void registerWith(Callback handler);
        void unregisterWith(Callback handler);
    }

    public interface IObserver {
        void update();
    }
}
