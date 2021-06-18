using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace fiat
{
    public class FiatConfig
    {
        // TODO Default should be UTF-8
        [JsonProperty("encoding")]
        public string TextEncoding { get; set; }

        [JsonIgnore]
        internal Encoding DefaultEncoding { get; private set; }

        public string BaseDir { get; set; }

        public string[] Args { get; private set; }

        public IEnumerable<Profile> Profiles { get; set; }

        private FiatConfig(string json)
        {
            var jsonObject = JsonConvert
                .DeserializeAnonymousType(json, new
                {
                    BaseDir = string.Empty,
                    Profiles = new List<Profile>()
                });

            BaseDir = jsonObject.BaseDir;
            Profiles = jsonObject.Profiles;
        }

        public static Result<FiatConfig> CreateInstance(string[] args)
        {
            try
            {
                if (!File.Exists(Program.FiatConfigFileName))
                    return Result<FiatConfig>.Failure($"The file '{Program.FiatConfigFileName.NormalizeSlashs()}' was not found. ");

                var json = File.ReadAllText("fiat.config.json");

                var fiatConfig = new FiatConfig(json)
                {
                    Args = args
                };

                if (!fiatConfig.Profiles?.Any() ?? true)
                    return Result<FiatConfig>
                        .Failure($"Any valid profile was found in '{Program.FiatConfigFileName}' file. ");

                fiatConfig.DefaultEncoding = Encoding.UTF8;
                try
                {
                    if(!string.IsNullOrEmpty(fiatConfig.TextEncoding))
                        fiatConfig.DefaultEncoding = Encoding.GetEncoding(fiatConfig.TextEncoding);
                } 
                catch
                {
                    FiatConsole.WriteWarn($"The fallback UTF-8 enconding is being used. It was not possible create the encoding with the given name: {fiatConfig.TextEncoding}");
                }

                return Result<FiatConfig>.Success(fiatConfig);
            }
            catch (Exception ex)
            {
                return Result<FiatConfig>.Failure(ex.Message);
            }
        }

        internal static void Init()
        {
            var fiatConfigFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fiat.config.json");

            File.WriteAllText(fiatConfigFile, FiatConfigTemplate, Encoding.UTF8);
            Console.WriteLine($"Fiat Configuration File created at {fiatConfigFile}");
        }

        const string FiatConfigTemplate = @"{
  ""baseDir"": ""C:/my-project-dir"",
  ""profiles"": [
    {
      ""name"": ""new-command"",
      ""vars"": [ ""entity"", ""action"" ],
	  ""stubs"": ""stubs"",
      ""scaffolds"": [
        {
          ""name"": ""validator"",
          ""file"": ""my-project-A/$entity$/Commands/Validators/$entity$$action$Validator.cs"",
          ""stub"": ""Validator.stub""
        },
        {
          ""name"": ""dto"",
          ""file"": ""my-project-A/$entity$/Commands/Dtos/$entity$$action$Dto.cs"",
          ""stub"": ""Dto.stub""
        },
        {
          ""name"": ""command"",
          ""file"": ""my-project-B/$entity$/Commands/$entity$$action$Command.cs"",
          ""stub"": ""Command.stub""
        },
        {
          ""name"": ""repository-interface"",
          ""file"": ""my-project-C/$entity$/I$entity$CommandRepository.cs"",
          ""stub"": ""CommandRepositoryInterface.stub"",
          ""put"": {
            ""new-method"": ""CommandRepositoryInterfaceNewMethod.stub""
          }
        },
        {
          ""name"": ""repository"",
          ""file"": ""my-project-D/$entity$/$entity$CommandRepository.cs"",
          ""stub"": ""CommandRepository.stub"",
          ""put"": {
          ""new-method"": ""CommandRepositoryNewMethod.stub"",
            ""new-comment"": ""CommandRepositoryNewComment.stub""
          }
        }
      ]
    }
  ]
}";
    }
}
