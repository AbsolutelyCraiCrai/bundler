using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace Bundler
{
    internal class Bundler
    {
        public static void Main(String[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Invalid arguments.");
                return;
            }

            string dir = args[0];

            string[] files = Directory.EnumerateFiles(dir, "*.js", SearchOption.AllDirectories).ToArray();
            string bundled = "";
            foreach (string file in files)
            {
                bundled += File.ReadAllText(file);
            }

            bundled = Regex.Replace(bundled, "import .+? from \"\\..+?\";", "", RegexOptions.IgnoreCase);
            bundled = Regex.Replace(bundled, "export .+?;", "", RegexOptions.IgnoreCase);

            File.WriteAllText("input.js", bundled);

            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "esbuild.exe";
            p.StartInfo.Arguments = "input.js --outfile=out\\bundle.js --minify --sourcemap";
            p.StartInfo.UseShellExecute = false;
            p.Start();
            p.WaitForExit();

            if(args.Length == 2)
            {
                // Comment file
                string comment = File.ReadAllText(args[1]);
                File.WriteAllText("out\\bundle.js", comment + "\n" + File.ReadAllText("out\\bundle.js"));
            }
        }
    }
}
