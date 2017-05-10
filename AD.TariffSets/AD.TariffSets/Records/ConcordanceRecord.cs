using JetBrains.Annotations;

namespace AD.TariffSets.Records
{
    [PublicAPI]
    public class ConcordanceRecord
    {
        public string Numeric3 { get; set; }

        public string Alpha3 { get; set; }

        public string Region { get; set; }

        public override string ToString()
        {
            return $"{Numeric3}|{Alpha3}|{Region}";
        }
    }
}