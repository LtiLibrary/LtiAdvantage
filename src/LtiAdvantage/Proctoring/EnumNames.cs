using System;

namespace LtiAdvantage.Proctoring
{
    /// <summary>
    /// Strict parsing of enum names from the wire.
    /// </summary>
    internal static class EnumNames
    {
        /// <summary>
        /// Parses <paramref name="value"/> only if it is exactly one defined member name (any case).
        /// </summary>
        /// <remarks>
        /// <see cref="Enum.TryParse{TEnum}(string, bool, out TEnum)"/> alone is not safe for values
        /// that come from another system: it also accepts numbers ("3") and comma-separated lists
        /// ("pause,resume", which it ORs together), so a string that names no action could still
        /// come out as a real one such as Terminate, or as a value the enum does not define.
        /// </remarks>
        public static bool TryParse<TEnum>(string value, out TEnum result) where TEnum : struct, Enum
        {
            result = default;

            if (string.IsNullOrEmpty(value) || value.IndexOf(',') >= 0) return false;
            if (value.Trim() != value) return false;
            if (char.IsDigit(value[0]) || value[0] == '-' || value[0] == '+') return false;

            return Enum.TryParse(value, true, out result) && Enum.IsDefined(typeof(TEnum), result);
        }
    }
}
