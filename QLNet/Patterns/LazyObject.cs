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
public class LazyObject : ObserverObservable
{
		protected bool _calculated;
		protected bool _frozen;

		// inline definitions
	
		public LazyObject()
		{
			_calculated = false;
			_frozen = false;
		}
		public void Dispose()
		{
		}
		//! \name Observer interface
		//@{
		public override void update()
		{
			// observers don't expect notifications from frozen objects
			// LazyObject forwards notifications only once until it has been 
			// recalculated
			if (!_frozen&& _calculated)
				notifyObservers();
			_calculated = false;
		}
		public void recalculate()
		{
			bool wasFrozen = _frozen;
			_calculated = _frozen = false;
			try
			{
				calculate();
			}
			catch
			{
				_frozen = wasFrozen;
				notifyObservers();
				throw;
			}
			_frozen = wasFrozen;
			notifyObservers();
		}
		public void freeze()
		{
			_frozen = true;
		}
		public void unfreeze()
		{
			_frozen = false;
			// send notification, just in case we lost any
			notifyObservers();
		}
		protected void calculate()
		{
			if (!_calculated && !_frozen)
			{
				_calculated = true; // prevent infinite recursion in
									  // case of bootstrapping
				try
				{
					performCalculations();
				}
				catch
				{
					_calculated = false;
					throw;
				}
			}
		}
//        ! This method must implement any calculations which must be
//            (re)done in order to calculate the desired results.
//        
		protected virtual void performCalculations(){throw new Exception("The method or operation is not implemented.");} 
		//@}
	}
}
