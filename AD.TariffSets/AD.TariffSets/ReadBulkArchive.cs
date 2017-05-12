using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using AD.IO;
using JetBrains.Annotations;
using AD.TariffSets.Records;

namespace AD.TariffSets
{
    /// <summary>
    /// Reads a bulk archive of inner archives containing delimited files.
    /// </summary>
    [PublicAPI]
    public static class ReadBulkArchiveExtensions
    {
        /// <summary>
        /// Reads a bulk archive of compressed archives containing delimited files. 
        /// </summary>
        /// <param name="bulkArchiveFile">
        /// A zip archive containing zip archives containing delimited files.
        /// </param>
        /// <param name="selector">
        /// A transform function to apply to each line of a file.
        /// </param>
        /// <param name="delimiter">
        /// The character delimiting values in the delimited files.
        /// </param>
        /// <param name="header">
        /// True if the delimited files have headers; otherwise false.
        /// </param>
        /// <returns>
        /// A <see cref="TariffRecord"/> collection.
        /// </returns>
        /// <remarks>
        /// The bulk archive structure:
        /// 
        /// -BulkArchive.zip
        /// ---InnerArchive0.zip
        /// -----File_0_0.csv 
        /// -----File_0_1.csv
        /// -----File_0_2.csv
        /// ---InnerArchive1.zip
        /// -----File_1_0.csv 
        /// -----File_1_1.csv
        /// -----File_1_2.csv
        /// ---InnerArchive2.zip
        /// -----File_2_0.csv 
        /// -----File_2_1.csv
        /// -----File_2_2.csv
        /// </remarks>
        [Pure]
        [NotNull]
        [ItemNotNull]
        public static ParallelQuery<TRecord> ReadBulkArchive<TRecord>([NotNull] this ZipFilePath bulkArchiveFile, [NotNull] Func<string[], TRecord> selector, char delimiter = ',', bool header = true) where TRecord : TariffRecord
        {
            if (bulkArchiveFile is null)
            {
                throw new ArgumentNullException(nameof(bulkArchiveFile));
            }
            if (selector is null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return
                ZipFile.OpenRead(bulkArchiveFile)
                       .Entries
                       .Select(
                           async x =>
                           {
                               using (Stream stream = x.Open())
                               {
                                   using (ZipArchive archive = new ZipArchive(stream))
                                   {
                                       foreach (ZipArchiveEntry entry in archive.Entries.Where(y => y.Name.EndsWith(".csv", StringComparison.OrdinalIgnoreCase)))
                                       {
                                           using (Stream innerStream = entry.Open())
                                           {
                                               using (StreamReader reader = new StreamReader(innerStream))
                                               {
                                                   return await reader.ReadToEndAsync();
                                               }
                                           }
                                       }
                                       return null;
                                   }
                               }
                           })
                       .AsParallel()
                       .Select(x => x.Result)
                       .Where(x => x != null)
                       .SelectMany(
                           x =>
                               x.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                                .Skip(header ? 1 : 0)
                                .SplitDelimitedLine(',')
                                .Select(a => a.Select(b => b.Trim()))
                                .Select(a => a.ToArray())
                                .Select(selector));
        }
    }
}
