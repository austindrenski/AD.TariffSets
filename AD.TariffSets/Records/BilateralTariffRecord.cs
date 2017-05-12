using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace AD.TariffSets.Records
{
    /// <summary>
    /// Represents a bilateral tariff imposed by the reporter on imports from the partner.
    /// </summary>
    [PublicAPI]
    public sealed class BilateralTariffRecord : TariffRecord, IEquatable<BilateralTariffRecord>
    {
        /// <summary>
        /// Compares the equality of two <see cref="BilateralTariffRecord"/> objects by <see cref="ReporterIso3"/>, <see cref="PartnerIso3"/>, and <see cref="TariffRecord.Product"/>.
        /// </summary>
        [NotNull]
        public static IEqualityComparer<BilateralTariffRecord> SetEqualityComparer { get; } = new InternalSetEqualityComparer();

        /// <summary>
        /// Provides a key selector function for grouping by <see cref="ReporterIso3"/> and <see cref="PartnerIso3"/>.
        /// </summary>
        public override Func<TariffRecord, (string, string)> GroupByKeySelector { get; }

        /// <summary>
        /// The ISO 3166-1 alpha-3 code for the importing country.
        /// </summary>
        [CanBeNull]
        public string ReporterIso3 { get; }
        
        /// <summary>
        /// The ISO 3166-1 alpha-3 code for the exporting country.
        /// </summary>
        [CanBeNull]
        public string PartnerIso3 { get; }

        /// <summary>
        /// The user-defined region for the importing country.
        /// </summary>
        [CanBeNull]
        public string ReporterRegion { get; }

        /// <summary>
        /// The user-defined region for the exporting country.
        /// </summary>
        [CanBeNull]
        public string PartnerRegion { get; }

        /// <summary>
        /// The type of the tariff.
        /// </summary>
        [CanBeNull]
        public string Type { get; }

        /// <summary>
        /// Constructs a <see cref="BilateralTariffRecord"/> that represents the tariff rate for a single product between the importing country and the exporting country.
        /// </summary>
        /// <param name="reporterIso3">
        /// The ISO 3166-1 alpha-3 code for the importing country.
        /// </param>
        /// <param name="partnerIso3">
        /// The ISO 3166-1 alpha-3 code for the exporting country.
        /// </param>
        /// <param name="reporterRegion">
        /// The user-defined region for the importing country.
        /// </param>
        /// <param name="partnerRegion">
        /// The user-defined region for the exporting country.
        /// </param>
        /// <param name="type">
        /// The type of the tariff.
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
        public BilateralTariffRecord([CanBeNull] string reporterIso3, [CanBeNull] string partnerIso3, [CanBeNull] string reporterRegion, [CanBeNull] string partnerRegion, [CanBeNull] string type, [CanBeNull] int? year, [CanBeNull] string product, [CanBeNull] double? tariff) : base(year, product, tariff)
        {
            ReporterIso3 = reporterIso3;
            PartnerIso3 = partnerIso3;
            ReporterRegion = reporterRegion;
            PartnerRegion = partnerRegion;
            Type = type;
            GroupByKeySelector = x => x is BilateralTariffRecord y ? (y.ReporterIso3, y.PartnerIso3) : throw new InvalidCastException(nameof(x));
        }

        /// <summary>
        /// Returns a string that represents this <see cref="BilateralTariffRecord"/> = ReporterIso3|PartnerIso3|ReporterRegion|PartnerRegion|Type|Year|Product|Tariff.
        /// </summary>
        /// <returns>
        /// A pipe-delimited string that represents this <see cref="BilateralTariffRecord"/>.
        /// </returns>
        [Pure]
        public override string ToString()
        {
            return $"{ReporterIso3}|{PartnerIso3}|{ReporterRegion}|{PartnerRegion}|{Type}|{base.ToString()}";
        }
        
        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>
        /// A hash code for this <see cref="BilateralTariffRecord"/>.
        /// </returns>
        [Pure]
        public override int GetHashCode()
        {
            unchecked
            {
                return
                    base.GetHashCode() +
                    397 * (ReporterIso3?.GetHashCode() ?? 0) +
                    397 * (PartnerIso3?.GetHashCode() ?? 0) +
                    397 * (ReporterRegion?.GetHashCode() ?? 0) +
                    397 * (PartnerRegion?.GetHashCode() ?? 0) +
                    397 * (Type?.GetHashCode() ?? 0);
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="BilateralTariffRecord"/> is equal to this <see cref="BilateralTariffRecord"/>.
        /// </summary>
        /// <param name="obj">
        /// The <see cref="BilateralTariffRecord"/> to compare with this <see cref="BilateralTariffRecord"/>.
        /// </param>
        /// <returns>
        /// True if the specified <see cref="BilateralTariffRecord"/> objects are equal; otherwise, false.
        /// </returns>
        [Pure]
        public override bool Equals(object obj)
        {
            return Equals(obj as BilateralTariffRecord);
        }

        /// <summary>
        /// Indicates whether this is equal to another <see cref="BilateralTariffRecord"/>.
        /// </summary>
        /// <param name="other">
        /// A <see cref="BilateralTariffRecord"/> to compare with this <see cref="BilateralTariffRecord"/>.
        /// </param>
        /// <returns>
        /// True if the specified <see cref="BilateralTariffRecord"/> objects are equal; otherwise, false.
        /// </returns>
        [Pure]
        public bool Equals(BilateralTariffRecord other)
        {
            if (other is null)
            {
                return false;
            }

            return
                base.Equals(other) &&
                (ReporterIso3?.Equals(other.ReporterIso3, StringComparison.OrdinalIgnoreCase) ?? false) &&
                (PartnerIso3?.Equals(other.PartnerIso3, StringComparison.OrdinalIgnoreCase) ?? false) &&
                (ReporterRegion?.Equals(other.ReporterRegion, StringComparison.OrdinalIgnoreCase) ?? false) &&
                (PartnerRegion?.Equals(other.PartnerRegion, StringComparison.OrdinalIgnoreCase) ?? false) &&
                (Type?.Equals(other.Type, StringComparison.OrdinalIgnoreCase) ?? false);
        }

        /// <summary>
        /// Compares <see cref="BilateralTariffRecord"/> objects by <see cref="ReporterIso3"/>, <see cref="PartnerIso3"/>, and <see cref="TariffRecord.Product"/>.
        /// </summary>
        private sealed class InternalSetEqualityComparer : IEqualityComparer<BilateralTariffRecord>
        {
            /// <summary>
            /// Determines whether the specified <see cref="BilateralTariffRecord"/> objects are equal.</summary>
            /// <param name="x">
            /// The first <see cref="BilateralTariffRecord"/> to compare.
            /// </param>
            /// <param name="y">
            /// The second <see cref="BilateralTariffRecord"/> to compare.
            /// </param>
            /// <returns>
            /// True if the specified <see cref="BilateralTariffRecord"/> objects are equal; otherwise, false.
            /// </returns>
            [Pure]
            public bool Equals(BilateralTariffRecord x, BilateralTariffRecord y)
            {
                if (x is null & y is null)
                {
                    return true;
                }
                if (x is null)
                {
                    return false;
                }
                if (y is null)
                {
                    return false;
                }

                return
                    (x.ReporterIso3?.Equals(y.ReporterIso3, StringComparison.OrdinalIgnoreCase) ?? false) &&
                    (x.PartnerIso3?.Equals(y.PartnerIso3, StringComparison.OrdinalIgnoreCase) ?? false) &&
                    (x.Product?.Equals(y.Product, StringComparison.OrdinalIgnoreCase) ?? false);
            }

            /// <summary>
            /// Returns a hash code for the specified object.
            /// </summary>
            /// <returns>
            /// A hash code for the specified object.
            /// </returns>
            /// <param name="obj">
            /// The <see cref="Object"/> for which a hash code is to be returned.
            /// </param>
            /// <exception cref="ArgumentNullException">
            /// The type of <paramref name="obj"/> is a reference type and <paramref name="obj" /> is null.
            /// </exception>
            [Pure]
            public int GetHashCode(BilateralTariffRecord obj)
            {
                if (obj is null)
                {
                    throw new ArgumentNullException(nameof(obj));
                }

                unchecked
                {
                    return
                        397 * (obj.ReporterIso3?.GetHashCode() ?? 0) +
                        397 * (obj.PartnerIso3?.GetHashCode() ?? 0) +
                        397 * (obj.Product?.GetHashCode() ?? 0);
                }
            }
        }
    }
}