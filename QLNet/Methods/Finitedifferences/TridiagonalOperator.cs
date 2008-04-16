using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QLNet
{
   public class TridiagonalOperator : ICloneable
   {
      protected Array<double> diagonal_, lowerDiagonal_, upperDiagonal_;
      double timeSetter_;

      public object Clone()
      {
         TridiagonalOperator t = new TridiagonalOperator();
         t.diagonal_ = new Array<double>(this.diagonal_);
         t.lowerDiagonal_ = new Array<double>(this.lowerDiagonal_);
         t.upperDiagonal_ = new Array<double>(this.upperDiagonal_);
         t.setTime(this.timeSetter_);

         return t;
      }

      private TridiagonalOperator() { }

      public TridiagonalOperator(int size) 
      {
         if (size>=2) 
         {
            diagonal_      = new Array<double>(size);
            lowerDiagonal_ = new Array<double>(size - 1);
            upperDiagonal_ = new Array<double>(size - 1);
         } 
         else if (size==0) 
         {
            diagonal_ = new Array<double>(0);
            lowerDiagonal_ = new Array<double>(0);
            upperDiagonal_ = new Array<double>(0);
         } 
         else 
         {
            throw new ArgumentException("invalid size (" + size + ") for tridiagonal operator " +
                                        "(must be null or >= 2)");
         }
      }

      public TridiagonalOperator(Array<double> low,Array<double> mid,Array<double> high)
      {
         if (low.Count != mid.Count - 1)
            throw new ArgumentException("wrong size for lower diagonal vector");
         if (high.Count != mid.Count - 1)
            throw new ArgumentException("wrong size for upper diagonal vector");

         diagonal_ = new Array<double>(mid);
         lowerDiagonal_ = new Array<double>(low);
         upperDiagonal_ = new Array<double>(high);
      }

      public TridiagonalOperator(TridiagonalOperator from) 
      {
         this.diagonal_ = new Array<double>(from.diagonal_);
         this.lowerDiagonal_ = new Array<double>(from.lowerDiagonal_);
         this.upperDiagonal_ = new Array<double>(from.upperDiagonal_);
         this.setTime(from.timeSetter_);

        //this = (TridiagonalOperator)from.Clone();
      }

      public int size()
      {
         return diagonal_.Count;
      }


      public Array<double> applyTo(Array<double> v) 
      {
         if ( v.Count != size() )
            throw new ArgumentException("vector of the wrong size (" + v.Count +
                                        "instead of " + size() + ")"  );
        Array<double> result = new Array<double>(size());

        for (int i = 0; i < diagonal_.Count; i++)
            result[i] = (double)diagonal_[i] * v[i];

        // matricial product
        result[0] += upperDiagonal_[0]*v[1];
        for (int j=1; j<=size()-2; j++)
            result[j] += lowerDiagonal_[j-1]*v[j-1]+ 
                         upperDiagonal_[j]*v[j+1];
        result[size()-1] += lowerDiagonal_[size()-2]*v[size()-2];

        return result;
    
      }

      public Array<double> solveFor(Array<double> rhs) 
      {
        
         if (rhs.Count != size() )
            throw new ArgumentException("rhs has the wrong size");

         Array<double> result = new Array<double>(size());
         Array<double> tmp = new Array<double>(size());
        
         double bet=diagonal_[0];
         if ( bet == 0 )
            throw new ApplicationException("division by zero");

         result[0] = rhs[0]/bet;
         int j;
         for (j=1; j<=size()-1; j++)
         {
            tmp[j]=upperDiagonal_[j-1]/bet;
            bet=diagonal_[j]-lowerDiagonal_[j-1]*tmp[j];
            if (bet == 0)
               throw new ApplicationException("division by zero");
            result[j] = (rhs[j]-lowerDiagonal_[j-1]*result[j-1])/bet;
        }
        // cannot be j>=0 with Size j
        for (j=size()-2; j>0; --j)
            result[j] -= tmp[j+1]*result[j+1];
        result[0] -= tmp[1]*result[1];
        return result;
      }


      public Array<double> SOR(Array<double> rhs,double tol) 
      {
         if ( rhs.Count != size() )
            throw new ArgumentException("rhs has the wrong size");

         // initial guess
         Array<double> result = new Array<double>(rhs);

         // solve tridiagonal system with SOR technique
         int sorIteration, i;
         double omega = 1.5;
         double err = 2.0*tol;
         double temp;
         for (sorIteration=0; err>tol ; sorIteration++) 
         {
            if (sorIteration>=100000)
               throw new ApplicationException("tolerance (" + tol + ") not reached in " +
                                              sorIteration + " iterations. " +
                                              "The error still is " + err);

            temp = omega * (rhs[0]     -
                            upperDiagonal_[0]   * result[1]-
                            diagonal_[0]        * result[0])/diagonal_[0];
            err = temp*temp;
            result[0] += temp;

            for (i=1; i<size()-1 ; i++) {
                temp = omega *(rhs[i]     -
                               upperDiagonal_[i]   * result[i+1]-
                               diagonal_[i]        * result[i] -
                               lowerDiagonal_[i-1] * result[i-1])/diagonal_[i];
                err += temp * temp;
                result[i] += temp;
            }

            temp = omega * (rhs[i]     -
                            diagonal_[i]        * result[i] -
                            lowerDiagonal_[i-1] * result[i-1])/diagonal_[i];
            err += temp*temp;
            result[i] += temp;
         }
         return result;
      }

      TridiagonalOperator identity(int size) 
      {
        TridiagonalOperator I = new TridiagonalOperator(new Array<double>(size-1),  // lower diagonal
                                                        new Array<double>(size,1),  // diagonal
                                                        new Array<double>(size-1)); // upper diagonal

        return I;
      }

      public Array<double> lowerDiagonal()
      {
         return lowerDiagonal_;
      }

      public Array<double> diagonal() 
      {
         return diagonal_;
      }

      public Array<double> upperDiagonal() 
      {
         return upperDiagonal_;
      }
   
      public void setFirstRow(double valB,double valC) 
      {
        diagonal_[0]      = valB;
        upperDiagonal_[0] = valC;
      }

      public void setMidRow(int i,double valA,double valB,double valC) 
      {
         if ( i < 1 || i > size()-2 )
            throw new ArgumentException("out of range in TridiagonalSystem::setMidRow");

        lowerDiagonal_[i-1] = valA;
        diagonal_[i]        = valB;
        upperDiagonal_[i]   = valC;
      }

      public void setMidRows(double valA,double valB,double valC) 
      {
         for (int i=1; i<=size()-2; i++) 
         {
            lowerDiagonal_[i-1] = valA;
            diagonal_[i]        = valB;
            upperDiagonal_[i]   = valC;
         }
      }

      public void setLastRow(double valA,double valB) 
      {
         lowerDiagonal_[size()-2] = valA;
         diagonal_[size()-1]      = valB;
      }

      public void setTime(double t)
      {
         //if (timeSetter_)
         //   timeSetter_->setTime(t, *this);
         timeSetter_ = t;
      }

      // Time constant algebra

      public static TridiagonalOperator operator +(TridiagonalOperator D) 
      {
        TridiagonalOperator D1 = ( TridiagonalOperator )D.Clone();
        return D1;
      }

     
      //public static TridiagonalOperator operator-(TridiagonalOperator D) 
      //{
      //  Array<double> low = new Array<double>(-D.lowerDiagonal_);
      //  Array<double> high = new Array<double>(-D.upperDiagonal_);
      //  TridiagonalOperator result = new TridiagonalOperator(low,mid,high);
      //  return result;
      //}

      //public static TridiagonalOperator  operator+(TridiagonalOperator D1,TridiagonalOperator D2) 
      //{
      //  Array<double> low = D1.lowerDiagonal_.+D2.lowerDiagonal_,
      //      mid = D1.diagonal_+D2.diagonal_,
      //      high = D1.upperDiagonal_+D2.upperDiagonal_;
      //  TridiagonalOperator result(low,mid,high);
      //  return result;
      //}

   }

}
