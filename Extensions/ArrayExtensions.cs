using System;

namespace Hull.Extensions {
    public static class ArrayExtensions {
        public delegate void ForEachIterator<X>(X element);

        public static void ForEach<X>(this X[] array, ForEachIterator<X> iterator) {
            for (var i = 0; i < array.Length; i++) {
                iterator(array[i]);
            }
        }

        public delegate void ForEachIndexedIterator<X>(X element, int index);

        public static void ForEach<X>(this X[] array, ForEachIndexedIterator<X> iterator) {
            for (var i = 0; i < array.Length; i++) {
                iterator(array[i], i);
            }
        }

        public delegate Y MapIterator<X, Y>(X element);

        public static Y[] Map<X, Y>(this X[] array, MapIterator<X, Y> iterator) {
            var result = new Y[array.Length];
            for (var i = 0; i < array.Length; i++) {
                result[i] = iterator(array[i]);
            }
            return result;
        }

        public delegate Y MapIndexedIterator<X, Y>(X element, int index);

        public static Y[] Map<X, Y>(this X[] array, MapIndexedIterator<X, Y> iterator) {
            var result = new Y[array.Length];
            for (var i = 0; i < array.Length; i++) {
                result[i] = iterator(array[i], i);
            }
            return result;
        }

        public delegate bool FilterIterator<X>(X element);

        public static X[] Filter<X>(this X[] array, FilterIterator<X> iterator) {
            var result = new X[array.Length];
            var count = 0;
            for (var i = 0; i < array.Length; i++) {
                var element = array[i];
                if (iterator(element)) {
                    result[count++] = element;
                }
            }

            if (count == result.Length) {
                return result;
            }

            var cropped = new X[count];
            Array.Copy(result, cropped, count);
            return cropped;
        }

        public delegate bool FilterIndexedIterator<X>(X element, int index);

        public static X[] Filter<X>(this X[] array, FilterIndexedIterator<X> iterator) {
            var result = new X[array.Length];
            var count = 0;
            for (var i = 0; i < array.Length; i++) {
                var element = array[i];
                if (iterator(element, i)) {
                    result[count++] = element;
                }
            }

            if (count == result.Length) {
                return result;
            }

            var cropped = new X[count];
            Array.Copy(result, cropped, count);
            return cropped;
        }

        public delegate Y ReduceIterator<X, Y>(Y accumulator, X element);

        public static Y Reduce<X, Y>(this X[] array, Y accumulator, ReduceIterator<X, Y> iterator) {
            for (var i = 0; i < array.Length; i++) {
                accumulator = iterator(accumulator, array[i]);
            }
            return accumulator;
        }

        public delegate Y ReduceIndexedIterator<X, Y>(Y accumulator, X element, int index);

        public static Y Reduce<X, Y>(this X[] array, Y accumulator, ReduceIndexedIterator<X, Y> iterator) {
            for (var i = 0; i < array.Length; i++) {
                accumulator = iterator(accumulator, array[i], i);
            }
            return accumulator;
        }

        public delegate void AccumulateIterator<X, Y>(Y accumulator, X element);

        public static Y Accumulate<X, Y>(this X[] array, Y accumulator, AccumulateIterator<X, Y> iterator) {
            for (var i = 0; i < array.Length; i++) {
                iterator(accumulator, array[i]);
            }
            return accumulator;
        }

        public delegate Y AccumulateIndexedIterator<X, Y>(Y accumulator, X element, int index);

        public static Y Accumulate<X, Y>(this X[] array, Y accumulator, AccumulateIndexedIterator<X, Y> iterator) {
            for (var i = 0; i < array.Length; i++) {
                iterator(accumulator, array[i], i);
            }
            return accumulator;
        }

        public static int IndexOf<X>(this X[] array, X item) {
            for (var i = 0; i < array.Length; i++) {
                var element = array[i];
                if (item == null) {
                    if (element == null) {
                        return i;
                    }
                }
                else {
                    if (item.Equals(element)) {
                        return i;
                    }
                }
            }
            return -1;
        }

        public static int IndexOf<X>(this X[] array, FilterIterator<X> iterator) {
            for (var i = 0; i < array.Length; i++) {
                if (iterator(array[i])) {
                    return i;
                }
            }
            return -1;
        }

        public static int LastIndexOf<X>(this X[] array, X item) {
            for (var i = array.Length - 1; i >= 0; i--) {
                var element = array[i];
                if (item == null) {
                    if (element == null) {
                        return i;
                    }
                }
                else {
                    if (item.Equals(element)) {
                        return i;
                    }
                }
            }
            return -1;
        }

        public static int LastIndexOf<X>(this X[] array, FilterIterator<X> iterator) {
            for (var i = array.Length - 1; i >= 0; i--) {
                if (iterator(array[i])) {
                    return i;
                }
            }
            return -1;
        }

        public static bool Contains<X>(this X[] array, X item) {
            return array.IndexOf(item) != -1;
        }

        public static bool Contains<X>(this X[] array, FilterIterator<X> iterator) {
            return array.IndexOf(iterator) != -1;
        }

        public static X Find<X>(this X[] array, FilterIterator<X> iterator) {
            for (var i = 0; i < array.Length; i++) {
                var element = array[i];
                if (iterator(element)) {
                    return element;
                }
            }
            return default(X);
        }

        public static X First<X>(this X[] array) {
            if (array.Length > 0) {
                return array[0];
            }
            return default(X);
        }

        public static X Last<X>(this X[] array) {
            if (array.Length > 0) {
                return array[array.Length - 1];
            }
            return default(X);
        }

        public static X[] Sort<X>(this X[] array, Comparison<X> comparer) {
            var result = new X[array.Length];
            Array.Copy(array, result, array.Length);
            Array.Sort(result, comparer);
            return result;
        }

        public static X[] Replace<X>(this X[] array, X replacement, FilterIterator<X> iterator) {
            var result = new X[array.Length];

            for (var i = 0; i < array.Length; i++) {
                var element = array[i];
                if (iterator(element)) {
                    result[i] = replacement;
                }
                else {
                    result[i] = element;
                }
            }

            return result;
        }

        public static X[] Unique<X>(this X[] array) {
            return array.Filter((x, i) => array.LastIndexOf(x) == i);
        }

        public static X[] Merge<X>(this X[][] array) {
            var result = new X[array.Reduce(0, (sum, arr) => sum + arr.Length)];
            var index = 0;
            for (var i = 0; i < array.Length; i++) {
                var arr = array[i];
                for (var j = 0; j < arr.Length; j++) {
                    result[index++] = arr[j];
                }
            }
            return result;
        }

        public static X[] Concat<X>(this X[] array, X[] other) {
            var result = new X[array.Length + other.Length];
            var index = 0;
            for (var i = 0; i < array.Length; i++) {
                result[index++] = array[i];
            }
            for (var i = 0; i < other.Length; i++) {
                result[index++] = other[i];
            }
            return result;
        }

        public static X[] Exclude<X>(this X[] array, X[] other) {
            return array.Filter(item => !other.Contains(item));
        }

        public static X[] Push<X>(this X[] array, X item) {
            var result = new X[array.Length + 1];
            array.CopyTo(result, 0);
            result[array.Length] = item;
            return result;
        }

        public static X[] Unshift<X>(this X[] array, X item) {
            var result = new X[array.Length + 1];
            array.CopyTo(result, 1);
            result[0] = item;
            return result;
        }

        public static X[] Remove<X>(this X[] array, int index) {
            var result = new X[array.Length - 1];
            var idx = 0;
            for (var i = 0; i < array.Length; i++) {
                if (index != i) {
                    result[idx++] = array[i];
                }
            }
            return result;
        }

        public static bool Intersects<X>(this X[] array, X[] other) {
            if (array.Length == 0 || other.Length == 0) {
                return false;
            }
            for (var i = 0; i < array.Length; i++) {
                if (other.Contains(array[i])) {
                    return true;
                }
            }
            return false;
        }
    }
}