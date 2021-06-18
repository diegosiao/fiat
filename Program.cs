using System;
using System.Linq;

namespace Fiat
{
    internal class Program
    {
        internal const string FiatConfigFileName = "fiat.config.json";

        internal static FiatConfig Config { get; private set; }

        static void Main(string[] args)
        {
            if (args.Length == 1 && "init".Equals(args[0], StringComparison.OrdinalIgnoreCase))
            {
                FiatConfig.Init();
                return;
            }

            var configResult = FiatConfig.CreateInstance(args);

            if (configResult.Failed)
            {
                FiatConsole.WriteError(configResult.Error, true);
                return;
            }

            Config = configResult.Value;

            if (string.IsNullOrEmpty(Config.BaseDir))
                Config.BaseDir = AppDomain.CurrentDomain.BaseDirectory;

            Console.WriteLine("");
            Console.WriteLine($"[i] Base directory: {Config.BaseDir}");
            Console.WriteLine("");

            var profile = Config.Profiles?
                .FirstOrDefault(x => args.Length == 0 || x.Name.Equals(args[0], StringComparison.OrdinalIgnoreCase));

            if (profile == null)
            {
                FiatConsole.WriteError($"The arguments must be empty or the first argument must be the name of a valid profile in '{FiatConfigFileName.NormalizeSlashs()}'. ");

                FiatConsole.WriteError("    Valid profiles:");
                foreach (var item in Config.Profiles)
                    FiatConsole.WriteError($"    > {item.Name}");

                FiatConsole.WriteError("", true);
                return;
            }

            var profileValidityCheck = profile.IsValid();

            if (profileValidityCheck.Failed)
            {
                FiatConsole.WriteError(profileValidityCheck.Error, true);
                return;
            }

            for (int i = 0; i < profile.Scaffolds.Length; i++)
            {
                var scaffold = profile.Scaffolds[i];
                
                Console.WriteLine(
                    string.IsNullOrEmpty(scaffold.Name) ? 
                    $"Scaffolding item {i + 1}..." : $"Scaffolding {scaffold.Name}...");

                var scaffoldPreparation = scaffold.Prepare(profile);

                if(scaffoldPreparation.Failed)
                {
                    FiatConsole.WriteError(scaffoldPreparation.Error);
                    continue;
                }

                scaffold.Create();
            }

            FiatConsole.WriteSummary();
        }
    }
}
