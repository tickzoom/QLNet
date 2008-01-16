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
using System.Reflection;

namespace QLNet
{
   public class Singleton1<T> where T : new()
   {
      /// <summary>
      /// Gets the singleton instance.
      /// </summary>
      public static T Instance
      {
         get
         {
            // Thread safe, lazy implementation of the singleton pattern.
            // See http://www.yoda.arachsys.com/csharp/singleton.html
            // for the full story.
            return SingletonInternal.instance;
         }
      }

      private class SingletonInternal
      {
         // Explicit static constructor to tell C# compiler
         // not to mark type as beforefieldinit.
         static SingletonInternal()
         {
         }

         internal static readonly T instance = new T();
      }
   }

   public class Singleton<T> where T : class
   {
      /// <summary>
      /// Gets the singleton instance.
      /// </summary>
      public static T Instance
      {
         get
         {
            // Thread safe, lazy implementation of the singleton pattern.
            // See http://www.yoda.arachsys.com/csharp/singleton.html
            // for the full story.
            return SingletonInternal.instance;
         }
      }

      private class SingletonInternal
      {
         // Explicit static constructor to tell C# compiler
         // not to mark type as beforefieldinit.
         static SingletonInternal()
         {
         }

         internal static readonly T instance = typeof(T).InvokeMember(typeof(T).Name,
                               BindingFlags.CreateInstance |
                               BindingFlags.Instance |
                               BindingFlags.NonPublic,
                               null, null, null) as T;
      }
   }

}
