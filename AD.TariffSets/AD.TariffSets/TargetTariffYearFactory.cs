using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AD.IO;
using JetBrains.Annotations;
using AD.TariffSets.Records;

namespace AD.TariffSets
{
    /// <summary>
    /// 
    /// </summary>
    [PublicAPI]
    public static class TargetTariffYearFactory
    {       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mfnFile"></param>
        /// <param name="prfFile"></param>
        /// <param name="regionFile"></param>
        /// <param name="outputDirectory"></param>
        /// <param name="targets"></param>
        /// <param name="predicate"></param>
        public static async Task Create([NotNull] ZipFilePath mfnFile, [NotNull] ZipFilePath prfFile, [NotNull] DelimitedFilePath regionFile, [NotNull] DirectoryPath outputDirectory, [NotNull] IEnumerable<(int minimum, int target)> targets, [CanBeNull] Func<BilateralTariffRecord, bool> predicate = null)
        {
            if (mfnFile is null)
            {
                throw new ArgumentNullException(nameof(mfnFile));
            }
            if (prfFile is null)
            {
                throw new ArgumentNullException(nameof(prfFile));
            }
            if (regionFile is null)
            {
                throw new ArgumentNullException(nameof(regionFile));
            }
            if (outputDirectory is null)
            {
                throw new ArgumentNullException(nameof(outputDirectory));
            }
            if (targets is null)
            {
                throw new ArgumentNullException(nameof(targets));
            }

            await Console.Out.WriteLineAsync($"{DateTime.Now}: Constructing resource sets of MFN and PRF tariffs, and region information.");

            ParallelQuery<MfnTariffRecord> mfn =
                mfnFile.ReadBulkArchive(
                           x =>
                               new MfnTariffRecord(
                                   reporterIsoNumeric: x[1],
                                   year: x[2].ParseInt(),
                                   product: x[3],
                                   tariff: x[7].ParseDouble()))
                       .ToImmutableArray()
                       .AsParallel();

            ParallelQuery<PrfTariffRecord> prf =
                prfFile.ReadBulkArchive(
                           x =>
                               new PrfTariffRecord(
                                   reporterIsoNumeric: x[1],
                                   partnerIsoNumeric: x[4],
                                   year: x[2].ParseInt(),
                                   product: x[3],
                                   tariff: x[9].ParseDouble()))
                       .ToImmutableArray()
                       .AsParallel();

            ParallelQuery<ConcordanceRecord> regions =
                File.ReadLines(regionFile)
                    .Skip(1)
                    .AsParallel()
                    .SplitDelimitedLine('|')
                    .Select(x => x.ToArray())
                    .Select(
                        x =>
                            new ConcordanceRecord
                            {
                                Numeric3 = x[0],
                                Alpha3 = x[1],
                                Region = x[2]
                            })
                    .Where(x => x.Alpha3 != null && x.Region != null)
                    .Distinct()
                    .ToImmutableArray()
                    .AsParallel();

            await Console.Out.WriteLineAsync($"{DateTime.Now}: Completed construction of resource sets.");

            foreach ((int minimum, int target) target in targets)
            {
                IEnumerable<BilateralTariffRecord> data =
                    regions.SelectTariffs(
                        mfn.Filter(target),
                        prf.Filter(target));

                if (predicate is null)
                {
                    await data.SaveAsync(DelimitedFilePath.Create($"{outputDirectory}\\prf_union_mfn_target_{target.target}.txt"));
                }
                else
                {
                    await data.Where(predicate).SaveAsync(DelimitedFilePath.Create($"{outputDirectory}\\prf_union_mfn_target_{target.target}.txt"));
                }
            }
        }
    }
}