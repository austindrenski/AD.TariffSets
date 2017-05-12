using System;
using JetBrains.Annotations;

namespace AD.TariffSets.Records
{
    /// <summary>
    /// Represents a unilateral Most Favored Nation (MFN) tariff imposed by the reporter on imports from all partners.
    /// </summary>
    [PublicAPI]
    public sealed class MfnTariffRecord : TariffRecord, IEquatable<MfnTariffRecord>
    {
        /// <summary>
        /// Provides a key selector function for grouping by <see cref="ReporterIsoNumeric"/>.
        /// </summary>
        public override Func<TariffRecord, (string, string)> GroupByKeySelector { get; }

        /// <summary>
        /// The ISO 3166-1 numeric code for the importing country.
        /// </summary>
        [CanBeNull]
        public string ReporterIsoNumeric { get; }
        
        /// <summary>
        /// Constructs an <see cref="MfnTariffRecord"/> that represents the unilateral tariff rate for a single product between the importing country and all partners.
        /// </summary>
        /// <param name="reporterIsoNumeric">
        /// The ISO 3166-1 numeric code for the importing country.
        /// </param>
        /// <param name="year">
        /// The tariff year of this record.
        /// </param>
        /// <param name="product">
        /// The product to which the tariff rate is applied.
        /// </param>
        /// <param name="tariff">
        /// The rate applied to imports of the product into the importing country from the exporting country.
        /// </param>
        public MfnTariffRecord([CanBeNull] string reporterIsoNumeric, [CanBeNull] int? year, [CanBeNull] string product, [CanBeNull] double? tariff) : base(year, product, tariff)
        {
            ReporterIsoNumeric = reporterIsoNumeric;
            GroupByKeySelector = x => x is MfnTariffRecord y ? (y.ReporterIsoNumeric, default(string)) : throw new InvalidCastException(nameof(x));
        }

        /// <summary>
        /// Returns a string that represents this <see cref="MfnTariffRecord"/> = ReporterIsoNumeric|Year|Product|Tariff.
        /// </summary>
        /// <returns>
        /// A pipe-delimited string that represents this <see cref="MfnTariffRecord"/>.
        /// </returns>
        [Pure]
        public override string ToString()
        {
            return $"{ReporterIsoNumeric}|{base.ToString()}";
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>
        /// A hash code for this <see cref="MfnTariffRecord"/>.
        /// </returns>
        [Pure]
        public override int GetHashCode()
        {
            unchecked
            {
                return
                    base.GetHashCode() +
                    397 * (ReporterIsoNumeric?.GetHashCode() ?? 0);
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="MfnTariffRecord"/> is equal to this <see cref="MfnTariffRecord"/>.
        /// </summary>
        /// <param name="obj">
        /// The <see cref="MfnTariffRecord"/> to compare with this <see cref="MfnTariffRecord"/>.
        /// </param>
        /// <returns>
        /// True if the specified <see cref="MfnTariffRecord"/> objects are equal; otherwise, false.
        /// </returns>
        [Pure]
        public override bool Equals(object obj)
        {
            return Equals(obj as MfnTariffRecord);
        }

        /// <summary>
        /// Indicates whether this is equal to another <see cref="MfnTariffRecord"/>.
        /// </summary>
        /// <param name="other">
        /// A <see cref="MfnTariffRecord"/> to compare with this <see cref="MfnTariffRecord"/>.
        /// </param>
        /// <returns>
        /// True if the specified <see cref="MfnTariffRecord"/> objects are equal; otherwise, false.
        /// </returns>
        [Pure]
        public bool Equals(MfnTariffRecord other)
        {
            if (other is null)
            {
                return false;
            }

            return
                base.Equals(other) &&
                (ReporterIsoNumeric?.Equals(other.ReporterIsoNumeric, StringComparison.OrdinalIgnoreCase) ?? false);
        }
    }
}