using System;
using System.Collections.Generic;
using System.Linq;

namespace BookStore.Client.Helpers
{
    public static class UrlQueryHelper
    {
        public static Dictionary<string, string> Parse(string uri)
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (!uri.Contains("?"))
                return dict;

            var q = uri.Split("?", 2)[1];

            foreach (var p in q.Split("&", StringSplitOptions.RemoveEmptyEntries))
            {
                var parts = p.Split("=", 2);
                if (parts.Length == 2)
                    dict[parts[0]] = Uri.UnescapeDataString(parts[1]);
            }
            return dict;
        }

        public static string? Get(this Dictionary<string, string> d, string key)
            => d.TryGetValue(key, out var v) ? v : null;

        public static int? GetInt(this Dictionary<string, string> d, string key)
            => d.TryGetValue(key, out var v) && int.TryParse(v, out int n) ? n : null;

        public static decimal? GetDecimal(this Dictionary<string, string> d, string key)
            => d.TryGetValue(key, out var v) && decimal.TryParse(v, out decimal n) ? n : null;

        public static string ToQueryString(this Dictionary<string, string> d)
            => string.Join("&", d.Select(x => $"{x.Key}={x.Value}"));
    }
}