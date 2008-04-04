/*
 Copyright (C) 2008 Siarhei Novik (snovik@gmail.com)
  
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
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QLNet
{
    //! rounding methods
    /*! The rounding methods follow the OMG specification available
        at ftp://ftp.omg.org/pub/docs/formal/00-06-29.pdf
        \warning the names of the Floor and Ceiling methods might be misleading. Check the provided reference. */
    public interface IRounding
    {
        decimal round(decimal value);
    }

    public class Rounding : IRounding
    {
        public enum Type
        {
            None,    /*!< do not round: return the number unmodified */
            Up,      /*!< the first decimal place past the precision will be
                          rounded up. This differs from the OMG rule which
                          rounds up only if the decimal to be rounded is
                          greater than or equal to the rounding digit */
            Down,    /*!< all decimal places past the precision will be truncated */
            Closest, /*!< the first decimal place past the precision
                          will be rounded up if greater than or equal
                          to the rounding digit; this corresponds to
                          the OMG round-up rule.  When the rounding
                          digit is 5, the result will be the one
                          closest to the original number, hence the name. */
            Floor,   /*!< positive numbers will be rounded up and negative
                          numbers will be rounded down using the OMG round up
                          and round down rules */
            Ceiling  /*!< positive numbers will be rounded down and negative
                          numbers will be rounded up using the OMG round up
                          and round down rules */
        };

        private int precision_;
        Type type_;

        // Inspectors
        int precision { get { return precision_; } }
        Type type { get { return type_; } }

        // default constructor
        // Instances built through this constructor don't perform any rounding.
        public Rounding()
        {
            type_ = Type.None;
        }

        public Rounding(int precision) : this(precision, Type.Closest) { }
        public Rounding(int precision, Type type)
        {
            precision_ = precision;
            type_ = type;
        }

        //! perform rounding
        public decimal round(decimal value)
        {
            if (type_ == Type.None) return value;

            decimal mult = (decimal)System.Math.Pow(10, precision_);
            decimal lvalue = value * mult;

            switch (type_)
            {
                case Type.Down:
                    lvalue = System.Math.Truncate(lvalue);
                    break;
                case Type.Up:
                    lvalue = System.Math.Truncate(lvalue + System.Math.Sign(value));
                    break;
                case Type.Closest:
                    lvalue = System.Math.Round(lvalue, 0, MidpointRounding.AwayFromZero);
                    break;
                case Type.Floor:
                    lvalue = System.Math.Floor(lvalue);
                    break;
                case Type.Ceiling:
                    lvalue = System.Math.Ceiling(lvalue);
                    break;
                default:
                    throw new ArgumentException("unknown rounding method");
            }
            return lvalue / mult;
        }
    }
}
