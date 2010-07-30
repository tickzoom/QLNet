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

namespace QLNet
{
	/// <summary>
	/// Amount of cash
	/// Money arithmetic is tested with and without currency conversions.
	/// </summary>
	public class Money
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

		#region Attributes

		[ThreadStatic]
		public static ConversionType conversionType;

		[ThreadStatic]
		public static Currency baseCurrency;

		private double value_;
		private Currency currency_;

		#endregion

		#region Constructor

		public Money()
		{
			value_ = 0.0;
		}

		public Money(Currency currency, double value)
		{
			value_ = value;
			currency_ = currency;
		}

		public Money(double value, Currency currency)
			: this(currency, value)
		{
		}

		#endregion

		#region Get/Set

		public Currency currency
		{
			get { return currency_; }
		}

		public double value
		{
			get { return value_; }
		}

		#endregion

		#region Methods

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
			if (baseCurrency.empty())
			{
				throw new Exception("no base currency set");
			}

			convertTo(ref m, baseCurrency);
		}

		public Money rounded()
		{
			return new Money(currency_.rounding.Round(value_), currency_);
		}

		public override String ToString()
		{
			return rounded().value + "-" + currency.code + "-" + currency.symbol;
		}

		#endregion

		public static Money operator *(Money m, double x)
		{
			return new Money(m.value_ * x, m.currency);
		}

		public static Money operator *(double x, Money m)
		{
			return m * x;
		}

		public static Money operator /(Money m, double x)
		{
			return new Money(m.value_ / x, m.currency);
		}

		public static Money operator +(Money m1, Money m2)
		{
			Money m = new Money(m1.currency, m1.value);

			if (m1.currency_ == m2.currency_)
			{
				m.value_ += m2.value_;
			}
			else if (conversionType == ConversionType.BaseCurrencyConversion)
			{
				convertToBase(ref m);
				Money tmp = m2;
				convertToBase(ref tmp);
				m += tmp;
			}
			else if (conversionType == ConversionType.AutomatedConversion)
			{
				Money tmp = m2;
				convertTo(ref tmp, m.currency_);
				m += tmp;
			}
			else
			{
				throw new Exception("currency mismatch and no conversion specified");
			}

			return m;
		}

		public static Money operator -(Money m1, Money m2)
		{
			Money m = new Money(m1.currency, m1.value);

			if (m.currency_ == m2.currency_)
			{
				m.value_ -= m2.value_;
			}
			else if (conversionType == ConversionType.BaseCurrencyConversion)
			{
				convertToBase(ref m);
				Money tmp = m2;
				convertToBase(ref tmp);
				m -= tmp;
			}
			else if (conversionType == ConversionType.AutomatedConversion)
			{
				Money tmp = m2;
				convertTo(ref tmp, m.currency_);
				m -= tmp;
			}
			else
			{
				throw new Exception("currency mismatch and no conversion specified");
			}

			return m;
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
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;

			if (currency_ == other.currency_)
				return value_ == other.value_;

			if (conversionType == Money.ConversionType.BaseCurrencyConversion)
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
				convertTo(ref tmp, currency_);
				return this == tmp;
			}

			throw new Exception("currency mismatch and no conversion specified"); // QA:[RG]::verified // TODO: message
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(Money)) return false;
			return Equals((Money)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (value_.GetHashCode() * 397) ^ (currency_ != null ? currency_.GetHashCode() : 0);
			}
		}
	}
}
