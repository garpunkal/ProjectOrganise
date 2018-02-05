using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace OperationOrganise
{
    public static class Extensions
    {
        public static void ConsoleLogMessage(this string s)
        {
            Console.WriteLine();
            Console.WriteLine("===============================================");
            Console.WriteLine(s);
            Console.WriteLine("===============================================");
            Console.WriteLine();
        }
        public static string GetNewFileName(this string newName)
        {
            var iteration = 1;
            while (File.Exists(newName))
            {
                newName =
                    $"{Path.GetDirectoryName(newName)}\\{Path.GetFileNameWithoutExtension(newName)}_{iteration}{Path.GetExtension(newName)}";
                iteration++;
            }

            return newName;
        }

    }
}
