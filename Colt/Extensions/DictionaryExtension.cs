using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cern.Colt;

namespace System.Collections.Generic
{
    public static class DictionaryExtension
    {

        public static void AddOrUpdate<T1, T2>(this Dictionary<T1, T2> originalDictionary, T1 key, T2 value)
        {
            if (originalDictionary.ContainsKey(key))
                originalDictionary[key] = value;
            else
                originalDictionary.Add(key, value);
        }

        public static void AddOrUpdateAll<T1, T2>(this Dictionary<T1, T2> originalDictionary, IEnumerable<KeyValuePair<T1, T2>> items)
        {
            foreach (var item in items)
            {
                if (originalDictionary.ContainsKey(item.Key))
                    originalDictionary[item.Key] = item.Value;
                else
                    originalDictionary.Add(item.Key, item.Value);
            }
        }

        /// <summary>
        /// Ensures that the receiver can hold at least the specified number of elements without needing to allocate new internal memory.
        /// If necessary, allocates new internal memory and increases the capacity of the receiver.
        /// <p>
        /// This method never need be called; it is for performance tuning only.
        /// Calling this method before<tt>put()</tt>ing a large number of associations boosts performance,
        /// because the receiver will grow only once instead of potentially many times.
        /// <p>
        /// <b>This default implementation does nothing.</b> Override this method if necessary.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic">Target <see cref="IDictionary{TKey, TValue}"/> object</param>
        /// <param name="minCapacity">the desired minimum capacity.</param>
        /// <returns><see cref="IDictionary{TKey, TValue}"/> object that allocated size</returns>
        public static IDictionary<TKey, TValue> EnsureCapacity<TKey, TValue>(this IDictionary<TKey, TValue> dic, int minCapacity)
        {
            if (dic.Count >= minCapacity)
                return dic;
            else
            {
                Dictionary<TKey, TValue> newDic = new Dictionary<TKey, TValue>(minCapacity);
                foreach (KeyValuePair<TKey, TValue> pair in dic)
                {
                    newDic.Add(pair.Key, pair.Value);
                }

                return (IDictionary<TKey, TValue>)newDic;
            }
        }

        /// <summary>
        /// Chooses a new prime table capacity optimized for growing that (approximately) satisfies the invariant
        /// c * minLoadFactor &lt;= size &lt;= c * maxLoadFactor
        /// and has at least one FREE slot for the given size.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="size"></param>
        /// <param name="minLoad"></param>
        /// <param name="maxLoad"></param>
        /// <returns></returns>
        public static int ChooseGrowCapacity<TKey, TValue>(this IDictionary<TKey, TValue> dic, int size, double minLoad, double maxLoad)
        {
            return PrimeFinder.NextPrime(System.Math.Max(size + 1, (int)((4 * size / (3 * minLoad + maxLoad)))));
        }

        /// <summary>
        /// Returns new high water mark threshold based on current capacity and maxLoadFactor.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="capacity"></param>
        /// <param name="maxLoad"></param>
        /// <returns>the new threshold.</returns>
        public static int ChooseHighWaterMark<TKey, TValue>(this IDictionary<TKey, TValue> dic, int capacity, double maxLoad)
        {
            return System.Math.Min(capacity - 2, (int)(capacity * maxLoad)); //makes sure there is always at least one FREE slot
        }

        /// <summary>
        /// Returns new low water mark threshold based on current capacity and minLoadFactor.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="capacity"></param>
        /// <param name="minLoad"></param>
        /// <returns>the new threshold.</returns>
        public static int ChooseLowWaterMark<TKey, TValue>(this IDictionary<TKey, TValue> dic, int capacity, double minLoad)
        {
            return (int)(capacity * minLoad);
        }

        /// <summary>
        /// Chooses a new prime table capacity neither favoring shrinking nor growing,
        /// that (approximately) satisfies the invariant
        /// c * minLoadFactor &lt;= size &lt;= c * maxLoadFactor
        /// and has at least one FREE slot for the given size.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="size"></param>
        /// <param name="minLoad"></param>
        /// <param name="maxLoad"></param>
        /// <returns></returns>
        public static int ChooseMeanCapacity<TKey, TValue>(this IDictionary<TKey, TValue> dic, int size, double minLoad, double maxLoad)
        {
            return PrimeFinder.NextPrime(System.Math.Max(size + 1, (int)((2 * size / (minLoad + maxLoad)))));
        }

        /// <summary>
        /// Chooses a new prime table capacity optimized for shrinking that (approximately) satisfies the invariant
        /// c * minLoadFactor &lt;= size &lt;= c * maxLoadFactor
        /// and has at least one FREE slot for the given size.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="size"></param>
        /// <param name="minLoad"></param>
        /// <param name="maxLoad"></param>
        /// <returns></returns>
        public static int ChooseShrinkCapacity<TKey, TValue>(this IDictionary<TKey, TValue> dic, int size, double minLoad, double maxLoad)
        {
            return PrimeFinder.NextPrime(System.Math.Max(size + 1, (int)((4 * size / (minLoad + 3 * maxLoad)))));
        }

        /// <summary>
        /// Trim the excess items from the Dictionary
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> TrimExcess<TKey, TValue>(this IDictionary<TKey, TValue> dic)
        {
            var kv = new KeyValuePair<TKey, TValue>[dic.Count];
            dic.CopyTo(kv, 0);
            List < KeyValuePair < TKey, TValue >> l = kv.ToList();
            l.TrimExcess();
            var newDic = new Dictionary<TKey, TValue>(l.Count);
            foreach (var p in l)
            {
                newDic.Add(p.Key, p.Value);
            }
            return newDic;
        }
    }
}
