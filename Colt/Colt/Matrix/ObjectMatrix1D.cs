﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cern.Colt.Matrix.Implementation;

namespace Cern.Colt.Matrix
{
    public abstract class ObjectMatrix1D: AbstractMatrix1D
    {

        #region Local Variables

        #endregion

        #region Property

        #endregion

        #region Constructor
        protected ObjectMatrix1D() { }
        #endregion

        #region Implement Methods

        #endregion

        #region Local Public Methods
        public Object aggregate(Cern.Colt.Function.ObjectObjectFunction<Object> aggr, Cern.Colt.Function.ObjectFunction<Object> f)
        {
            if (Size == 0) return null;
            Object a = f(this[Size - 1]);
            for (int i = Size - 1; --i >= 0;)
            {
                a = aggr(a, f(this[i]));
            }
            return a;
        }
        /// <summary>
/// Applies a function to each corresponding cell of two matrices and aggregates the results.
/// Returns a value <i>v</i> such that<i> v==a(Size)</i> where<i> a(i) == aggr(a(i-1), f(get(i), other.Get(i)) )</i> and terminators are<i> a(1) == f(get(0),other.Get(0)), a(0)==null</i>.
/// <p>
/// <b>Example:</b>
/// <pre>
/// Cern.jet.math.Functions F = Cern.jet.math.Functions.Functions;
/// x = 0 1 2 3 
/// y = 0 1 2 3 

/// // Sum( x[i]*y[i] )
/// x.aggregate(y, F.plus, F.mult);
/// --> 14

/// // Sum( (x[i]+y[i])^2 )
/// x.aggregate(y, F.plus, F.chain(F.square, F.plus));
/// --> 56
/// </pre>
/// For further examples, see the<a href= "package-summary.html#FunctionObjects" > package doc</a>.

/// @param aggr an aggregation function taking as first argument the current aggregation and as second argument the transformed current cell values.
/// @param f a function transforming the current cell values.
/// @return the aggregated measure.
/// @throws ArgumentException if <i>Size != other.Count</i>.
/// </summary>
public Object aggregate(ObjectMatrix1D other, Cern.Colt.Function.ObjectObjectFunction<Object> aggr, Cern.Colt.Function.ObjectObjectFunction<Object> f)
        {
            CheckSize(other);
            if (Size == 0) return null;
            Object a = f(this[Size - 1], other[Size - 1]);
            for (int i = Size - 1; --i >= 0;)
            {
                a = aggr(a, f(this[i], other[i]));
            }
            return a;
        }
        /// <summary>
        /// Sets all cells to the state specified by <i>values</i>.
        /// <i>values</i> is required to have the same number of cells as the receiver.
        /// <p>
        /// The values are copiedd So subsequent changes in <i>values</i> are not reflected in the matrix, and vice-versa.
        ///
        /// @param    values the values to be filled into the cells.
        /// @return <i>this</i> (for convenience only).
        /// @throws ArgumentException if <i>values.Length != Size</i>.
        /// </summary>
        public ObjectMatrix1D assign(Object[] values)
        {
            if (values.Length != Size) throw new ArgumentException("Must have same number of cells: Length=" + values.Length + ", Size=" + Size);
            for (int i = Size; --i >= 0;)
            {
                setQuick(i, values[i]);
            }
            return this;
        }
        /// <summary>
/// Assigns the result of a function to each cell; <i>x[i] = function(x[i])</i>.
/// (Iterates downwards from<i>[Size-1]</i> to<i>[0]</i>).
/// <p>
/// <b>Example:</b>
/// <pre>
/// // change each cell to its sine
/// matrix =   0.5      1.5      2.5       3.5 
/// matrix.assign(Cern.jet.math.Functions.sin);
/// -->
/// matrix ==  0.479426 0.997495 0.598472 -0.350783
/// </pre>
/// For further examples, see the<a href= "package-summary.html#FunctionObjects" > package doc</a>.

/// @param function a function object taking as argument the current cell's value.
/// @return<i>this</i> (for convenience only).
/// @see Cern.jet.math.Functions
/// </summary>
public ObjectMatrix1D assign(Cern.Colt.Function.ObjectFunction<Object> function)
        {
            for (int i = Size; --i >= 0;)
            {
                setQuick(i, function(this[i]));
            }
            return this;
        }
        /// <summary>
        /// Replaces all cell values of the receiver with the values of another matrix.
        /// Both matrices must have the same size.
        /// If both matrices share the same cells (as is the case if they are views derived from the same matrix) and intersect in an ambiguous way, then replaces <i>as if</i> using an intermediate auxiliary deep copy of <i>other</i>.
        ///
        /// @param     other   the source matrix to copy from (may be identical to the receiver).
        /// @return <i>this</i> (for convenience only).
        /// @throws	ArgumentException if <i>Size != other.Count</i>.
        /// </summary>
        public ObjectMatrix1D assign(ObjectMatrix1D other)
        {
            if (other == this) return this;
            CheckSize(other);
            if (haveSharedCells(other)) other = other.copy();

            for (int i = Size; --i >= 0;)
            {
                setQuick(i, other[i]);
            }
            return this;
        }
        /// <summary>
/// Assigns the result of a function to each cell; <i>x[i] = function(x[i], y[i])</i>.
/// <p>
/// <b>Example:</b>
/// <pre>
/// // assign x[i] = x[i]<sup>y[i]</sup>
/// m1 = 0 1 2 3;
/// m2 = 0 2 4 6;
/// m1.assign(m2, Cern.jet.math.Functions.pow);
/// -->
/// m1 == 1 1 16 729
/// </pre>
/// For further examples, see the<a href= "package-summary.html#FunctionObjects" > package doc</a>.

/// @param y the secondary matrix to operate on.
/// @param function a function object taking as first argument the current cell's value of <i>this</i>,
/// and as second argument the current cell's value of <i>y</i>,
/// @return<i>this</i> (for convenience only).
/// @throws ArgumentException if <i>Size != y.Count</i>.
/// @see Cern.jet.math.Functions
/// </summary>
public ObjectMatrix1D assign(ObjectMatrix1D y, Cern.Colt.Function.ObjectObjectFunction<Object> function)
        {
            CheckSize(y);
            for (int i = Size; --i >= 0;)
            {
                setQuick(i, function(this[i], y[i]));
            }
            return this;
        }
        /// <summary>
        /// Sets all cells to the state specified by <i>value</i>.
        /// @param    value the value to be filled into the cells.
        /// @return <i>this</i> (for convenience only).
        /// </summary>
        public ObjectMatrix1D assign(Object value)
        {
            for (int i = Size; --i >= 0;)
            {
                setQuick(i, value);
            }
            return this;
        }
        /// <summary>
        /// Returns the number of cells having non-zero values; ignores tolerance.
        /// </summary>
        public int cardinality()
        {
            int cardinality = 0;
            for (int i = Size; --i >= 0;)
            {
                if (this[i] != null) cardinality++;
            }
            return cardinality;
        }
        /// <summary>
        /// Constructs and returns a deep copy of the receiver.
        /// <p>
        /// <b>Note that the returned matrix is an independent deep copy.</b>
        /// The returned matrix is not backed by this matrix, so changes in the returned matrix are not reflected in this matrix, and vice-versad 
        ///
        /// @return  a deep copy of the receiver.
        /// </summary>
        public ObjectMatrix1D copy()
        {
            ObjectMatrix1D copy = like();
            copy.assign(this);
            return copy;
        }
/// <summary>
/// * Compares the specified Object with the receiver for equality.
/// * Equivalent to<i> Equals(otherObj,true)</i>d  
/// *
/// * @param otherObj the Object to be compared for equality with the receiver.
/// * @return true if the specified Object is equal to the receiver.
/// </summary>
public override Boolean Equals(Object otherObj)
        { //delta
            return Equals(otherObj, true);
        }
/// <summary>
/// * Compares the specified Object with the receiver for equality.
/// * Returns true if and only if the specified Object is also at least an ObjectMatrix1D, both matrices have the
/// * same size, and all corresponding pairs of cells in the two matrices are the same.
/// * In other words, two matrices are defined to be equal if they contain the
/// * same cell values in the same order.
/// * Tests elements for equality or identity as specified by <i>testForEquality</i>.
/// * When testing for equality, two elements <i>e1</i> and
/// * <i>e2</i> are <i>equal</i> if <i>(e1==null ? e2==null :
/// * e1.Equals(e2))</i>d)  
/// *
/// * @param otherObj the Object to be compared for equality with the receiver.
/// * @param testForEquality if true -> tests for equality, otherwise for identity.
/// * @return true if the specified Object is equal to the receiver.
/// </summary>
public Boolean Equals(Object otherObj, Boolean testForEquality)
        { //delta
            if (!(otherObj is ObjectMatrix1D)) { return false; }
            if (this == otherObj) return true;
            if (otherObj == null) return false;
            ObjectMatrix1D other = (ObjectMatrix1D)otherObj;
            if (Size != other.Size) return false;

            if (!testForEquality)
            {
                for (int i = Size; --i >= 0;)
                {
                    if (this[i] != other[i]) return false;
                }
            }
            else
            {
                for (int i = Size; --i >= 0;)
                {
                    if (!(this[i] == null ? other[i] == null : this[i].Equals(other[i]))) return false;
                }
            }

            return true;

        }
        /// <summary>
        /// Returns the matrix cell value at coordinate <i>index</i>.
        ///
        /// @param     index   the index of the cell.
        /// @return    the value of the specified cell.
        /// @throws	IndexOutOfRangeException if <i>index&lt;0 || index&gt;=Size</i>.
        /// </summary>
        public Object get(int index)
        {
            if (index < 0 || index >= Size) CheckIndex(index);
            return this[index];
        }
        /// <summary>
        /// Returns the content of this matrix if it is a wrapper; or <i>this</i> otherwise.
        /// Override this method in wrappers.
        /// </summary>
        protected ObjectMatrix1D getContent()
        {
            return this;
        }
        /// <summary>
/// Fills the coordinates and values of cells having non-zero values into the specified lists.
/// Fills into the lists, starting at index 0.
/// After this call returns the specified lists all have a new size, the number of non-zero values.
/// <p>
/// In general, fill order is <i>unspecified</i>.
/// This implementation fills like: <i>for (index = 0..Count - 1)  do ..d </i>.
/// However, subclasses are free to us any other order, even an order that may change over time as cell values are changed.
/// (Of course, result lists indexes are guaranteed to correspond to the same cell).
/// <p>
/// <b>Example:</b>
/// <br>
/// <pre>
/// 0, 0, 8, 0, 7
/// -->
/// indexList  = (2,4)
/// valueList  = (8,7)
/// </pre>
/// In other words, <i>get(2)==8, get(4)==7</i>.

/// @param indexList the list to be filled with indexes, can have any size.
/// @param valueList the list to be filled with values, can have any size.
/// </summary>
public void getNonZeros(List<int> indexList, List<Object> valueList)
        {
            Boolean fillIndexList = indexList != null;
            Boolean fillValueList = valueList != null;
            if (fillIndexList) indexList.Clear();
            if (fillValueList) valueList.Clear();
            int s = Size;
            for (int i = 0; i < s; i++)
            {
                Object value = this[i];
                if (value != null)
                {
                    if (fillIndexList) indexList.Add(i);
                    if (fillValueList) valueList.Add(value);
                }
            }
        }
        /// <summary>
        /// Returns the matrix cell value at coordinate <i>index</i>.
        ///
        /// <p>Provided with invalid parameters this method may return invalid objects without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <i>index&lt;0 || index&gt;=Size</i>.
        ///
        /// @param     index   the index of the cell.
        /// @return    the value of the specified cell.
        /// </summary>
        public abstract Object this[int index] { get; set; }
        /// <summary>
        /// Returns <i>true</i> if both matrices share at least one identical cell.
        /// </summary>
        protected Boolean haveSharedCells(ObjectMatrix1D other)
        {
            if (other == null) return false;
            if (this == other) return true;
            return getContent().haveSharedCellsRaw(other.getContent());
        }
        /// <summary>
        /// Returns <i>true</i> if both matrices share at least one identical cell.
        /// </summary>
        protected Boolean haveSharedCellsRaw(ObjectMatrix1D other)
        {
            return false;
        }
        /// <summary>
        /// Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the same size.
        /// For example, if the receiver is an instance of type <i>DenseObjectMatrix1D</i> the new matrix must also be of type <i>DenseObjectMatrix1D</i>,
        /// if the receiver is an instance of type <i>SparseObjectMatrix1D</i> the new matrix must also be of type <i>SparseObjectMatrix1D</i>, etc.
        /// In general, the new matrix should have internal parametrization as similar as possible.
        ///
        /// @return  a new empty matrix of the same dynamic type.
        /// </summary>
        public ObjectMatrix1D like()
        {
            return like(Size);
        }
        /// <summary>
        /// Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the specified size.
        /// For example, if the receiver is an instance of type <i>DenseObjectMatrix1D</i> the new matrix must also be of type <i>DenseObjectMatrix1D</i>,
        /// if the receiver is an instance of type <i>SparseObjectMatrix1D</i> the new matrix must also be of type <i>SparseObjectMatrix1D</i>, etc.
        /// In general, the new matrix should have internal parametrization as similar as possible.
        ///
        /// @param size the number of cell the matrix shall have.
        /// @return  a new empty matrix of the same dynamic type.
        /// </summary>
        public abstract ObjectMatrix1D like(int size);
        /// <summary>
        /// Construct and returns a new 2-d matrix <i>of the corresponding dynamic type</i>, entirelly independent of the receiver.
        /// For example, if the receiver is an instance of type <i>DenseObjectMatrix1D</i> the new matrix must be of type <i>DenseObjectMatrix2D</i>,
        /// if the receiver is an instance of type <i>SparseObjectMatrix1D</i> the new matrix must be of type <i>SparseObjectMatrix2D</i>, etc.
        ///
        /// @param rows the number of rows the matrix shall have.
        /// @param columns the number of columns the matrix shall have.
        /// @return  a new matrix of the corresponding dynamic type.
        /// </summary>
        public abstract ObjectMatrix2D like2D(int rows, int columns);
        /// <summary>
        /// Sets the matrix cell at coordinate <i>index</i> to the specified value.
        ///
        /// @param     index   the index of the cell.
        /// @param    value the value to be filled into the specified cell.
        /// @throws	IndexOutOfRangeException if <i>index&lt;0 || index&gt;=Size</i>.
        /// </summary>
        public void set(int index, Object value)
        {
            if (index < 0 || index >= Size) CheckIndex(index);
            setQuick(index, value);
        }
        /// <summary>
        /// Sets the matrix cell at coordinate <i>index</i> to the specified value.
        ///
        /// <p>Provided with invalid parameters this method may access illegal indexes without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <i>index&lt;0 || index&gt;=Size</i>.
        ///
        /// @param     index   the index of the cell.
        /// @param    value the value to be filled into the specified cell.
        /// </summary>
        public abstract void setQuick(int index, Object value);
        /// <summary>
/// Swaps each element<i> this[i]</i> with <i>other [i]</i>.
/// @throws ArgumentException if <i>Size != other.Count</i>.
/// </summary>
public void swap(ObjectMatrix1D other)
        {
            CheckSize(other);
            for (int i = Size; --i >= 0;)
            {
                Object tmp = this[i];
                setQuick(i, other[i]);
                other.setQuick(i, tmp);
            }
            return;
        }
        /// <summary>
/// Constructs and returns a 1-dimensional array containing the cell values.
/// The values are copied.So subsequent changes in <i>values</i> are not reflected in the matrix, and vice-versa.
/// The returned array<i> values</i> has the form
/// <br>
/// <i>for (int i= 0; i<Size; i++) values[i] = get(i);</i>

/// @return an array filled with the values of the cells.
/// </summary>
public Object[] ToArray()
        {
            Object[] values = new Object[Size];
            ToArray(values);
            return values;
        }
        /// <summary>
/// Fills the cell values into the specified 1-dimensional array.
/// The values are copied.So subsequent changes in <i>values</i> are not reflected in the matrix, and vice-versa.
/// After this call returns the array <i>values</i> has the form 
/// <br>
/// <i>for (int i= 0; i<Size; i++) values[i] = get(i);</i>

/// @throws ArgumentException if <i>values.Length<Size</i>.
/// </summary>
public void ToArray(Object[] values)
        {
            if (values.Length < Size) throw new ArgumentException("values too small");
            for (int i = Size; --i >= 0;)
            {
                values[i] = this[i];
            }
        }
        /// <summary>
        /// Returns a string representation using default formatting.
        /// @see Cern.Colt.Matrix.objectalgo.Formatter
        /// </summary>
        public override String ToString()
        {
            return new Cern.Colt.Matrix.objectalgo.Formatter().ToString(this);
        }
        /// <summary>
        /// Constructs and returns a new view equal to the receiver.
        /// The view is a shallow cloned Calls <code>clone()</code> and casts the result.
        /// <p>
        /// <b>Note that the view is not a deep copy.</b>
        /// The returned matrix is backed by this matrix, so changes in the returned matrix are reflected in this matrix, and vice-versad 
        /// <p>
        /// Use {@link #copy()} to construct an independent deep copy rather than a new view.
        ///
        /// @return  a new view of the receiver.
        /// </summary>
        protected ObjectMatrix1D view()
        {
            return (ObjectMatrix1D)Clone();
        }
        /// <summary>
/// Constructs and returns a new <i>flip view</i>.
/// What used to be index<i>0</i> is now index <i>Size-1</i>, ..d, what used to be index<i> Size-1</i> is now index <i>0</i>.
/// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.

/// @return a new flip view.
/// </summary>
public ObjectMatrix1D viewFlip()
        {
            return (ObjectMatrix1D)(view().VFlip());
        }
        /// <summary>
/// Constructs and returns a new <i>sub-range view</i> that is a<i> width</i> sub matrix starting at <i>index</i>.

/// Operations on the returned view can only be applied to the restricted range.
/// Any attempt to access coordinates not contained in the view will throw an<i> IndexOutOfRangeException</i>.
/// <p>
/// <b>Note that the view is really just a range restriction:</b> 
/// The returned matrix is backed by this matrix, so changes in the returned matrix are reflected in this matrix, and vice-versa.
/// <p>
/// The view contains the cells from <i>index..index+width-1</i>.
/// and has <i>view.Count == width</i>.
/// A view's legal coordinates are again zero based, as usual.
/// In other words, legal coordinates of the view are <i>0 .d view.Count-1==width-1</i>.
/// As usual, any attempt to access a cell at other coordinates will throw an<i> IndexOutOfRangeException</i>.


/// @param index   The index of the first cell.
/// @param width   The width of the range.
/// @throws IndexOutOfRangeException if <i>index<0 || width<0 || index+width>Size</i>.
/// @return the new view.

/// </summary>
public ObjectMatrix1D viewPart(int index, int width)
        {
            return (ObjectMatrix1D)(view().VPart(index, width));
        }
        /// <summary>
/// Constructs and returns a new <i>selection view</i> that is a matrix holding the indicated cells.
/// There holds <i>view.Count == indexes.Length</i> and <i>view.Get(i) == this.Get(indexes[i])</i>.
/// Indexes can occur multiple times and can be in arbitrary order.
/// <p>
/// <b>Example:</b>
/// <br>
/// <pre>
/// this     = (0, 0, 8, 0, 7)
/// indexes  = (0, 2, 4, 2)
/// -- >
/// view = (0, 8, 7, 8)
/// </ pre >
/// Note that modifying <i>indexes</i> after this call has returned has no effect on the view.
/// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.

/// @param indexes   The indexes of the cells that shall be visible in the new view.To indicate that<i> all</i> cells shall be visible, simply set this parameter to <i>null</i>.
/// @return the new view.
/// @throws IndexOutOfRangeException if <i>!(0 <= indexes[i] < Size)</i> for any<i> i = 0..indexes.Length() - 1 </ tt >.
/// </summary>
public ObjectMatrix1D viewSelection(int[] indexes)
        {
            // check for "all"
            if (indexes == null)
            {
                indexes = new int[Size];
                for (int i = Size; --i >= 0;) indexes[i] = i;
            }

            CheckIndexes(indexes);
            int[] offsets = new int[indexes.Length];
            for (int i = indexes.Length; --i >= 0;)
            {
                offsets[i] = Index(indexes[i]);
            }
            return viewSelectionLike(offsets);
        }
        /// <summary>
/// Constructs and returns a new <i>selection view</i> that is a matrix holding the cells matching the given condition.
/// Applies the condition to each cell and takes only those cells where <i>condition.apply(get(i))</i> yields<i>true</i>.
/// <p>
/// <b>Example:</b>
/// <br>
/// <pre>
/// // extract and view all cells with even value
/// matrix = 0 1 2 3 
/// matrix.viewSelection(
/// &nbsp;&nbsp;&nbsp;new ObjectProcedure()
/// {
/// &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;public Boolean apply(Object a) { return a % 2 == 0; }
/// &nbsp;&nbsp;&nbsp;}
/// );
/// -->
/// matrix ==  0 2
/// </pre>
/// For further examples, see the<a href="package-summary.html#FunctionObjects">package doc</a>.
/// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.

/// @param condition The condition to be matched.
/// @return the new view.
/// </summary>
public ObjectMatrix1D viewSelection(Cern.Colt.Function.ObjectProcedure<Object> condition)
    {
        List<int> matches = new List<int>();
        for (int i = 0; i < Size; i++)
        {
            if (condition(this[i])) matches.Add(i);
        }
        matches.TrimExcess();
        return viewSelection(matches.ToArray());
    }
    /// <summary>
    /// Construct and returns a new selection view.
    ///
    /// @param offsets the offsets of the visible elements.
    /// @return  a new view.
    /// </summary>
    protected abstract ObjectMatrix1D viewSelectionLike(int[] offsets);
    /// <summary>
/// Sorts the vector into ascending order, according to the<i>natural ordering</i>.
/// This sort is guaranteed to be<i> stable</i>.
/// For further information, see {
/// @link Cern.Colt.Matrix.objectalgo.Sorting#sort(ObjectMatrix1D)}.
/// For more advanced sorting functionality, see { @link Cern.Colt.Matrix.objectalgo.Sorting}.
/// @return a new sorted vector(matrix) view.
    /// </summary>
public ObjectMatrix1D viewSorted()
        {
            return Cern.Colt.Matrix.objectalgo.Sorting.mergeSort.sort(this);
        }
        /// <summary>
        /// Constructs and returns a new < i > stride view </ i > which is a sub matrix consisting of every i - th cell.
        /// More specifically, the view has size < tt > this.Count / stride </ tt > holding cells < tt > this.Get(i * stride) </ tt > for all<i> i = 0..Count / stride - 1 </ tt >.
                                

        /// @param  stride  the step factor.
        /// @throws IndexOutOfRangeException if < tt > stride <= 0 </ tt >.
        /// @return the new view.
    
/// </summary>
        public ObjectMatrix1D viewStrides(int stride)
        {
            return (ObjectMatrix1D)(view().VStrides(stride));
        }
        /// <summary>
        /// Applies a procedure to each cell's value.
        /// Iterates downwards from <i>[Size-1]</i> to <i>[0]</i>,
        /// as demonstrated by this snippet:
        /// <pre>
        /// for (int i=Size; --i >=0;) {
        /// if (!procedure.apply(this[i])) return false;
        /// }
        /// return true;
        /// </pre>
        /// Note that an implementation may use more efficient techniques, but must not use any other order.
        ///
        /// @param procedure a procedure object taking as argument the current cell's valued Stops iteration if the procedure returns <i>false</i>, otherwise continuesd 
        /// @return <i>false</i> if the procedure stopped before all elements where iterated over, <i>true</i> otherwised 
        /// </summary>
        private Boolean xforEach(Cern.Colt.Function.ObjectProcedure<Object> procedure)
        {
            for (int i = Size; --i >= 0;)
            {
                if (!procedure(this[i])) return false;
            }
            return true;
        }
        #endregion

        #region Local Private Methods

        #endregion

    }
}
