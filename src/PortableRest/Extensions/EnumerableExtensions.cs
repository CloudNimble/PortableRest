using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class EnumerableExtensions
    {

        /// <summary>Applies an accumulator function over a sequence.</summary>
        /// <returns>The final accumulator value.</returns>
        /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to aggregate over.</param>
        /// <param name="func">An accumulator function to be invoked on each element.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="source" /> or <paramref name="func" /> is null.</exception>
        /// <exception cref="T:System.InvalidOperationException">
        ///   <paramref name="source" /> contains no elements.</exception>
        public static TSource Aggregate<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, TSource> func)
        {
            TSource tSource;
            if (source != null)
            {
                if (func != null)
                {
                    IEnumerator<TSource> enumerator = source.GetEnumerator();
                    using (enumerator)
                    {
                        if (enumerator.MoveNext())
                        {
                            TSource current = enumerator.Current;
                            while (enumerator.MoveNext())
                            {
                                current = func(current, enumerator.Current);
                            }
                            tSource = current;
                        }
                        else
                        {
                            throw new InvalidOperationException("The source contains no elements.");
                        }
                    }
                    return tSource;
                }
                throw new ArgumentNullException("func");
            }
            throw new ArgumentNullException("source");
        }


        /// <summary>Applies an accumulator function over a sequence. The specified seed value is used as the initial accumulator value.</summary>
        /// <returns>The final accumulator value.</returns>
        /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to aggregate over.</param>
        /// <param name="seed">The initial accumulator value.</param>
        /// <param name="func">An accumulator function to be invoked on each element.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <typeparam name="TAccumulate">The type of the accumulator value.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="source" /> or <paramref name="func" /> is null.</exception>
        public static TAccumulate Aggregate<TSource, TAccumulate>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
        {
            if (source != null)
            {
                if (func != null)
                {
                    return Enumerable.Aggregate(source, seed, func);
                }
                throw new ArgumentNullException("func");
            }
            throw new ArgumentNullException("source");
        }

        /// <summary>Applies an accumulator function over a sequence. The specified seed value is used as the initial accumulator value, and the specified function is used to select the result value.</summary>
        /// <returns>The transformed final accumulator value.</returns>
        /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to aggregate over.</param>
        /// <param name="seed">The initial accumulator value.</param>
        /// <param name="func">An accumulator function to be invoked on each element.</param>
        /// <param name="resultSelector">A function to transform the final accumulator value into the result value.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <typeparam name="TAccumulate">The type of the accumulator value.</typeparam>
        /// <typeparam name="TResult">The type of the resulting value.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="source" /> or <paramref name="func" /> or <paramref name="resultSelector" /> is null.</exception>
        public static TResult Aggregate<TSource, TAccumulate, TResult>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
        {
            if (source != null)
            {
                if (func != null)
                {
                    if (resultSelector != null)
                    {
                        TAccumulate tAccumulate = Enumerable.Aggregate(source, seed, func);
                        return resultSelector(tAccumulate);
                    }
                    throw new ArgumentNullException("resultSelector");
                }
                throw new ArgumentNullException("func");
            }
            throw new ArgumentNullException("source");
        }
    }
}
