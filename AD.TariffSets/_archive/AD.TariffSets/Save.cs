using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using AD.IO.Standard;

namespace AD.TariffSets
{
    /// <summary>
    /// Extension methods to write records into files.
    /// </summary>
    [PublicAPI]
    public static class SaveExtension
    {
        /// <summary>
        /// Writes records to the <see cref="DelimitedFilePath"/> using the specified file encoding and append behavior.
        /// </summary>
        /// <typeparam name="T">
        /// The type of record. This type should override the <see cref="Object.ToString"/> method with an appropriate delimited string representation.
        /// </typeparam>
        /// <param name="source">
        /// The collection of records to be written to the <see cref="DelimitedFilePath"/>.
        /// </param>
        /// <param name="delimitedFilePath">
        /// The file to which records are written.
        /// </param>
        /// <param name="append">
        /// True to append records if the file already exists; false to overwrite.
        /// </param>
        /// <param name="encoding">
        /// The file encoding to be used. <see cref="Encoding.UTF8"/> is recommended.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if any of the arguments are null, or if any item in <paramref name="source"/> is null. 
        /// </exception>
        [NotNull]
        [CollectionAccess(CollectionAccessType.Read)]
        public static async Task SaveAsync<T>([NotNull][ItemNotNull] this IEnumerable<T> source, [NotNull] DelimitedFilePath delimitedFilePath, bool append = false, [CanBeNull] Encoding encoding = null)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (delimitedFilePath is null)
            {
                throw new ArgumentNullException(nameof(delimitedFilePath));
            }

            await Console.Out.WriteLineAsync($"{DateTime.Now}: Writing to {delimitedFilePath}.");

            using (Stream stream = new FileStream(delimitedFilePath, append ? FileMode.Append : FileMode.Truncate, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter writer = new StreamWriter(stream, encoding ?? Encoding.UTF8))
                {
                    foreach (T record in source)
                    {
                        await writer.WriteLineAsync(record.ToString());
                    }
                }
            }

            await Console.Out.WriteLineAsync($"{DateTime.Now}: Completed writing to {delimitedFilePath}.");
        }
    }
}