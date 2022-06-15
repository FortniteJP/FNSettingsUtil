using FNSettingsUtil;

namespace Program
{
    public static class Program
    {
        public static async Task Main()
        {
            var settings = await FNSettingsUtil.FNSettingsUtil.DeserializeClientSettingsAsync("Data/ClientSettings.Sav");

            var r = "";
            foreach (var guid in settings.Guids)
            {
                r += $"{guid.ToString()}\n";
            }
            foreach (var prop in settings.Properties)
            {
                var x = $"[{(prop.Value.HasPropertyGuid ? prop.Value.Guid : "None")}]{prop.Key}: {prop.Value.Value}\n";
                Console.Write(x);
                r += x;
            }
            File.WriteAllText("Data/Parsed.txt", r);

            Console.ReadLine();

            var file = File.OpenWrite("Data/Compressed.Sav");
            var compressed = await FNSettingsUtil.FNSettingsUtil.SerializeClientSettingsAsync(settings);
            Console.WriteLine(compressed.Length);
            compressed.Position = 0;
            await compressed.CopyToAsync(file);
            file.Close();
        }
    }
}