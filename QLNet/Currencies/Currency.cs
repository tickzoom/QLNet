/*
 This file is part of QLNet Project http://trac2.assembla.com/QLNet
 
 QLNet is a porting of QuantLib, a free-software/open-source library
 for financial quantitative analysts and developers - http://quantlib.org/
 The license is available online at http://quantlib.org/license.shtml.
 
 This program is distributed in the hope that it will be useful, but WITHOUT
 ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 FOR A PARTICULAR PURPOSE.  See the license for more details.
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace QLNet
{
   public class Currency
   {
      protected struct Data
      {
         public string name;
         public string code;
         public int numeric;
         public string symbol;
         public string fractionSymbol;
         public int fractionsPerUnit;
         public Rounding rounding;
         public Currency triangulated;
         public string formatString;

         
         public Data(string name, string code, int numericCode, string symbol, string fractionSymbol,
                     int fractionsPerUnit, Rounding rounding, string formatString)
            : this(name, code, numericCode, symbol, fractionSymbol, fractionsPerUnit, rounding, formatString, new Currency())
         {}

         public Data(string name, string code, int numericCode, string symbol, string fractionSymbol,
                     int fractionsPerUnit, Rounding rounding, string formatString, Currency triangulationCurrency)
         {
            this.name = name;
            this.code = code;
            this.numeric = numericCode;
            this.symbol = symbol;
            this.fractionSymbol = fractionSymbol;
            this.fractionsPerUnit = fractionsPerUnit;
            this.rounding = rounding;
            this.triangulated = triangulationCurrency ;
            this.formatString = formatString;

         }

      }
      
      protected Data _data;

      public Currency()
      {
      }
   }
}
