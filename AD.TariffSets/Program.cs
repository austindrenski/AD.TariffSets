using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AD.TariffSets
{
    [PublicAPI]
    public static class Program
    {
        public static void Main()
        {
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
        }
    }
}