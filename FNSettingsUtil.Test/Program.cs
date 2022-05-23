using FNSettingsUtil;

namespace Program
{
    public static class Program
    {
        public static async Task Main()
        {
            var settings = await FNSettingsUtil.FNSettingsUtil.GetClientSettingsAsync("Data/ClientSettings.Sav");

            var r = "";
            foreach (var prop in settings.Properties)
            {
                var x = $"{prop.Key}: {prop.Value.Value}\n";
                Console.WriteLine(x);
                r += x;
            }
            File.WriteAllText("Data/Parsed.txt", r);
        }
    }
}