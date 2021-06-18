using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fiat
{
    public class Profile
    {
        public string Name { get; set; }

        public string BaseDir { get; set; }

        public string StubsDir { get; set; }

        public string[] Vars { get; set; }

        public Dictionary<string, string> BindedVars { get; private set; }

        public Scaffold[] Scaffolds { get; set; }

        public Profile()
        {
            BindedVars = new Dictionary<string, string>();
        }

        public Result IsValid()
        {
            try
            {
                Console.WriteLine($"Checking{$" {Name} " ?? " "}profile validity...\r\n");
                Vars ??= new string[0];

                if (Vars.Length != Program.Config.Args.Length - 1)
                    return Result.Failure(
                        $"Variable binding failed. This profile expects positional values as arguments for: " +
                        $"{string.Join(" ", Vars.Select(x => $"[{x}]"))};\r\nYou can always provide \"\" to represent empty variables. ");

                for (int i = 1; i <= Vars.Length; i++)
                {
                    // If there are 'args' a valid profile name must be the first
                    // The others must load the values for variables declared in profile.vars string array
                    BindedVars.Add(Vars[i - 1], Program.Config.Args[i]);
                }

                if (!Scaffolds?.Any() ?? true)
                {
                    return Result.Failure($"Any scaffolds were found, at least one must be informed. ");
                }

                if (string.IsNullOrEmpty(BaseDir))
                    BaseDir = Program.Config.BaseDir;

                // TODO Should we create de base directory folder structure at this point?

                if (string.IsNullOrEmpty(StubsDir))
                    StubsDir = Path.Combine(Program.Config.BaseDir, "stubs");
                else if (!Path.IsPathFullyQualified(StubsDir))
                    StubsDir = Path.Combine(Program.Config.BaseDir, StubsDir);

                if (!Directory.Exists(StubsDir) || !Directory.GetFiles(StubsDir).Any())
                    return Result.Failure($"The directory specified for stubs is empty: '{StubsDir.NormalizeSlashs()}'");

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }
    }
}
