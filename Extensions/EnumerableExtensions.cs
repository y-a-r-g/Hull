using System;
using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace Hull.Extensions {
    public static class EnumerableExtensions {
        #region ForEach

        /// <summary>
        /// Iterator used in ForeEach loop
        /// </summary>
        /// <param name="element"></param>
        /// <typeparam name="TItem"></typeparam>
        public delegate void ForEachIterator<TItem>(TItem element);

        /// <summary>
        /// Iterates over enumerable and call iterator for every item
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="iterator"></param>
        /// <typeparam name="TItem"></typeparam>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static void ForEach<TItem>(this IEnumerable<TItem> enumerable, ForEachIterator<TItem> iterator) {
            if (enumerable == null) {
                throw new ArgumentNullException("enumerable");
            }
            if (iterator == null) {
                throw new ArgumentException("iterator");
            }

            using (var e = enumerable.GetEnumerator()) {
                while (e.MoveNext()) {
                    iterator(e.Current);
                }
            }
        }

        /// <summary>
        /// Iterator used in ForeEach loop
        /// </summary>
        /// <param name="element"></param>
        /// <param name="index"></param>
        /// <typeparam name="TItem"></typeparam>
        public delegate void ForEachIndexedIterator<TItem>(TItem element, int index);

        /// <summary>
        /// Iterates over enumerable and call iterator for every item
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="iterator"></param>
        /// <typeparam name="TItem"></typeparam>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static void ForEach<TItem>(this IEnumerable<TItem> enumerable, ForEachIndexedIterator<TItem> iterator) {
            if (enumerable == null) {
                throw new ArgumentNullException("enumerable");
            }
            if (iterator == null) {
                throw new ArgumentException("iterator");
            }

            var index = 0;
            using (var e = enumerable.GetEnumerator()) {
                while (e.MoveNext()) {
                    iterator(e.Current, index++);
                }
            }
        }

        #endregion

        #region Map

        /// <summary>
        /// Iterator used in Map loop
        /// </summary>
        /// <param name="element"></param>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        public delegate TOutput MapIterator<TInput, TOutput>(TInput element);

        /// <summary>
        /// Creates new enumerable with the same size and fills it with result of iterator call for each item
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="iterator"></param>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <returns></returns>
        public static IEnumerable<TOutput> Map<TInput, TOutput>(
                this IEnumerable<TInput> enumerable, MapIterator<TInput, TOutput> iterator
            ) {
            int count;

            if (enumerable.CountFast(out count)) {
                var result = new TOutput[count];
                enumerable.ForEach((item, index) => result[index] = iterator(item));
                return result;
            }

            var list = new List<TOutput>();
            enumerable.ForEach(item => list.Add(iterator(item)));
            return list;
        }

        /// <summary>
        /// Iterator used in Map loop
        /// </summary>
        /// <param name="element"></param>
        /// <param name="index"></param>
        /// <typeparam name="X"></typeparam>
        /// <typeparam name="Y"></typeparam>
        public delegate Y MapIndexedIterator<X, Y>(X element, int index);

        /// <summary>
        /// Creates new enumerable with the same size and fills it with result of iterator call for each item
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="iterator"></param>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <returns></returns>
        public static IEnumerable<TOutput> Map<TInput, TOutput>(
                IEnumerable<TInput> enumerable, MapIndexedIterator<TInput, TOutput> iterator
            ) {
            int count;

            if (enumerable.CountFast(out count)) {
                var result = new TOutput[count];
                enumerable.ForEach((item, index) => result[index] = iterator(item, index));
                return result;
            }

            var list = new List<TOutput>();
            enumerable.ForEach((item, index) => list.Add(iterator(item, index)));
            return list;
        }

        #endregion

        #region Utils

        /// <summary>
        /// Returns amount of items in enumerable. Note: if enumerable is not an array or list - it wil be enumerated to calculate length
        /// </summary>
        /// <param name="enumerable"></param>
        /// <typeparam name="TItem"></typeparam>
        /// <returns></returns>
        public static int Count<TItem>(this IEnumerable<TItem> enumerable) {
            int count;
            if (!enumerable.CountFast(out count)) {
                using (var e = enumerable.GetEnumerator()) {
                    while (e.MoveNext()) {
                        count++;
                    }
                }
            }

            return count;
        }

        /// <summary>
        /// Detects if the enumerable contains no elements
        /// </summary>
        /// <param name="enumerable"></param>
        /// <typeparam name="TItem"></typeparam>
        /// <returns></returns>
        public static bool Empty<TItem>(this IEnumerable<TItem> enumerable) {
            using (var e = enumerable.GetEnumerator()) {
                return !e.MoveNext();
            }
        }

        /// <summary>
        /// Assigns amount of items in enumerable to <value>count</value> only if it is available to do without enumerating 
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="count"></param>
        /// <typeparam name="TItem"></typeparam>
        /// <returns><value>true</value> if count was set correctly</returns>
        public static bool CountFast<TItem>(this IEnumerable<TItem> enumerable, out int count) {
            var array = enumerable as TItem[];
            if (array != null) {
                count = array.Length;
                return true;
            }

            var list = enumerable as IList<TItem>;
            if (list != null) {
                count = list.Count;
                return true;
            }

            count = 0;
            return false;
        }

        /// <summary>
        /// Converts enumerable to the array
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="forceCopy">If enumerable is already an array and <value>forceCopy</value> is <value>true</value> - new instance of the enumerable will be created</param>
        /// <typeparam name="TItem"></typeparam>
        /// <returns></returns>
        public static TItem[] ToArray<TItem>(this IEnumerable<TItem> enumerable, bool forceCopy = false) {
            if (!forceCopy) {
                var array = enumerable as TItem[];
                if (array != null) {
                    return array;
                }
            }

            var list = enumerable as IList<TItem>;
            if (list == null) {
                list = new List<TItem>();
                enumerable.ForEach(item => list.Add(item));
            }
            return list.ToArray();
        }

        /// <summary>
        /// Converts enumerable to the list
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="forceCopy">If enumerable is already an array and <value>forceCopy</value> is <value>true</value> - new instance of the list will be created</param>
        /// <typeparam name="TItem"></typeparam>
        /// <returns></returns>
        public static List<TItem> ToList<TItem>(this IEnumerable<TItem> enumerable, bool forceCopy = false) {
            var list = enumerable as List<TItem>;

            if (!forceCopy && (list != null)) {
                return list;
            }

            if (list != null) {
                return new List<TItem>(list);
            }

            list = new List<TItem>();
            enumerable.ForEach(item => list.Add(item));
            return list;
        }

        #endregion

        #region Filter

        /// <summary>
        /// Iterator used in For loop
        /// </summary>
        /// <param name="element"></param>
        /// <typeparam name="TItem"></typeparam>
        public delegate bool FilterIterator<TItem>(TItem element);

        /// <summary>
        /// Creates new enumerable contains only items for which <value>true</value> was returned by iterator 
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="iterator"></param>
        /// <typeparam name="TItem"></typeparam>
        /// <returns></returns>
        public static IEnumerable<TItem> Filter<TItem>(
                this IEnumerable<TItem> enumerable, FilterIterator<TItem> iterator
            ) {
            var result = new List<TItem>();
            enumerable.ForEach(
                item => {
                    if (iterator(item)) {
                        result.Add(item);
                    }
                });

            return result;
        }

        /// <summary>
        /// Iterator used in For loop
        /// </summary>
        /// <param name="element"></param>
        /// <param name="index"></param>
        /// <typeparam name="TItem"></typeparam>
        public delegate bool FilterIndexedIterator<TItem>(TItem element, int index);

        /// <summary>
        /// Creates new enumerable contains only items for which <value>true</value> was returned by iterator
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="iterator"></param>
        /// <typeparam name="TItem"></typeparam>
        /// <returns></returns>
        public static IEnumerable<TItem> Filter<TItem>(
                this IEnumerable<TItem> enumerable, FilterIndexedIterator<TItem> iterator
            ) {
            var result = new List<TItem>();
            enumerable.ForEach(
                (item, index) => {
                    if (iterator(item, index)) {
                        result.Add(item);
                    }
                });

            return result;
        }

        #endregion

        #region Reduce/Accumulate

        /// <summary>
        /// Iterator used in Reduce loop
        /// </summary>
        /// <param name="accumulator"></param>
        /// <param name="element"></param>
        /// <typeparam name="TItem"></typeparam>
        /// <typeparam name="TAccumulator"></typeparam>
        public delegate TAccumulator ReduceIterator<TItem, TAccumulator>(TAccumulator accumulator, TItem element);

        /// <summary>
        /// Iterates over enumerable calling iterator for each item and passing result of this call to the next iterator call.
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="accumulator"></param>
        /// <param name="iterator"></param>
        /// <typeparam name="TItem"></typeparam>
        /// <typeparam name="TAccumulator"></typeparam>
        /// <returns>Result of the last iterator call</returns>
        public static TAccumulator Reduce<TItem, TAccumulator>(
                this IEnumerable<TItem> enumerable,
                TAccumulator accumulator,
                ReduceIterator<TItem, TAccumulator> iterator
            ) {
            enumerable.ForEach(item => accumulator = iterator(accumulator, item));
            return accumulator;
        }

        /// <summary>
        /// Iterator used in Reduce loop
        /// </summary>
        /// <param name="accumulator"></param>
        /// <param name="element"></param>
        /// <param name="index"></param>
        /// <typeparam name="X"></typeparam>
        /// <typeparam name="Y"></typeparam>
        public delegate Y ReduceIndexedIterator<X, Y>(Y accumulator, X element, int index);

        /// <summary>
        /// Iterates over enumerable calling iterator for each item and passing result of this call to the next iterator call.
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="accumulator"></param>
        /// <param name="iterator"></param>
        /// <typeparam name="TItem"></typeparam>
        /// <typeparam name="TAccumulator"></typeparam>
        /// <returns>Result of the last iterator call</returns>
        public static TAccumulator Reduce<TItem, TAccumulator>(
                this IEnumerable<TItem> enumerable, TAccumulator accumulator,
                ReduceIndexedIterator<TItem, TAccumulator> iterator
            ) {
            enumerable.ForEach((item, index) => accumulator = iterator(accumulator, item, index));
            return accumulator;
        }

        /// <summary>
        /// Iterator used in Accumulate loop
        /// </summary>
        /// <param name="accumulator"></param>
        /// <param name="element"></param>
        /// <typeparam name="TItem"></typeparam>
        /// <typeparam name="TAccumulator"></typeparam>
        public delegate void AccumulateIterator<TItem, TAccumulator>(TAccumulator accumulator, TItem element);

        /// <summary>
        /// Iterates over collection passing same accumulator instance to every iterator call
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="accumulator"></param>
        /// <param name="iterator"></param>
        /// <typeparam name="TItem"></typeparam>
        /// <typeparam name="TAccumulator"></typeparam>
        /// <returns></returns>
        public static TAccumulator Accumulate<TItem, TAccumulator>(
                this IEnumerable<TItem> enumerable, TAccumulator accumulator,
                AccumulateIterator<TItem, TAccumulator> iterator
            ) {
            enumerable.ForEach(item => iterator(accumulator, item));
            return accumulator;
        }

        /// <summary>
        /// Iterator used in Accumulate loop
        /// </summary>
        /// <param name="accumulator"></param>
        /// <param name="element"></param>
        /// <param name="index"></param>
        /// <typeparam name="TItem"></typeparam>
        /// <typeparam name="TAccumulator"></typeparam>
        public delegate void AccumulateIndexedIterator<TItem, TAccumulator>(
            TAccumulator accumulator, TItem element, int index);

        /// <summary>
        /// Iterates over collection passing same accumulator instance to every iterator call
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="accumulator"></param>
        /// <param name="iterator"></param>
        /// <typeparam name="TItem"></typeparam>
        /// <typeparam name="TAccumulator"></typeparam>
        /// <returns></returns>
        public static TAccumulator Accumulate<TItem, TAccumulator>(
                this IEnumerable<TItem> enumerable, TAccumulator accumulator,
                AccumulateIndexedIterator<TItem, TAccumulator> iterator
            ) {
            enumerable.ForEach((item, index) => iterator(accumulator, item, index));
            return accumulator;
        }

        #endregion

        #region IndexOf

        /// <summary>
        /// Returns index of given element in the enumerable
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="item"></param>
        /// <typeparam name="TItem"></typeparam>
        /// <returns><value>-1</value> if not found</returns>
        public static int IndexOf<TItem>(this IEnumerable<TItem> enumerable, TItem item) {
            using (var e = enumerable.GetEnumerator()) {
                var index = 0;
                while (e.MoveNext()) {
                    if ((item == null) ? (e.Current == null) : item.Equals(e.Current)) {
                        return index;
                    }
                    index++;
                }
                return -1;
            }
        }

        /// <summary>
        /// Returns index of element for which iterator returned <value>true</value>
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="iterator"></param>
        /// <typeparam name="TItem"></typeparam>
        /// <returns><value>-1</value> if not found</returns>
        public static int IndexOf<TItem>(this IEnumerable<TItem> enumerable, FilterIterator<TItem> iterator) {
            using (var e = enumerable.GetEnumerator()) {
                var index = 0;
                while (e.MoveNext()) {
                    if (iterator(e.Current)) {
                        return index;
                    }
                    index++;
                }
                return -1;
            }
        }

        /// <summary>
        /// Returns index of last entrance of given element in the enumerable
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="item"></param>
        /// <typeparam name="TItem"></typeparam>
        /// <returns><value>-1</value> if not found</returns>
        public static int LastIndexOf<TItem>(this IEnumerable<TItem> enumerable, TItem item) {
            using (var e = enumerable.GetEnumerator()) {
                var result = -1;
                var index = 0;
                while (e.MoveNext()) {
                    if ((item == null) ? (e.Current == null) : item.Equals(e.Current)) {
                        result = index;
                    }
                    index++;
                }
                return result;
            }
        }

        /// <summary>
        /// Returns index of last entrance of given element for which iterator returned <value>true</value>
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="iterator"></param>
        /// <typeparam name="TItem"></typeparam>
        /// <returns><value>-1</value> if not found</returns>
        public static int LastIndexOf<TItem>(this IEnumerable<TItem> enumerable, FilterIterator<TItem> iterator) {
            using (var e = enumerable.GetEnumerator()) {
                var result = -1;
                var index = 0;
                while (e.MoveNext()) {
                    if (iterator(e.Current)) {
                        result = index;
                    }
                    index++;
                }
                return result;
            }
        }

        #endregion

        #region Find/Contains

        /// <summary>
        /// Returns <value>true</value> if item found in enumerable
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="item"></param>
        /// <typeparam name="TItem"></typeparam>
        /// <returns></returns>
        public static bool Contains<TItem>(this IEnumerable<TItem> enumerable, TItem item) {
            return enumerable.IndexOf(item) != -1;
        }

        /// <summary>
        /// Returns <value>true</value> if iterator returned true for any item
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="iterator"></param>
        /// <typeparam name="TItem"></typeparam>
        /// <returns></returns>
        public static bool Contains<TItem>(this IEnumerable<TItem> enumerable, FilterIterator<TItem> iterator) {
            return enumerable.IndexOf(iterator) != -1;
        }

        /// <summary>
        /// Returns item if iterator returned true
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="iterator"></param>
        /// <typeparam name="TItem"></typeparam>
        /// <returns></returns>
        public static TItem Find<TItem>(this IEnumerable<TItem> enumerable, FilterIterator<TItem> iterator) {
            using (var e = enumerable.GetEnumerator()) {
                while (e.MoveNext()) {
                    if (iterator(e.Current)) {
                        return e.Current;
                    }
                }
            }
            return default(TItem);
        }

        /// <summary>
        /// Returns item of the enumerable with specified index
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="index"></param>
        /// <typeparam name="TItem"></typeparam>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public static TItem At<TItem>(this IEnumerable<TItem> enumerable, int index) {
            var array = enumerable as TItem[];
            if (array != null) {
                return array[index];
            }
            var list = enumerable as IList<TItem>;
            if (list != null) {
                return list[index];
            }
            
            using (var e = enumerable.GetEnumerator()) {
                var i = 0;
                while (e.MoveNext()) {
                    if (i == index) {
                        return e.Current;
                    }
                }
            }
            throw new IndexOutOfRangeException();
        }

        #endregion
    }
}
// ReSharper restore PossibleMultipleEnumeration
