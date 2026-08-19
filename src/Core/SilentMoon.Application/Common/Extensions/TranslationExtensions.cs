using SilentMoon.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace SilentMoon.Application.Common.Extensions
{
    public static class TranslationExtensions
    {
        public static Dictionary<string, string> ToLanguageLookup(this IEnumerable<Translation> translations, string languageCode) =>
            translations
                .Where(x => x.LanguageCode == languageCode)
                .ToDictionary(x => x.Key, x => x.Value);

        public static string Localize(this Dictionary<string, string> lookup, string key, string fallback) =>
            lookup.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value)
                ? value
                : fallback;
    }

    public static class TranslationKeys
    {
        public static string For(string entityType, int id, string field) => $"{entityType}:{id}:{field}";
    }
}
