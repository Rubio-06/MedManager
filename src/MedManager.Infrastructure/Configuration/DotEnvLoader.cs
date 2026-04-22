namespace MedManager.Infrastructure.Configuration
{
    public static class DotEnvLoader
    {
        public static void Load(string fileName = ".env")
        {
            var envPath = FindUpward(fileName);
            if (envPath is null)
            {
                return;
            }

            foreach (var rawLine in File.ReadAllLines(envPath))
            {
                var line = rawLine.Trim();
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#", StringComparison.Ordinal))
                {
                    continue;
                }

                var separatorIndex = line.IndexOf('=');
                if (separatorIndex <= 0)
                {
                    continue;
                }

                var key = line[..separatorIndex].Trim();
                if (string.IsNullOrWhiteSpace(key))
                {
                    continue;
                }

                var value = line[(separatorIndex + 1)..].Trim();
                if (value.StartsWith('"') && value.EndsWith('"') && value.Length >= 2)
                {
                    value = value[1..^1];
                }

                if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(key)))
                {
                    Environment.SetEnvironmentVariable(key, value);
                }
            }
        }

        private static string? FindUpward(string fileName)
        {
            var current = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (current is not null)
            {
                var candidate = Path.Combine(current.FullName, fileName);
                if (File.Exists(candidate))
                {
                    return candidate;
                }

                current = current.Parent;
            }

            return null;
        }
    }
}