using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Resources;
using System.Text.RegularExpressions;

namespace ResXResourceReaderConsole
{
    class Program
    {
        private static string Expression = @"msgid ""(.*)""\w*\s*msgstr ""(.*)"""; 



        static void Main(string[] args)
        {
            //ProcessResxToCamelcasekey()
            ProcessPoToREsxFile();
            ProcessPotToREsxFile();
        }

        private static void ProcessPotToREsxFile()
        {
            foreach (var potFile in GetPotFiles())
            {
                PotToResx(potFile);
            }
        }

        private static void ProcessPoToREsxFile()
        {
            foreach (var poFile in GetPoFiles())
            {
                PoToResx(poFile);
            }
        }

        private static void ProcessResxToCamelcasekey()
        {
            foreach (var file in GetResxFiles())
            {


                // Create a ResXResourceReader for the file items.resx.
                ResXResourceReader rsxr = new ResXResourceReader(file);
                ResXResourceWriter resourceWriter = new ResXResourceWriter(file);



                // Iterate through the resources and display the contents to the console.
                foreach (DictionaryEntry d in rsxr)
                {
                    Console.WriteLine(d.Key.ToString() + ":\t" + d.Value.ToString());
                    resourceWriter.AddResource(ToCamelCase(d.Key.ToString()), d.Value.ToString());
                }

                //Close the reader.
                rsxr.Close();
                resourceWriter.Generate();
                resourceWriter.Close();
            }
        }
        private static void PotToResx(string poFilePath)
        {
            var info = new System.IO.FileInfo(poFilePath);
            var derectoriinfo = new System.IO.DirectoryInfo(poFilePath);
            var str = System.IO.File.ReadAllText(poFilePath);
            var regex = new Regex(Expression);
            // Find matches.
            var matches = regex.Matches(str);
            // Report on each match.
            var resourceWriter = new ResXResourceWriter(poFilePath.Replace(".pot", $".resx"));

            var dico = new Dictionary<string, string>();

            foreach (Match match in matches)
            {
                var groups = match.Groups;
                var key = ToCamelCase(groups[1].Value);
                if (!string.IsNullOrWhiteSpace(key) && !dico.ContainsKey(key))
                    dico.Add(key, groups[1].Value);
            }

            foreach (var lst in dico)
            {
                resourceWriter.AddResource(lst.Key, lst.Value);
            }

            resourceWriter.Generate();
            resourceWriter.Close();
        }

        private static void PoToResx(string poFilePath)
        {
            var info = new System.IO.FileInfo(poFilePath);
           var derectoriinfo = new System.IO.DirectoryInfo(poFilePath);
            var str = System.IO.File.ReadAllText(poFilePath);
            var regex = new Regex(Expression);
            // Find matches.
            var matches = regex.Matches(str);
            // Report on each match.
            var resourceWriter = new ResXResourceWriter(poFilePath.Replace(".po",$".{derectoriinfo.Parent.Name}.resx"));

           var dico = new Dictionary<string,string>();

            foreach (Match match in matches)
            {
                var groups = match.Groups;
                var key = ToCamelCase(groups[1].Value);
                if (!string.IsNullOrWhiteSpace(key) && !dico.ContainsKey(key))
                    dico.Add(key, groups[2].Value);
            }

            foreach (var lst in dico)
            {
                resourceWriter.AddResource(lst.Key,lst.Value);
            }

            resourceWriter.Generate();
            resourceWriter.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from">openmediavault.fr_FR.resx</param>
        /// <param name="to">openmediavault.resx</param>
        private static void KeyToValue(string from, string to)
        {
            // Create a ResXResourceReader for the file items.resx.
            ResXResourceReader rsxr = new ResXResourceReader(from);
            ResXResourceWriter resourceWriter = new ResXResourceWriter(to);

            // Iterate through the resources and display the contents to the console.
            foreach (DictionaryEntry d in rsxr)
            {
                Console.WriteLine(d.Key.ToString() + ":\t" + d.Value.ToString());
                resourceWriter.AddResource(d.Key.ToString(), d.Key.ToString());
            }

            //Close the reader.
            rsxr.Close();
            resourceWriter.Generate();
            resourceWriter.Close();
        }


        private static IEnumerable<string> GetPoFiles()
        {
        string path = Directory.GetCurrentDirectory();
        string[] filePaths = Directory.GetFiles(path, "*.po",SearchOption.AllDirectories);
        return filePaths;
        }


        private static IEnumerable<string> GetPotFiles()
        {
            string path = Directory.GetCurrentDirectory();
            string[] filePaths = Directory.GetFiles(path, "*.pot", SearchOption.AllDirectories);
            return filePaths;
        }

        private static IEnumerable<string> GetResxFiles()
        {
            yield return "openmediavault.resx";
            yield return "openmediavault.de_DE.resx";
            yield return "openmediavault.es_ES.resx";
            yield return "openmediavault.fr_FR.resx";
        }

        private static string ToCamelCase(string str)
        {
            var titlecase = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str);
            var camelcase = titlecase.Replace("_", string.Empty).Replace(" ", string.Empty);
            return camelcase;
        }
    }
    
}
