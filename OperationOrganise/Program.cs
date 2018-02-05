using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Configuration;

namespace OperationOrganise
{
    class Program
    {
        static void Main(string[] args)
        {
            "START".ConsoleLogMessage();

            var organiser = new Organiser(
                ConfigurationManager.AppSettings["sourcePath"],
                int.Parse(ConfigurationManager.AppSettings["maxPerFolder"]));

            var section = (Hashtable)ConfigurationManager.GetSection("ImageTransforms");
            
            organiser.Process(
                Organiser.Mode.Images,
                ConfigurationManager.AppSettings["validImageExtensions"].Split(','),
                 section.Cast<DictionaryEntry>().ToDictionary(d => (string)d.Key, d => (string)d.Value));

            organiser.Process(
                Organiser.Mode.Videos,
                ConfigurationManager.AppSettings["validVideoExtensions"].Split(','));

            "FIN".ConsoleLogMessage();
        }
    }
}