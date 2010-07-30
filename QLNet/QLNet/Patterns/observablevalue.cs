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

namespace QLNet.Patterns
{
	//! %observable and assignable proxy to concrete value
	/*! Observers can be registered with instances of this class so
		that they are notified when a different value is assigned to
		such instances. Client code can copy the contained value or
		pass it to functions via implicit conversion.
		\note it is not possible to call non-const method on the
			  returned value. This is by design, as this possibility
			  would necessarily bypass the notification code; client
			  code should modify the value via re-assignment instead.
	*/
	public class ObservableValue<T> : DefaultObservable
		where T : new()
	{
		private T _value;

		public ObservableValue()
		{
			_value = new T();
		}

		public ObservableValue(T t)
		{
			_value = t;
		}

		public ObservableValue(ObservableValue<T> t)
		{
			_value = t._value;
		}

		public ObservableValue<T> Assign(T t)
		{
			_value = t;
			notifyObservers();
			return this;
		}

		public ObservableValue<T> Assign(ObservableValue<T> t)
		{
			_value = t._value;
			notifyObservers();
			return this;
		}

		[Obsolete("Use Value property instead.")]
		public T value()
		{
			return _value;
		}

		public T Value
		{
			get { return _value; }
		}

		public static implicit operator T(ObservableValue<T> observableValue)
		{
			return observableValue._value;
		}
	}
}
