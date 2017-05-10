using System;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AD.TariffSets
{
    [PublicAPI]
    public static class Program
    {
        public static void Main()
        {
            //const string directory = "C:\\Users\\adren\\Desktop\\Argentina V2";
            //const string directory = "C:\\Users\\austin.drenski\\Desktop\\Argentina V2";
            const string directory = "C:\\Work\\Austin\\April 18 - new work";

            Task task =
                TargetTariffYearFactory.Create(
                    $"{directory}\\Tariff data\\Downloads\\MFN_Applied_4_16_17.zip",
                    $"{directory}\\Tariff data\\Downloads\\PRF_Applied_4_16_17.zip",
                    $"{directory}\\regions.txt",
                    $"{directory}\\tariff data",
                    new (int minimum, int target)[]
                    {
                        (minimum: 1995, target: 2011)
                    });

            task.Wait();
            Console.WriteLine($"Finished with status: {task.Status}. Press enter to exit.");
            Console.ReadLine();

            //TargetTariffYearFactory.Create(
            //    $"{directory}\\Tariff data\\Downloads\\MFN_Applied_4_16_17.zip",
            //    $"{directory}\\Tariff data\\Downloads\\PRF_Applied_4_16_17.zip",
            //    $"{directory}\\regions.txt",
            //    $"{directory}\\tariff data",
            //    Enumerable.Range(2015, 1),
            //    x => new string[]
            //        {
            //            "Argentina",
            //            "Brazil",
            //            "Paraguay",
            //            "Uruguay",
            //            "Venezuela"
            //        }
            //        .Intersect(
            //            new string[]
            //            {
            //                x.ReporterRegion,
            //                x.PartnerRegion
            //            })
            //        .Any());

            //ProcessTrade.Process($"{directory}\\GTAP_Matrices", "$"{directory}\\Output\\Output0.txt");

            // These are the linkage matrices constructed directly from the concordance
            //ProcessTrade.Process($"{directory}\\Linkage_Matrices", $"{directory}\\Output\\Output0_b.txt");

            // These are the linkage matrices constructed by splitting on sector shares
            //ProcessTrade.Process($"{directory}\\Linkage_Matrices", $"{directory}\\Output\\Output1.txt");

            //ProcessTariffs.Process($"{directory}\\GTAP_Tariff_Matrices", "$"{directory}\\Output\\Output2.txt");
            //ProcessTariffs.Process($"{directory}\\Linkage_Tariff_Matrices", $"{directory}\\output_tariffs.txt");
        }
    }
}