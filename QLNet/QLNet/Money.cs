/*
 Copyright (C) 2008 Andrea Maggiulli
 Copyright (C) 2008 Toyin Akin (toyin_akin@hotmail.com)
 
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
using QLNet.Currencies;

namespace QLNet
{
	/// <summary>
	/// Amount of cash
	/// Money arithmetic is tested with and without currency conversions.
	/// </summary>
	public struct Money
	{
		#region Define

		public enum ConversionType
		{
			/// <summary>
			/// do not perform conversions
			/// </summary>
			NoConversion,

			/// <summary>
			/// convert both operands to the base currency before converting
			/// </summary>
			BaseCurrencyConversion,

			/// <summary>
			/// return the result in the currency of the first operand
			/// </summary>
			AutomatedConversion
		}

		#endregion

		[ThreadStatic]
		public static ConversionType conversionType;

		[ThreadStatic]
		public static Currency BaseCurrency;

		private double _value;
		private readonly Currency _currency;

		public Money(double value, Currency currency)
			: this(currency, value)
		{
		}

		public Money(Currency currency, double value)
		{
			_value = value;
			_currency = currency;
		}

		#region Get/Set

		public Currency currency
		{
			get { return _currency; }
		}

		public double value
		{
			get { return _value; }
		}

		#endregion

		public static void convertTo(ref Money m, Currency target)
		{
			if (m.currency != target)
			{
				ExchangeRate rate = ExchangeRateManager.Instance.lookup(m.currency, target);
				m = rate.exchange(m).rounded();
			}
		}

		public static void convertToBase(ref Money m)
		{
			if (BaseCurrency.IsEmpty)
			{
				throw new Exception("no base currency set");
			}

			convertTo(ref m, BaseCurrency);
		}

		public Money rounded()
		{
			return new Money(_currency.rounding.Round(_value), _currency);
		}

		public override string ToString()
		{
			return rounded().value + "-" + currency.code + "-" + currency.symbol;
		}

		public static Money operator *(Money m, double x)
		{
			return new Money(m._value * x, m.currency);
		}

		public static Money operator *(double x, Money m)
		{
			return m * x;
		}

		public static Money operator /(Money m, double x)
		{
			return new Money(m._value / x, m.currency);
		}

		public static Money operator +(Money m1, Money m2)
		{
			Money newInstance = new Money(m1.currency, m1.value);

			if (m1._currency == m2._currency)
			{
				newInstance._value += m2._value;
			}
			else if (conversionType == ConversionType.BaseCurrencyConversion)
			{
				convertToBase(ref newInstance);
				Money tmp = (Money)m2.MemberwiseClone();
				convertToBase(ref tmp);
				newInstance += tmp;
			}
			else if (conversionType == ConversionType.AutomatedConversion)
			{
				Money tmp = (Money)m2.MemberwiseClone();
				convertTo(ref tmp, newInstance._currency);
				newInstance += tmp;
			}
			else
			{
				throw new Exception("currency mismatch and no conversion specified");
			}

			return newInstance;
		}

		public static Money operator -(Money m1, Money m2)
		{
			Money newInstance = new Money(m1.currency, m1.value);

			if (newInstance._currency == m2._currency)
			{
				newInstance._value -= m2._value;
			}
			else if (conversionType == ConversionType.BaseCurrencyConversion)
			{
				convertToBase(ref newInstance);
				Money tmp = (Money)m2.MemberwiseClone();
				convertToBase(ref tmp);
				newInstance -= tmp;
			}
			else if (conversionType == ConversionType.AutomatedConversion)
			{
				Money tmp = (Money)m2.MemberwiseClone();
				convertTo(ref tmp, newInstance._currency);
				newInstance -= tmp;
			}
			else
			{
				throw new Exception("currency mismatch and no conversion specified");
			}

			return newInstance;
		}

		public static bool operator ==(Money m1, Money m2)
		{
			return Equals(m1, m2);
		}

		public static bool operator !=(Money m1, Money m2)
		{
			return !Equals(m1, m2);
		}

		public bool Equals(Money other)
		{
			if (_currency == other._currency)
			{
				return _value == other._value;
			}

			if (conversionType == ConversionType.BaseCurrencyConversion)
			{
				Money tmp1 = (Money)MemberwiseClone();
				convertToBase(ref tmp1);
				Money tmp2 = (Money)other.MemberwiseClone();
				convertToBase(ref tmp2);
				// recursive...
				return tmp1 == tmp2;
			}

			if (conversionType == ConversionType.AutomatedConversion)
			{
				Money tmp = (Money)other.MemberwiseClone();
				convertTo(ref tmp, _currency);
				return this == tmp;
			}

			throw new Exception("currency mismatch and no conversion specified"); // QA:[RG]::verified // TODO: message
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (obj.GetType() != typeof (Money)) return false;
			return Equals((Money) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (_value.GetHashCode()*397) ^ (_currency != null ? _currency.GetHashCode() : 0);
			}
		}
	}
}
