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
   public class Extrapolator
   {
      private bool _extrapolate;

      public Extrapolator()
      {
         _extrapolate = false;
      }
      public void Dispose()
      {
      }
      //! \name modifiers
      //@{
      //! enable extrapolation in subsequent calls
      public void enableExtrapolation()
      {
         enableExtrapolation(true);
      }
      public void enableExtrapolation(bool b)
      {
         _extrapolate = b;
      }
      //! disable extrapolation in subsequent calls
      public void disableExtrapolation()
      {
         disableExtrapolation(true);
      }
      public void disableExtrapolation(bool b)
      {
         _extrapolate = !b;
      }
      //@}
      //! \name inspectors
      //@{
      //! tells whether extrapolation is enabled
      public bool allowsExtrapolation()
      {
         return _extrapolate;
      }

   }

}
