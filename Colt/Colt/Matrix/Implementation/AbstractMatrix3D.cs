﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Colt.Matrix.Implementation
{
    /// <summary>
    /// Abstract base class for 3-d matrices holding objects or primitive data types such as <code>int</code>, <code>double</code>, etc.
    /// </summary>
    /// <remarks>This implementation is not synchronized.</remarks>
    public abstract class AbstractMatrix3D : AbstractMatrix
    {

        /// <summary>
        /// Gets or sets the number of Slices this matrix (view) has.
        /// </summary>
        public int Slices { get; protected set; }

        /// <summary>
        /// Gets or sets the number of colums this matrix (view) has.
        /// </summary>
        public int Columns { get; protected set; }

        /// <summary>
        /// Gets or sets the number of Rows this matrix (view) has.
        /// </summary>
        public int Rows { get; protected set; }

        /// <summary>
        /// Gets or sets the number of elements between two Slices, i.ed <i>index(i+1,j,k) - index(i,j,k)</i>.
        /// </summary>
        protected int Slicestride { get; set; }

        /// <summary>
        /// Gets or sets the number of elements between two Rows, i.ed <i>index(i+1,j,k) - index(i,j,k)</i>.
        /// </summary>
        protected int Rowstride { get; set; }

        /// <summary>
        /// Gets or sets the number of elements between two Columns, i.ed <i>index(i,j+1,k) - index(i,j,k)</i>.
        /// </summary>
        protected int Columnstride { get; set; }

        /// <summary>
        /// Gets or sets the Slices index of the first element.
        /// </summary>
        protected int sliceZero { get; set; }

        /// <summary>
        /// Gets or sets the row index of the first element.
        /// </summary>
        protected int rowZero { get; set; }

        /// <summary>
        /// Gets or sets the column index of the first element.
        /// </summary>
        protected int columnZero { get; set; }

        /// <summary>
        /// Returns the position of the given absolute rank within the (virtual or non-virtual) internal 1-dimensional arrayd 
        /// Default implementationd Override, if necessary.
        /// </summary>
        /// <param name="absRank">the absolute rank of the element.</param>
        /// <returns>the position.</returns>
        protected int columnOffset(int absRank)
        {
            return absRank;
        }

        /// <summary>
        /// Returns the absolute rank of the given relative rankd 
        /// </summary>
        /// <param name="rank">the relative rank of the element.</param>
        /// <returns>the absolute rank of the element.</returns>
        protected int columnRank(int rank)
        {
            return columnZero + rank * Columnstride;
        }

        /// <summary>
        /// Returns the position of the given absolute rank within the (virtual or non-virtual) internal 1-dimensional arrayd 
        /// Default implementationd Override, if necessary.
        /// </summary>
        /// <param name="rank">the absolute rank of the element.</param>
        /// <returns>the position.</returns>
        protected int rowOffset(int absRank)
        {
            return absRank;
        }

        /// <summary>
        /// Returns the absolute rank of the given relative rankd 
        /// </summary>
        /// <param name="rank">the relative rank of the element.</param>
        /// <returns>the absolute rank of the element.</returns>
        protected int rowRank(int rank)
        {
            return rowZero + rank * Rowstride;
        }

        /// <summary>
        /// Returns the position of the given absolute rank within the (virtual or non-virtual) internal 1-dimensional arrayd 
        /// Default implementationd Override, if necessary.
        /// </summary>
        /// <param name="rank">the absolute rank of the element.</param>
        /// <returns>the position.</returns>
        protected int sliceOffset(int absRank)
        {
            return absRank;
        }

        /// <summary>
        /// Returns the absolute rank of the given relative rankd 
        /// </summary>
        /// <param name="rank">the relative rank of the element.</param>
        /// <returns>the absolute rank of the element.</returns>
        protected int sliceRank(int rank)
        {
            return sliceZero + rank * Slicestride;
        }

        /// <summary>
        /// Checks whether the receiver contains the given box and thRows an exception, if necessary.
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">if <i>row %lt; 0 || height %lt; 0 || row + height %gt; Rows || slice %lt; 0 || depth %lt; 0 || slice+depth %gt; Slices  || column %lt; 0 || width %lt; 0 || column + width %gt; Columns</i></exception>
        protected void checkBox(int slice, int row, int column, int depth, int height, int width)
        {
            if (slice < 0 || depth < 0 || slice + depth > Slices || row < 0 || height < 0 || row + height > Rows || column < 0 || width < 0 || column + width > Columns) throw new IndexOutOfRangeException(toStringShort() + ", slice:" + slice + ", row:" + row + " ,column:" + column + ", depth:" + depth + " ,height:" + height + ", width:" + width);
        }

        /// <summary>
        /// Sanity check for operations requiring a column index to be within bounds.
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">if <i>column  %lt; 0 || column  %gt;= Columns()</i>.</exception>
        protected void checkColumn(int column)
        {
            if (column < 0 || column >= Columns) throw new IndexOutOfRangeException("Attempted to access " + toStringShort() + " at column=" + column);
        }

        /// <summary>
        /// Checks whether indexes are legal and thRows an exception, if necessary.
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">if <i>! (0 %lt;= indexes[i] %lt; Columns())</i> for any i=0..indexes.Length()-1.</exception>
        protected void checkColumnIndexes(int[] indexes)
        {
            for (int i = indexes.Length; --i >= 0;)
            {
                int index = indexes[i];
                if (index < 0 || index >= Columns) checkColumn(index);
            }
        }

        /// <summary>
        /// Sanity check for operations requiring a row index to be within bounds.
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">if <i>row %lt; 0 || row %gt;= Rows()</i>.</exception>
        protected void checkRow(int row)
        {
            if (row < 0 || row >= Rows) throw new IndexOutOfRangeException("Attempted to access " + toStringShort() + " at row=" + row);
        }

        /// <summary>
        /// Checks whether indexes are legal and thRows an exception, if necessary.
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">if <i>! (0 %lt;= indexes[i] %lt; Rows())</i> for any i=0..indexes.Length()-1.</exception>
        protected void checkRowIndexes(int[] indexes)
        {
            for (int i = indexes.Length; --i >= 0;)
            {
                int index = indexes[i];
                if (index < 0 || index >= Rows) checkRow(index);
            }
        }

        /// <summary>
        /// Sanity check for operations requiring two matrices with the same number of Slices, Rows and Columns.
        /// </summary>
        /// <exception cref="ArgumentException">if <i>Slices() != B.Slices() || Rows() != B.Rows() || Columns() != B.Columns()</i>.</exception>
        public void checkShape(AbstractMatrix3D B)
        {
            if (Slices != B.Slices || Rows != B.Rows || Columns != B.Columns) throw new ArgumentException("Incompatible dimensions: " + toStringShort() + " and " + B.toStringShort());
        }

        /// <summary>
        /// Sanity check for operations requiring matrices with the same number of Slices, Rows and Columns.
        /// </summary>
        /// <exception cref="ArgumentException">if <i>Slices() != B.Slices() || Rows() != B.Rows() || Columns() != B.Columns() || Slices() != C.Slices() || Rows() != C.Rows() || Columns() != C.Columns()</i>.</exception>
        public void checkShape(AbstractMatrix3D B, AbstractMatrix3D C)
        {
            if (Slices != B.Slices || Rows != B.Rows || Columns != B.Columns || Slices != C.Slices || Rows != C.Rows || Columns != C.Columns) throw new ArgumentException("Incompatible dimensions: " + toStringShort() + ", " + B.toStringShort() + ", " + C.toStringShort());
        }

        /// <summary>
        /// Sanity check for operations requiring a slice index to be within bounds.
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">if <i>slice %lt; 0 || slice %gt;= Slices()</i>.</exception>
        protected void checkSlice(int slice)
        {
            if (slice < 0 || slice >= Slices) throw new IndexOutOfRangeException("Attempted to access " + toStringShort() + " at slice=" + slice);
        }

        /// <summary>
        /// Checks whether indexes are legal and thRows an exception, if necessary.
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">if <i>! (0 %lt;= indexes[i] %lt; Slices())</i> for any i=0..indexes.Length()-1.</exception>
        protected void checkSliceIndexes(int[] indexes)
        {
            for (int i = indexes.Length; --i >= 0;)
            {
                int index = indexes[i];
                if (index < 0 || index >= Slices) checkSlice(index);
            }
        }

        /// <summary>
        /// Returns the position of the given coordinate within the (virtual or non-virtual) internal 1-dimensional arrayd 
        /// </summary>
        /// <param name="slice"> the index of the slice-coordinate.</param>
        /// <param name="row">the index of the row-coordinate.</param>
        /// <param name="column">the index of the third-coordinate.</param>
        /// <returns></returns>
        protected int index(int slice, int row, int column)
        {
            return sliceOffset(sliceRank(slice)) + rowOffset(rowRank(row)) + columnOffset(columnRank(column));
        }

        /// <summary>
        /// Sets up a matrix with a given number of Slices and Rows.
        /// </summary>
        /// <param name="Slices">the number of Slices the matrix shall have.</param>
        /// <param name="Rows">the number of Rows the matrix shall have.</param>
        /// <param name="Columns">the number of Columns the matrix shall have.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">if <i>(double)Rows * Slices %gt; int.MaxValue</i>.</exception>
        /// <exception cref="ArgumentException">if <i>Slices %lt; 0 || Rows %lt; 0 || Columns %lt; 0</i>.</exception>
        protected void setUp(int Slices, int Rows, int Columns)
        {
            setUp(Slices, Rows, Columns, 0, 0, 0, Rows * Columns, Columns, 1);
        }

        /// <summary>
        /// Sets up a matrix with a given number of Slices and Rows and the given strides.
        /// </summary>
        /// <param name="Slices">the number of Slices the matrix shall have.</param>
        /// <param name="Rows">the number of Rows the matrix shall have.</param>
        /// <param name="Columns">the number of Columns the matrix shall have.</param>
        /// <param name="sliceZero">the position of the first element.</param>
        /// <param name="rowZero">the position of the first element.</param>
        /// <param name="columnZero">the position of the first element.</param>
        /// <param name="Slicestride">the number of elements between two Slices, i.ed <i>index(k+1,i,j)-index(k,i,j)</i>.</param>
        /// <param name="Rowstride">the number of elements between two Rows, i.ed <i>index(k,i+1,j)-index(k,i,j)</i>.</param>
        /// <param name="columnnStride">the number of elements between two Columns, i.ed <i>index(k,i,j+1)-index(k,i,j)</i>.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">if <i>(double)Slices * Rows * Columnss %gt; int.MaxValue</i>.</exception>
        /// <exception cref="ArgumentException">if <i>Slices %lt; 0 || Rows %lt; 0 || Columns %lt; 0</i>.</exception>
        protected void setUp(int Slices, int Rows, int Columns, int sliceZero, int rowZero, int columnZero, int Slicestride, int Rowstride, int Columnstride)
        {
            if (Slices < 0 || Rows < 0 || Columns < 0) throw new ArgumentException("negative size");
            if ((double)Slices * Rows * Columns > int.MaxValue) throw new ArgumentException("matrix too large");

            this.Slices = Slices;
            this.Rows = Rows;
            this.Columns = Columns;

            this.sliceZero = sliceZero;
            this.rowZero = rowZero;
            this.columnZero = columnZero;

            this.Slicestride = Slicestride;
            this.Rowstride = Rowstride;
            this.Columnstride = Columnstride;

            this.isView = false;
        }

        protected int[] shape()
        {
            int[] shape = new int[3];
            shape[0] = Slices;
            shape[1] = Rows;
            shape[2] = Columns;
            return shape;
        }
        
        /// <summary>
        /// Returns the number of cells which is <i>Slices()*Rows()*Columns()</i>.
        /// </summary>
        public int size()
        {
            return Slices * Rows * Columns;
        }
        
        /// <summary>
        /// Returns a string representation of the receiver's shape.
        /// </summary>
        public String toStringShort()
        {
            return AbstractFormatter.Shape(this);
        }
        
        /// <summary>
        /// Self modifying version of viewColumnFlip().
        /// </summary>
        protected AbstractMatrix3D vColumnFlip()
        {
            if (Columns > 0)
            {
                columnZero += (Columns - 1) * Columnstride;
                Columnstride = -Columnstride;
                this.isView = true;
            }
            return this;
        }

        /// <summary>
        /// Self modifying version of viewDice().
        /// </summary>
        /// <exception cref="ArgumentException">if some of the parameters are equal or not in range 0..2.</exception>
        protected AbstractMatrix3D vDice(int axis0, int axis1, int axis2)
        {
            int d = 3;
            if (axis0 < 0 || axis0 >= d || axis1 < 0 || axis1 >= d || axis2 < 0 || axis2 >= d ||
                axis0 == axis1 || axis0 == axis2 || axis1 == axis2)
            {
                throw new ArgumentException("Illegal Axes: " + axis0 + ", " + axis1 + ", " + axis2);
            }

            // swap shape
            int[] shapes = shape();

            this.Slices = shapes[axis0];
            this.Rows = shapes[axis1];
            this.Columns = shapes[axis2];

            // swap strides
            int[] strides = new int[3];
            strides[0] = this.Slicestride;
            strides[1] = this.Rowstride;
            strides[2] = this.Columnstride;

            this.Slicestride = strides[axis0];
            this.Rowstride = strides[axis1];
            this.Columnstride = strides[axis2];

            this.isView = true;
            return this;
        }

        /// <summary>
        /// Self modifying version of viewPart().
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">if <i>slice %lt; 0 || depth %lt; 0 || slice+depth %gt; Slices() || row %lt; 0 || height %lt; 0 || row+height %gt; Rows() || column %lt; 0 || width %lt; 0 || column + width %gt; Columns()</i></exception>
        protected AbstractMatrix3D vPart(int slice, int row, int column, int depth, int height, int width)
        {
            checkBox(slice, row, column, depth, height, width);

            this.sliceZero += this.Slicestride * slice;
	this.rowZero += this.Rowstride * row;
	this.columnZero += this.Columnstride * column;

            this.Slices = depth;
            this.Rows = height;
            this.Columns = width;

            this.isView = true;
            return this;
        }
        
        /// <summary>
        /// Self modifying version of viewRowFlip().
        /// </summary>
        protected AbstractMatrix3D vRowFlip()
        {
            if (Rows > 0)
            {
                rowZero += (Rows - 1) * Rowstride;
                Rowstride = -Rowstride;
                this.isView = true;
            }
            return this;
        }
        
        /// <summary>
        /// Self modifying version of viewSliceFlip().
        /// </summary>
        protected AbstractMatrix3D vSliceFlip()
        {
            if (Slices > 0)
            {
                sliceZero += (Slices - 1) * Slicestride;
                Slicestride = -Slicestride;
                this.isView = true;
            }
            return this;
        }

        /// <summary>
        /// Self modifying version of viewStrides().
        /// @thRows  
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">if <i>Slicestride %lt;= 0 || Rowstride %lt;= 0 || Columnstride %lt;= 0</i>.</exception>
        protected AbstractMatrix3D vStrides(int Slicestride, int Rowstride, int Columnstride)
        {
            if (Slicestride <= 0 || Rowstride <= 0 || Columnstride <= 0) throw new IndexOutOfRangeException("illegal strides: " + Slicestride + ", " + Rowstride + ", " + Columnstride);

            this.Slicestride *= Slicestride;
	this.Rowstride *= Rowstride;
	this.Columnstride *= Columnstride;

            if (this.Slices != 0) this.Slices = (this.Slices - 1) / Slicestride + 1;
            if (this.Rows != 0) this.Rows = (this.Rows - 1) / Rowstride + 1;
            if (this.Columns != 0) this.Columns = (this.Columns - 1) / Columnstride + 1;

            this.isView = true;
            return this;
        }
    }
}
