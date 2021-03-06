﻿/*
 Copyright (C) 2009 Philippe Real (ph_real@hotmail.com)
  
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
	public class GenericModelEngine<TModelType, TArgumentsType, TResultsType> : GenericEngine<TArgumentsType, TResultsType>
		where TArgumentsType : IPricingEngineArguments, new()
		where TResultsType : IPricingEngineResults, new()
		where TModelType : class, IObservable
	{
		protected TModelType model_;

		public GenericModelEngine()
		{
		}
	
		public GenericModelEngine(TModelType model)
		{
			model_ = model;
			model_.registerWith(update);
		}

		public void setModel(TModelType model)
		{
			if (model_ != null)
			{
				model_.unregisterWith(update);
			}
		
			model_ = model;
			
			if (model_ != null)
			{
				model_.registerWith(update);
			}
			
			update();
		}
	}
}
