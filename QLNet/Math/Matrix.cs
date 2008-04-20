/*
 Copyright (C) 2008 Siarhei Novik (snovik@gmail.com)
  
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
using System.Linq;
using System.Text;

namespace QLNet {
    //! %Matrix used in linear algebra.
    /*! This class implements the concept of Matrix as used in linear
        algebra. As such, it is <b>not</b> meant to be used as a
        container.
    */
    public struct Matrix {
        #region properties
        private int rows_, columns_;
        public int rows() { return rows_; }
        public int columns() { return columns_; }
        public bool empty() { return rows_ == 0 || columns_ == 0; }

        private double[,] data_;
        public Vector row(int r) {
            Vector result = new Vector(rows_);
            for (int i = 0; i < rows_; i++)
                result[i] = data_[r, i];
            return result;
        }
        public Vector column(int c) {
            Vector result = new Vector(columns_);
            for (int i = 0; i < columns_; i++)
                result[i] = data_[i, c];
            return result;
        } 
        #endregion

        #region Constructors
        //! creates a null matrix
        // public Matrix() : base(0) { rows_ = 0; columns_ = 0; }

        //! creates a matrix with the given dimensions
        public Matrix(int rows, int columns) {
            data_ = new double[rows, columns];
            rows_ = rows;
            columns_ = columns;
        }

        //! creates the matrix and fills it with <tt>value</tt>
        public Matrix(int rows, int columns, double value) {
            data_ = new double[rows, columns];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                    data_[i, j] = value;
            rows_ = rows;
            columns_ = columns;
        }

        public Matrix(ref Matrix from) {
            data_ = !from.empty() ? new double[from.rows_, from.columns_] : null;
            rows_ = from.rows_;
            columns_ = from.columns_;

            for (int i = 0; i < rows_; i++)
                for (int j = 0; j < columns_; j++)
                    data_[i, j] = from.data_[i, j];
        }
        // Matrix(const Disposable<Matrix>&); 
	    #endregion
    
        //! \name Algebraic operators
        /*! \pre all matrices involved in an algebraic expression must have
                 the same size.
        */
        //@{
        public static Matrix operator +(Matrix m1, Matrix m2) { return operMatrix(ref m1, ref m2, (x, y) => x + y); }
        public static Matrix operator -(Matrix m1, Matrix m2) { return operMatrix(ref m1, ref m2, (x, y) => x - y); }
        public static Matrix operator *(Matrix m1, double value) { return operValue(ref m1, value, (x, y) => x * y); }
        public static Matrix operator /(Matrix m1, double value) { return operValue(ref m1, value, (x, y) => x / y); }
        private static Matrix operMatrix(ref Matrix m1, ref Matrix m2, Func<double, double, double> func) {
            if (!(m1.rows_ == m2.rows_ && m1.columns_ == m2.columns_))
                throw new ApplicationException("operation on matrices with different sizes (" +
                       m2.rows_ + "x" + m2.columns_ + ", " + m1.rows_ + "x" + m1.columns_ + ")");

            Matrix result = new Matrix(m1.rows_, m1.columns_);
            for (int i = 0; i < m1.rows_; i++)
                for (int j = 0; j < m1.columns_; j++)
                    result.data_[i, j] = func(m1.data_[i, j], m2.data_[i, j]);
            return result;
        }
        private static Matrix operValue(ref Matrix m1, double value, Func<double, double, double> func) {
            Matrix result = new Matrix(m1.rows_, m1.columns_);
            for (int i = 0; i < m1.rows_; i++)
                for (int j = 0; j < m1.columns_; j++)
                    result.data_[i, j] = func(m1.data_[i, j], value);
            return result;
        }

        public static Vector operator *(Vector v, Matrix m){
            if (!(v.size() == m.rows()))
                throw new ApplicationException("vectors and matrices with different sizes ("
                       + v.size() + ", " + m.rows() + "x" + m.columns() + ") cannot be multiplied");
            Vector result = new Vector(m.columns());
            for (int i=0; i<result.size(); i++)
                result[i] = v * m.column(i);
            return result;
        }
        /*! \relates Matrix */
        public static Vector operator *(Matrix m, Vector v) {
            if (!(v.size() == m.columns()))
                throw new ApplicationException("vectors and matrices with different sizes ("
                       + v.size() + ", " + m.rows() + "x" + m.columns() + ") cannot be multiplied");
            Vector result = new Vector(m.rows());
            for (int i=0; i<result.size(); i++)
                result[i] = m.row(i) * v;
            return result;
        }
        /*! \relates Matrix */
        public static Matrix operator *(Matrix m1, Matrix m2) {
            if (!(m1.columns() == m2.rows()))
                throw new ApplicationException("matrices with different sizes (" +
                       m1.rows() + "x" + m1.columns() + ", " +
                       m2.rows() + "x" + m2.columns() + ") cannot be multiplied");
            Matrix result = new Matrix(m1.rows(),m2.columns());
            for (int i=0; i<result.rows(); i++)
                for (int j=0; j<result.columns(); j++)
                    result.data_[i, j] = m1.row(i) * m2.column(j);
            return result;
        }

        public static Matrix transpose(Matrix m) {
            Matrix result = new Matrix(m.columns(),m.rows());
            for (int i=0; i<m.rows(); i++)
                for (int j=0; j<m.columns();j++)
                    result.data_[j,i] = m.data_[j,i];
            return result;
        }
    }
}
