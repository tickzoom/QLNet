using System;
using System.Collections.Generic;
using System.Text;

namespace QLNet.Global
{
   public static class Math
   {
      public static bool close(double x, double  y)
      {
         return close(x, y, 42);
      }

      public static bool close(double x, double y, int n)
      {
         double diff = System.Math.Abs(x - y);
         double tolerance = n * Double.Epsilon;
         // FLOATING_POINT_EXCEPTION
         return diff <= tolerance * System.Math.Abs(x) && diff <= tolerance * System.Math.Abs(y);
      }

   }
}
