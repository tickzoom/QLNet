/*
 Copyright (C) 2008 Andrea Maggiulli
 Copyright (C) 2008 Siarhei Novik (snovik@gmail.com)
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
using System.Collections.Generic;

namespace QLNet.Currencies
{
	/// <summary>
	/// exchange-rate repository
	/// test lookup of direct, triangulated, and derived exchange rates is tested
	/// </summary>
	public class ExchangeRateManager
	{
		[ThreadStatic]
		private static ExchangeRateManager _instance;

		public static ExchangeRateManager Instance
		{
			get { return _instance ?? (_instance = new ExchangeRateManager()); }
		}

		private ExchangeRateManager()
		{
			AddKnownRates();
		}

		private readonly Dictionary<int, List<ExchangeRateManagerEntry>> _data = new Dictionary<int, List<ExchangeRateManagerEntry>>();

		private void AddKnownRates()
		{
			// currencies obsoleted by Euro
			Add(new ExchangeRate(new EURCurrency(), new ATSCurrency(), 13.7603), new Date(1, Month.January, 1999), Date.maxDate());
			Add(new ExchangeRate(new EURCurrency(), new BEFCurrency(), 40.3399), new Date(1, Month.January, 1999), Date.maxDate());
			Add(new ExchangeRate(new EURCurrency(), new DEMCurrency(), 1.95583), new Date(1, Month.January, 1999), Date.maxDate());
			Add(new ExchangeRate(new EURCurrency(), new ESPCurrency(), 166.386), new Date(1, Month.January, 1999), Date.maxDate());
			Add(new ExchangeRate(new EURCurrency(), new FIMCurrency(), 5.94573), new Date(1, Month.January, 1999), Date.maxDate());
			Add(new ExchangeRate(new EURCurrency(), new FRFCurrency(), 6.55957), new Date(1, Month.January, 1999), Date.maxDate());
			Add(new ExchangeRate(new EURCurrency(), new GRDCurrency(), 340.750), new Date(1, Month.January, 2001), Date.maxDate());
			Add(new ExchangeRate(new EURCurrency(), new IEPCurrency(), 0.787564), new Date(1, Month.January, 1999), Date.maxDate());
			Add(new ExchangeRate(new EURCurrency(), new ITLCurrency(), 1936.27), new Date(1, Month.January, 1999), Date.maxDate());
			Add(new ExchangeRate(new EURCurrency(), new LUFCurrency(), 40.3399), new Date(1, Month.January, 1999), Date.maxDate());
			Add(new ExchangeRate(new EURCurrency(), new NLGCurrency(), 2.20371), new Date(1, Month.January, 1999), Date.maxDate());
			Add(new ExchangeRate(new EURCurrency(), new PTECurrency(), 200.482), new Date(1, Month.January, 1999), Date.maxDate());
			// other obsoleted currencies
			Add(new ExchangeRate(new TRYCurrency(), new TRLCurrency(), 1000000.0), new Date(1, Month.January, 2005), Date.maxDate());
			Add(new ExchangeRate(new RONCurrency(), new ROLCurrency(), 10000.0), new Date(1, Month.July, 2005), Date.maxDate());
			Add(new ExchangeRate(new PENCurrency(), new PEICurrency(), 1000000.0), new Date(1, Month.July, 1991), Date.maxDate());
			Add(new ExchangeRate(new PEICurrency(), new PEHCurrency(), 1000.0), new Date(1, Month.February, 1985), Date.maxDate());
		}

		public void Add(ExchangeRate rate)
		{
			Add(rate, Date.minDate(), Date.maxDate());
		}

		public void Add(ExchangeRate rate, Date startDate)
		{
			Add(rate, startDate, Date.maxDate());
		}

		/// <summary>
		/// Add an exchange rate.
		/// </summary>
		/// <param name="rate"></param>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <remarks>
		/// The given rate is valid between the given dates.
		/// If two rates are given between the same currencies
		/// and with overlapping date ranges, the latest one
		/// added takes precedence during lookup.
		/// </remarks> 
		private void Add(ExchangeRate rate, Date startDate, Date endDate)
		{
			int k = Hash(rate.source, rate.target);
			if (_data.ContainsKey(k))
			{
				_data[k].Insert(0, new ExchangeRateManagerEntry(rate, startDate, endDate));
			}
			else
			{
				_data[k] = new List<ExchangeRateManagerEntry> { new ExchangeRateManagerEntry(rate, startDate, endDate) };
			}
		}

		private static int Hash(Currency c1, Currency c2)
		{
			return Math.Min(c1.numericCode, c2.numericCode) * 1000
				 + Math.Max(c1.numericCode, c2.numericCode);
		}

		private static bool Hashes(int k, Currency c)
		{
			return (c.numericCode == k % 1000) || (c.numericCode == k / 1000);
		}

		public ExchangeRate lookup(Currency source, Currency target)
		{
			return lookup(source, target, Settings.evaluationDate(), ExchangeRate.Type.Derived);
		}

		public ExchangeRate lookup(Currency source, Currency target, Date date)
		{
			return lookup(source, target, date, ExchangeRate.Type.Derived);
		}

		/// <summary>
		/// Lookup the exchange rate between two currencies at a given
		/// date.  If the given type is Direct, only direct exchange
		/// rates will be returned if available; if Derived, direct
		/// rates are still preferred but derived rates are allowed.
		/// </summary>
		/// <remarks>
		/// if two or more exchange-rate chains are possible
		/// which allow to specify a requested rate, it is
		/// unspecified which one is returned.
		/// </remarks>
		/// <param name="source"></param>
		/// <param name="target"></param>
		/// <param name="date"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public ExchangeRate lookup(Currency source, Currency target, Date date, ExchangeRate.Type type)
		{
			if (source == target)
			{
				return new ExchangeRate(source, target, 1.0);
			}

			if (date == new Date())
			{
				date = Settings.evaluationDate();
			}

			if (type == ExchangeRate.Type.Direct)
			{
				return DirectLookup(source, target, date);
			}

			if (!source.triangulationCurrency.IsEmpty)
			{
				Currency link = source.triangulationCurrency;
				return link == target ? DirectLookup(source, link, date) : ExchangeRate.chain(DirectLookup(source, link, date), lookup(link, target, date));
			}

			if (!target.triangulationCurrency.IsEmpty)
			{
				Currency link = target.triangulationCurrency;
				return source == link ? DirectLookup(link, target, date) : ExchangeRate.chain(lookup(source, link, date), DirectLookup(link, target, date));
			}

			return SmartLookup(source, target, date);
		}

		private ExchangeRate DirectLookup(Currency source, Currency target, Date date)
		{
			ExchangeRate rate = Fetch(source, target, date);

			if (rate.rate != 0)
				return rate;

			throw new Exception("no direct conversion available from " + source.code + " to " + target.code + " for " + date);
		}

		private ExchangeRate SmartLookup(Currency source, Currency target, Date date)
		{
			return SmartLookup(source, target, date, new List<int>());
		}

		private ExchangeRate SmartLookup(Currency source, Currency target, Date date, List<int> forbidden)
		{
			// direct exchange rates are preferred.
			ExchangeRate direct = Fetch(source, target, date);
			if (direct.HasValue)
			{
				return direct;
			}

			// if none is found, turn to smart lookup. The source currency
			// is forbidden to subsequent lookups in order to avoid cycles.
			forbidden.Add(source.numericCode);

			foreach (KeyValuePair<int, List<ExchangeRateManagerEntry>> i in _data)
			{
				// we look for exchange-rate data which involve our source
				// currency...
				if (Hashes(i.Key, source) && (i.Value.Count != 0))
				{
					// ...whose other currency is not forbidden...
					ExchangeRateManagerEntry e = i.Value[0];// front();
					Currency other = source == e.Item1.source ? e.Item1.target : e.Item1.source;
					if (!forbidden.Contains(other.numericCode))
					{
						// ...and which carries information for the requested date.
						ExchangeRate head = Fetch(source, other, date);
						if (((double?)head.rate).HasValue)
						{
							// if we can get to the target from here...
							try
							{
								ExchangeRate tail = SmartLookup(other, target, date, forbidden);
								// ..we're done.
								return ExchangeRate.chain(head, tail);
							}
							catch (Exception)
							{
							}
						}
					}
				}
			}

			// if the loop completed, we have no way to return the requested rate.
			throw new Exception("no conversion available from " + source.code + " to " + target.code + " for " + date);
		}

		private ExchangeRate Fetch(Currency source, Currency target, Date date)
		{
			if (_data.ContainsKey(Hash(source, target)))
			{
				List<ExchangeRateManagerEntry> rates = _data[Hash(source, target)];
				
				foreach (ExchangeRateManagerEntry e in rates)
				{
					if (e.IsInRange(date))
					{
						return e.Item1;
					}
				}
			}

			return new ExchangeRate();
		}

		/// <summary>
		/// remove the added exchange rates
		/// </summary>
		public void clear()
		{
			_data.Clear();
			AddKnownRates();
		}

		private class ExchangeRateManagerEntry : Tuple<ExchangeRate, Date, Date>
		{
			public ExchangeRateManagerEntry(ExchangeRate item1, Date item2, Date item3) 
				: base(item1, item2, item3)
			{
			}

			public bool IsInRange(Date date)
			{
				return date >= Item2 && date <= Item3;
			}
		}
	}
}
