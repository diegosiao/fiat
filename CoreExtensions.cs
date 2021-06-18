namespace Fiat
{
    public static class CoreExtensions
    {
        public static string ReplaceVars(this string text, Profile profile)
        {
            foreach (var variable in profile.BindedVars)
            {
                text = text.Replace($"${variable.Key}$", variable.Value);
            }

            return text;
        }

        public static string NormalizeSlashs(this string path)
        {
            return path.Replace("\\", "/");
        }
    }
}
