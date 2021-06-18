using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace fiat
{
    public class Scaffold
    {
        public string Name { get; set; }

        [JsonProperty("file")]
        public string ScaffoldFile { get; set; }

        public string Stub { get; set; }

        public string StubContent { get; private set; }

        public Dictionary<string, string> Put { get; set; }

        internal Result Prepare(Profile profile)
        {
            // Stub (optional): If informed, stub file must exist
            if(!string.IsNullOrEmpty(Stub))
            {
                if(!File.Exists(Path.Combine(profile.StubsDir, Stub)))
                    return Result.Failure($"     > Stub file not found: {Path.Combine(profile.StubsDir, Stub)}");

                Stub = Path.Combine(profile.StubsDir, Stub);
            }

            // ScaffoldFile (required): Must always be informed, but does not have to exist (for 'stub' or 'put')
            if(string.IsNullOrEmpty(ScaffoldFile))
            {
                return Result.Failure("     > The scaffold file name template must be informed.");
            }

            // Apply variables values to ScaffoldFile name template
            ScaffoldFile = ScaffoldFile.ReplaceVars(profile);
            
            // If stub not informed, ScaffoldFile must exist for this profile
            ScaffoldFile = Path.Combine(Program.Config.BaseDir, ScaffoldFile);

            if (string.IsNullOrEmpty(Stub))
            {
                if(!File.Exists(ScaffoldFile))
                    return Result
                        .Failure($"     > You need to provide a stub if the file does not already exist. File not found: {ScaffoldFile}");

            }

            StubContent = File.Exists(ScaffoldFile) ? 
                File.ReadAllText(ScaffoldFile, Program.Config.DefaultEncoding) :
                File.ReadAllText(Stub, Program.Config.DefaultEncoding);
            
            // After all checking, time to prepare the StubContent (already loaded from Stub or ScaffoldFile)
            StubContent = StubContent.ReplaceVars(profile);
            
            // Put (optional): Should find a insertion point at the specified ScaffoldFile
            ScanInsertionPoints(profile);

            return Result.Success();
        }

        internal Result Create()
        {
            var directory = Path.GetDirectoryName(ScaffoldFile);

            if (!Directory.Exists(directory))
            {
                Console.WriteLine($"     > Creating directory {directory}");
                Directory.CreateDirectory(directory);
            }

            // StubContent was safely loaded and prepared from Stub or ScaffoldFile at this point
            File.WriteAllText(ScaffoldFile, StubContent, Program.Config.DefaultEncoding);

            return Result.Success();
        }

        private void ScanInsertionPoints(Profile profile)
        {
            if (Put?.Any() ?? false)
            {
                foreach (var item in Put)
                {
                    if (string.IsNullOrEmpty(item.Key) || string.IsNullOrEmpty(item.Value))
                        continue;

                    var putStubFile = Path.Combine(profile.StubsDir, item.Value);

                    if(!File.Exists(putStubFile))
                    {
                        FiatConsole.WriteWarn(
                            $"     > Put '{item.Key}' stub file not found: {putStubFile}");
                        continue;
                    }

                    Console.WriteLine($"     > Adding '{item.Key}'");

                    var putContent = File.ReadAllText(putStubFile, Program.Config.DefaultEncoding);
                    putContent = putContent.ReplaceVars(profile);

                    if (StubContent.IndexOf($"fiat:{item.Key}") >= 0)
                    {
                        StubContent = StubContent.Replace($"fiat:{item.Key}", $"fiat:{item.Key}{putContent}");
                    }
                    else
                        FiatConsole.WriteWarn($"     > The insertion point 'fiat:{item.Key}' was not found. ");
                }
            }
        }
    }
}
