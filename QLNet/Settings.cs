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
using System.Text;
using QLNet;

namespace QLNet
{
    // we need only one instance of the class
    // we can not derive it from IObservable because the class is static
    public static class Settings
    {
        private static Date evaluationDate_ = Date.Today;
        private static bool enforcesTodaysHistoricFixings_ = false;

        public static Date evaluationDate() { return evaluationDate_; }
        public static void setEvaluationDate(Date d)
        {
            evaluationDate_ = d;
            notifyObservers();
        }

        public static bool enforcesTodaysHistoricFixings
        {
            get { return enforcesTodaysHistoricFixings_; }
            set { enforcesTodaysHistoricFixings_ = value; }
        }

        ////////////////////////////////////////////////////
        // Observable interface
        private static event Callback notifyObserversEvent;
        public static void registerWith(Callback handler) { notifyObserversEvent += handler; }
        public static void unregisterWith(Callback handler) { notifyObserversEvent -= handler; }
        private static void notifyObservers()
        {
            Callback handler = notifyObserversEvent;
            if (handler != null)
            {
                handler();
            }
        }
    }
}