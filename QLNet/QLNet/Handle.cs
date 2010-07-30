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
using System.Reflection;
using QLNet.Patterns;

namespace QLNet
{
	//! Shared handle to an observable
	/*! All copies of an instance of this class refer to the same observable by means of a relinkable smart pointer. When such
		pointer is relinked to another observable, the change will be propagated to all the copies.
		<tt>registerAsObserver</tt> is not needed since C# does automatic garbage collection */
	public class Handle<T>
		where T : IObservable, new()
	{
		protected readonly Link _link;

		public Handle() 
			: this(new T())
		{
		}

		public Handle(T h) 
			: this(h, true)
		{
		}
		
		public Handle(T h, bool registerAsObserver)
		{
			_link = new Link(h, registerAsObserver);
		}

		public T currentLink()
		{
			return link;
		}

		public T link
		{
			get
			{
				if (IsEmpty)
				{
					throw new ApplicationException("empty Handle cannot be dereferenced");
				}

				return _link.CurrentLink;
			}
		}

		public void registerWith(Action handler)
		{
			_link.registerWith(handler);
		}

		public void unregisterWith(Action handler)
		{
			_link.unregisterWith(handler);
		}

		[Obsolete("Use IsEmpty property instead.")]
		public bool empty()
		{
			return IsEmpty;
		}

		/// <summary>
		/// Checks if the contained shared pointer points to anything
		/// </summary>
		/// <returns></returns>
		public bool IsEmpty
		{
			get { return _link.IsEmpty; }
		}

		public static bool operator ==(Handle<T> here, Handle<T> there)
		{
			return Equals(here, there);
		}

		public static bool operator !=(Handle<T> here, Handle<T> there)
		{
			return !Equals(here, there);
		}

		public bool Equals(Handle<T> other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other._link, _link);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(Handle<T>)) return false;
			return Equals((Handle<T>)obj);
		}

		public override int GetHashCode()
		{
			return (_link != null ? _link.GetHashCode() : 0);
		}

		public static implicit operator T(Handle<T> handle)
		{
			return handle.link;
		}

		protected class Link : DefaultObservable, IObserver
		{
			private T _h;
			private bool _isObserver;

			public Link(T h, bool registerAsObserver)
			{
				LinkTo(h, registerAsObserver);
			}

			public void LinkTo(T h, bool registerAsObserver)
			{
				if (!h.Equals(_h) || (_isObserver != registerAsObserver))
				{

					if (_h != null && _isObserver)
					{
						_h.unregisterWith(update);
					}

					_h = h;
					_isObserver = registerAsObserver;

					if (_h != null && _isObserver)
					{
						_h.registerWith(update);
					}

					// finally, notify observers of this of the change in the underlying object
					notifyObservers();
				}
			}

			public bool IsEmpty
			{
				get { return _h == null; }
			}

			public T CurrentLink
			{
				get { return _h; }
			}

			public void update()
			{
				notifyObservers();
			}
		}
	}

	/// <summary>
	/// Relinkable handle to an observable.
	/// 
	/// An instance of this class can be relinked so that it points to another observable. 
	/// The change will be propagated to all handles that were created as copies of such instance.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class RelinkableHandle<T> : Handle<T>
		where T : IObservable, new()
	{
		public RelinkableHandle()
			: base(new T(), true)
		{
		}

		public RelinkableHandle(T h)
			: base(h, true)
		{
		}

		public RelinkableHandle(T h, bool registerAsObserver) 
			: base(h, registerAsObserver)
		{
		}

		public void linkTo(T h)
		{
			linkTo(h, true);
		}

		public void linkTo(T h, bool registerAsObserver)
		{
			_link.LinkTo(h, registerAsObserver);
		}
	}
}
