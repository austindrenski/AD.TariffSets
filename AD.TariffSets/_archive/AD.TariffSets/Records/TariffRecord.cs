using System;
using JetBrains.Annotations;

namespace AD.TariffSets.Records
{
    /// <summary>
    /// Defines base characteristics for tariff records.
    /// </summary>
    [PublicAPI]
    public abstract class TariffRecord : IEquatable<TariffRecord>
    {
        /// <summary>
        /// Provides a key selector function for grouping operations.
        /// </summary>
        [NotNull]
        public abstract Func<TariffRecord, (string, string)> GroupByKeySelector { get; }

        /// <summary>
        /// The tariff year of this record.
        /// </summary>
        [CanBeNull]
        public int? Year { get; }

        /// <summary>
        /// The product to which the tariff rate is applied.
        /// </summary>
        [CanBeNull]
        public string Product { get; }

        /// <summary>
        /// The rate applied to imports of the product into the importing country from the exporting country.
        /// </summary>
        [CanBeNull]
        public double? Tariff { get; }

        /// <summary>
        /// Base class constructor for various tariff record types.
        /// </summary>
        /// <param name="year">
        /// The tariff year of this record.
        /// </param>
        /// <param name="product">
        /// The product to which the tariff rate is applied.
        /// </param>
        /// <param name="tariff">
        /// The rate applied to imports of the product into the importing country from the exporting country.
        /// </param>
        protected TariffRecord([CanBeNull] int? year, [CanBeNull] string product, [CanBeNull] double? tariff)
        {
            Year = year;
            Product = product;
            Tariff = tariff;
        }

        /// <summary>
        /// Returns a string that represents this <see cref="TariffRecord"/> = Year|Product|Tariff.
        /// </summary>
        /// <returns>
        /// A pipe-delimited string that represents this <see cref="TariffRecord"/>.
        /// </returns>
        [Pure]
        public override string ToString()
        {
            return $"{Year}|{Product}|{Tariff}";
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>
        /// A hash code for this <see cref="TariffRecord"/>.
        /// </returns>
        [Pure]
        public override int GetHashCode()
        {
            unchecked
            {
                return
                    397 * (Year?.GetHashCode() ?? 0) +
                    397 * (Product?.GetHashCode() ?? 0) +
                    397 * (Tariff?.GetHashCode() ?? 0);
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="TariffRecord"/> is equal to this <see cref="TariffRecord"/>.
        /// </summary>
        /// <param name="obj">
        /// The <see cref="TariffRecord"/> to compare with this <see cref="TariffRecord"/>.
        /// </param>
        /// <returns>
        /// True if the specified <see cref="TariffRecord"/> objects are equal; otherwise, false.
        /// </returns>
        [Pure]
        public override bool Equals(object obj)
        {
            return Equals(obj as TariffRecord);
        }

        /// <summary>
        /// Indicates whether this is equal to another <see cref="TariffRecord"/>.
        /// </summary>
        /// <param name="other">
        /// A <see cref="TariffRecord"/> to compare with this <see cref="TariffRecord"/>.
        /// </param>
        /// <returns>
        /// True if the specified <see cref="TariffRecord"/> objects are equal; otherwise, false.
        /// </returns>
        [Pure]
        public bool Equals(TariffRecord other)
        {
            if (other is null)
            {
                return false;
            }

            return
                (Year?.Equals(other.Year) ?? false) &&
                (Product?.Equals(other.Product, StringComparison.OrdinalIgnoreCase) ?? false) &&
                (Tariff?.Equals(other.Tariff) ?? false);
        }
    }
}