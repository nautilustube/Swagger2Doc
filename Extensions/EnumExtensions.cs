using System.ComponentModel;

namespace Swagger2Doc.Extensions
{
    public static class EnumExtensions
    {
        public static string? GetEnumDescription(this Enum e)
        {
            var descriptionAttribute = e.GetType().GetMember(e.ToString())[0]
                .GetCustomAttributes(typeof(DescriptionAttribute), inherit: false)[0]
                as DescriptionAttribute;

            return descriptionAttribute?.Description;
        }
    }
}
