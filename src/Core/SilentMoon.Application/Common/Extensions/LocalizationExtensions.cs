using Microsoft.Extensions.Localization;
using SilentMoon.Domain.Enums;
using SilentMoon.SharedKernel.Resources;

namespace SilentMoon.Application.Common.Extensions
{
    public static class LocalizationExtensions
    {
        public static string LocalizeCategory(this IStringLocalizer<Messages> localizer, ContentCategory category) =>
            localizer[$"ContentCategory.{category}"];
    }
}
