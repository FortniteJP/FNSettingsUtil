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

            var file = File.OpenWrite("Data/Compressed.Sav");
            var compressed = await FNSettingsUtil.FNSettingsUtil.SerializeClientSettingsAsync(settings);
            Console.WriteLine(compressed.Length);
            compressed.Position = 0;
            byte[] buf = new byte[4096];

            while (true)
            {
                int read = await compressed.ReadAsync(buf, 0, buf.Length);
                if (read == 0) break;
                await file.WriteAsync(buf, 0, read);
            }
        }
    }
}