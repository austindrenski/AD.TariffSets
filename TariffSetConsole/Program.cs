﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AD.TariffSets;

namespace TariffSetConsole
{
    /// <summary>
    /// 
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// 
        /// </summary>
        public static void Main()
        {

            //const string directory = "\\Work\\Austin\\April 18 - new work";
            const string directory = "\\Users\\adren\\Desktop\\Argentina_V2";

            Directory.EnumerateFileSystemEntries(Directory.GetCurrentDirectory()).ToList().ForEach(Console.WriteLine);
            Directory.EnumerateFileSystemEntries(directory).ToList().ForEach(Console.WriteLine);

            Task task =
                TargetTariffYearFactory.Create(
                    $"{directory}\\Tariff data\\Downloads\\MFN_Applied_4_16_17.zip",
                    $"{directory}\\Tariff data\\Downloads\\PRF_Applied_4_16_17.zip",
                    $"{directory}\\regions.txt",
                    $"{directory}\\tariff data",
                    new(int minimum, int target)[]
                    {
                        (minimum: 1995, target: 2011)
                    });

            task.Wait();
            Console.WriteLine($"Finished with status: {task.Status}. Press enter to exit.");
            Console.ReadLine();
        }
    }
}