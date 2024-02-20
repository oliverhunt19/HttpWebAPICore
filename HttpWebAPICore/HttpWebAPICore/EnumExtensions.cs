using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;

namespace HttpWebAPICore
{
    public static class EnumExtension
    {
        /// <summary>
        /// Converts a <see cref="Enum"/> value to string.
        /// If enum is a <see cref="FlagsAttribute"/>, values are separated by the passed <paramref name="delimeter"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Enum"/> type.</typeparam>
        /// <param name="enum">The <see cref="Enum"/> to convert of <typeparamref name="T"/>.</param>
        /// <param name="delimeter">The separator inserted between each value if the <see cref="Enum"/>.</param>
        /// <returns>A <see cref="string"/> representation of the enum value.</returns>
        public static string ToEnumString<T>(this T @enum, char delimeter)
            where T : struct
        {
            return Convert.ToString(@enum, CultureInfo.InvariantCulture)?.ToLower().Replace(',', delimeter).Replace(" ", "");
        }

        /// <summary>
        /// Converts the enum into a string, by getting the <see cref="EnumMemberAttribute"/> value.
        /// </summary>
        /// <typeparam name="T">The <see cref="Enum"/> type.</typeparam>
        /// <param name="enum">The enum value.</param>
        /// <returns>A <see cref="string"/> representation of the enum value.</returns>
        public static string ToEnumMemberString<T>(this T @enum)
            where T : struct
        {
            return typeof(T)
                .GetTypeInfo()
                .DeclaredMembers
                .SingleOrDefault(x => x.Name == @enum.ToString())?
                .GetCustomAttribute<EnumMemberAttribute>(false)?
                .Value;
        }

        /// <summary>
        /// Will get the string value for a given enums value, this will
        /// only work if you assign the StringValue attribute to
        /// the items in your enum.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string? GetStringValue(this Enum value)
        {
            // Get the type
            Type type = value.GetType();

            // Get fieldinfo for this type
            FieldInfo? fieldInfo = type.GetField(value.ToString());

            // Get the stringvalue attributes
            EnumMemberAttribute[]? attribs = fieldInfo?.GetCustomAttributes<EnumMemberAttribute>(false).ToArray();

            // Return the first if there was a match.
            return attribs?.Length > 0 ? attribs[0].Value : null;
        }
    }
}
