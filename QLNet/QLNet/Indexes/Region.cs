/*
 Copyright (C) 2008, 2009 , 2010  Andrea Maggiulli (a.maggiulli@gmail.com)
 * 
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

namespace QLNet
{
	/// <summary>
	/// Region class, used for inflation applicability.
	/// </summary>
	public class Region
	{
		public string name()
		{
			return data_.name;
		}

		public string code()
		{
			return data_.code;
		}

		protected Region() { }

		protected Data data_;

		public bool Equals(Region other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return other.data_.Equals(data_);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (Region)) return false;
			return Equals((Region) obj);
		}

		public override int GetHashCode()
		{
			return data_.GetHashCode();
		}

		public static bool operator ==(Region r1, Region r2)
		{
			return Equals(r1, r2);
		}

		public static bool operator !=(Region r1, Region r2)
		{
			return !Equals(r1, r2);
		}

		protected struct Data
		{
			public string name;
			public string code;

			public Data(string Name, string Code)
			{
				name = Name;
				code = Code;
			}

			public bool Equals(Data other)
			{
				return Equals(other.name, name) && Equals(other.code, code);
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (obj.GetType() != typeof (Data)) return false;
				return Equals((Data) obj);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					return ((name != null ? name.GetHashCode() : 0)*397) ^ (code != null ? code.GetHashCode() : 0);
				}
			}
		}
	}

	/// <summary>
	/// Australia as geographical/economic region
	/// </summary>
	public class AustraliaRegion : Region
	{
		public AustraliaRegion()
		{
			data_ = new Data("Australia", "AU");
		}
	}

	/// <summary>
	/// European Union as geographical/economic region.
	/// </summary>
	public class EURegion : Region
	{
		public EURegion()
		{
			data_ = new Data("EU", "EU");
		}
	}

	/// <summary>
	/// France as geographical/economic region.
	/// </summary>
	public class FranceRegion : Region
	{
		public FranceRegion()
		{
			data_ = new Data("France", "FR");
		}
	}

	/// <summary>
	/// United Kingdom as geographical/economic region.
	/// </summary>
	public class UKRegion : Region
	{
		public UKRegion()
		{
			data_ = new Data("UK", "UK");
		}
	}

	/// <summary>
	/// USA as geographical/economic region.
	/// </summary>
	public class USRegion : Region
	{
		public USRegion()
		{
			data_ = new Data("USA", "US");
		}
	}
}
