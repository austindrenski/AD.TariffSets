using System;
using System.Linq;
using JetBrains.Annotations;
using AD.TariffSets.Records;

namespace AD.TariffSets
{
    /// <summary>
    /// Extension methods to filter tariff records.
    /// </summary>
    [PublicAPI]
    public static class FilterExtenstions
    {
        /// <summary>
        /// Filters a tariff data set for records for the target year, or the nearest previous year available.
        /// </summary>
        /// <param name="source">
        /// A <see cref="TariffRecord"/> collection.
        /// </param>
        /// <param name="target">
        /// The year of interest by which data are filtered and the minimum acceptable year.
        /// </param>
        /// <returns>
        /// A data set of tariffs in the target year, or the nearest previous year.
        /// </returns>
        [Pure]
        [NotNull]
        [ItemNotNull]
        [LinqTunnel]
        [CollectionAccess(CollectionAccessType.Read)]
        public static ParallelQuery<TRecord> Filter<TRecord>([NotNull] this ParallelQuery<TRecord> source, (int minimum, int target) target) where TRecord : TariffRecord
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return
                source.Where(x => x.Year >= target.minimum)
                      .Where(x => x.Year <= target.target)
                      .GroupBy(x => x.GroupByKeySelector)
                      .Select(
                          x => new
                          {
                              x,
                              MaxYear = x.Max(y => y.Year)
                          })
                      .SelectMany(x => x.x.Where(y => y.Year == x.MaxYear));
        }
    }
}