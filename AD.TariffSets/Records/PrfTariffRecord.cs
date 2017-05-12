using System;
using JetBrains.Annotations;

namespace AD.TariffSets.Records
{
    /// <summary>
    /// Represents a bilateral preferential (PRF) tariff imposed by the reporter on imports from the partner.
    /// </summary>
    [PublicAPI]
    public sealed class PrfTariffRecord : TariffRecord, IEquatable<PrfTariffRecord>
    {
        /// <summary>
        /// Provides a key selector function for grouping by <see cref="ReporterIsoNumeric"/> and <see cref="PartnerIsoNumeric"/>.
        /// </summary>
        public override Func<TariffRecord, (string, string)> GroupByKeySelector { get; }

        /// <summary>
        /// The ISO 3166-1 numeric code for the importing country.
        /// </summary>
        [CanBeNull]
        public string ReporterIsoNumeric { get; }

        /// <summary>
        /// The ISO 3166-1 numeric code for the exporting country.
        /// </summary>
        [CanBeNull]
        public string PartnerIsoNumeric { get; }

        /// <summary>
        /// Constructs a <see cref="PrfTariffRecord"/> that represents the preferential tariff rate for a single product between the importing country and the exporting country.
        /// </summary>
        /// <param name="reporterIsoNumeric">
        /// The ISO 3166-1 numeric code for the importing country.
        /// </param>
        /// <param name="partnerIsoNumeric">
        /// The ISO 3166-1 numeric code for the exporting country.
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
        public PrfTariffRecord([CanBeNull] string reporterIsoNumeric, [CanBeNull] string partnerIsoNumeric, [CanBeNull] int? year, [CanBeNull] string product, [CanBeNull] double? tariff) : base(year, product, tariff)
        {
            ReporterIsoNumeric = reporterIsoNumeric;
            PartnerIsoNumeric = partnerIsoNumeric;
            GroupByKeySelector = x => x is PrfTariffRecord y ? (y.ReporterIsoNumeric, y.PartnerIsoNumeric) : throw new InvalidCastException(nameof(x));
        }

        /// <summary>
        /// Returns a string that represents this <see cref="PrfTariffRecord"/> = ReporterIsoNumeric|PartnerIsoNumeric|Year|Product|Tariff.
        /// </summary>
        /// <returns>
        /// A pipe-delimited string that represents this <see cref="PrfTariffRecord"/>.
        /// </returns>
        [Pure]
        public override string ToString()
        {
            return $"{ReporterIsoNumeric}|{PartnerIsoNumeric}|{base.ToString()}";
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>
        /// A hash code for this <see cref="PrfTariffRecord"/>.
        /// </returns>
        [Pure]
        public override int GetHashCode()
        {
            unchecked
            {
                return
                    base.GetHashCode() +
                    397 * (ReporterIsoNumeric?.GetHashCode() ?? 0) +
                    397 * (PartnerIsoNumeric?.GetHashCode() ?? 0);
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="PrfTariffRecord"/> is equal to this <see cref="PrfTariffRecord"/>.
        /// </summary>
        /// <param name="obj">
        /// The <see cref="PrfTariffRecord"/> to compare with this <see cref="PrfTariffRecord"/>.
        /// </param>
        /// <returns>
        /// True if the specified <see cref="PrfTariffRecord"/> objects are equal; otherwise, false.
        /// </returns>
        [Pure]
        public override bool Equals(object obj)
        {
            return Equals(obj as PrfTariffRecord);
        }

        /// <summary>
        /// Indicates whether this is equal to another <see cref="PrfTariffRecord"/>.
        /// </summary>
        /// <param name="other">
        /// A <see cref="PrfTariffRecord"/> to compare with this <see cref="PrfTariffRecord"/>.
        /// </param>
        /// <returns>
        /// True if the specified <see cref="PrfTariffRecord"/> objects are equal; otherwise, false.
        /// </returns>
        [Pure]
        public bool Equals(PrfTariffRecord other)
        {
            if (other is null)
            {
                return false;
            }

            return
                base.Equals(other) &&
                (ReporterIsoNumeric?.Equals(other.ReporterIsoNumeric, StringComparison.OrdinalIgnoreCase) ?? false) &&
                (PartnerIsoNumeric?.Equals(other.PartnerIsoNumeric, StringComparison.OrdinalIgnoreCase) ?? false);
        }
    }
}