using System;
using System.Linq;
using JetBrains.Annotations;
using AD.TariffSets.Records;

namespace AD.TariffSets
{
    /// <summary>
    /// Extension methods to join sets of preferential tariffs with Most Favored Nation tariffs.
    /// </summary>
    [PublicAPI]
    public static class SelectTariffsExtenstions
    {
        /// <summary>
        /// Selects PRF tariffs where available, otherwise fills in with MFN tariffs.
        /// </summary>
        /// <param name="regionMap">
        /// A set of <see cref="ConcordanceRecord"/> objects mapping from ISO 3166-1 numeric codes to ISO 3166-1 alpha-3 codes and user-defined regions
        /// </param>
        /// <param name="mfn">
        /// A set of <see cref="MfnTariffRecord"/> objects.
        /// </param>
        /// <param name="prf">
        /// A set of <see cref="PrfTariffRecord"/> objects.
        /// </param>
        /// <returns>
        /// A set of <see cref="BilateralTariffRecord"/> objects resulting from a partial-key union of PRF and MFN rates.
        /// </returns>
        [Pure]
        [NotNull]
        [ItemNotNull]
        [CollectionAccess(CollectionAccessType.Read)]
        public static ParallelQuery<BilateralTariffRecord> SelectTariffs([NotNull] this ParallelQuery<ConcordanceRecord> regionMap, [NotNull] ParallelQuery<MfnTariffRecord> mfn, [NotNull] ParallelQuery<PrfTariffRecord> prf)
        {
            if (regionMap is null)
            {
                throw new ArgumentNullException(nameof(regionMap));
            }
            if (mfn is null)
            {
                throw new ArgumentNullException(nameof(mfn));
            }
            if (prf is null)
            {
                throw new ArgumentNullException(nameof(prf));
            }
            
            ParallelQuery<BilateralTariffRecord> prf2 =
                prf.GroupJoin(
                       regionMap,
                       x => x.ReporterIsoNumeric,
                       c => c.Numeric3,
                       (x, c) => new { x, c })
                   .SelectMany(
                       x => x.c.DefaultIfEmpty(new ConcordanceRecord()),
                       (x, c) => new
                       {
                           ReporterIso3 = c.Alpha3,
                           ReporterRegion = c.Region,
                           x.x.PartnerIsoNumeric,
                           x.x.Year,
                           x.x.Product,
                           x.x.Tariff
                       })
                   .GroupJoin(
                       regionMap,
                       x => x.PartnerIsoNumeric,
                       c => c.Numeric3,
                       (x, c) => new { x, c })
                   .SelectMany(
                       x => x.c.DefaultIfEmpty(new ConcordanceRecord()),
                       (x, c) =>
                           new BilateralTariffRecord(
                               reporterIso3: x.x.ReporterIso3,
                               partnerIso3: c.Alpha3,
                               reporterRegion: x.x.ReporterRegion,
                               partnerRegion: c.Region,
                               type: "PRF",
                               year: x.x.Year,
                               product: x.x.Product,
                               tariff: x.x.Tariff));

            var mfn2 =
                mfn.GroupJoin(
                       regionMap,
                       x => x.ReporterIsoNumeric,
                       c => c.Numeric3,
                       (x, c) => new { x, c })
                   .SelectMany(
                       x => x.c.DefaultIfEmpty(new ConcordanceRecord()),
                       (x, c) => new
                       {
                           ReporterIso3 = c.Alpha3,
                           ReporterRegion = c.Region,
                           x.x.Year,
                           x.x.Product,
                           x.x.Tariff
                       })
                   .ToArray()
                   .AsParallel();

            ParallelQuery<BilateralTariffRecord> squareMfn =
                regionMap.Aggregate(
                    ParallelEnumerable.Empty<BilateralTariffRecord>,
                    (current, next) =>
                        current.Concat(
                            mfn2.Where(x => x.ReporterIso3 != next.Alpha3)
                                .Select(
                                    x =>
                                        new BilateralTariffRecord(
                                            reporterIso3: x.ReporterIso3,
                                            partnerIso3: next.Alpha3,
                                            reporterRegion: x.ReporterRegion,
                                            partnerRegion: next.Region,
                                            type: "MFN",
                                            year: x.Year,
                                            product: x.Product,
                                            tariff: x.Tariff))),
                    (main, local) =>
                        main.Concat(local),
                    result =>
                        result.Where(x => !string.IsNullOrEmpty(x.ReporterIso3) && !string.IsNullOrEmpty(x.PartnerIso3))
                              .Where(x => !string.IsNullOrEmpty(x.ReporterRegion) && !string.IsNullOrEmpty(x.PartnerRegion)));

            return prf2.Union(squareMfn, BilateralTariffRecord.SetEqualityComparer);
        }
    }
}