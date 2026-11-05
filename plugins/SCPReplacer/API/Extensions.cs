using System.Text.RegularExpressions;
using PlayerRoles;

namespace SCPReplacer.API
{
    public static class Extensions
    {
        private static readonly Regex NonDigits = new Regex(@"[^0-9]", RegexOptions.Compiled);

        private static readonly Regex ScpArgument = new Regex(@"^[A-Za-z\-]{0,4}[0-9]{1,4}(-[0-9])?$", RegexOptions.Compiled);

        public static string ScpNumber(this RoleTypeId role) => NonDigits.Replace(role.ToString(), string.Empty);

        public static string ScpNumber(this string value) => NonDigits.Replace(value, string.Empty);

        public static bool IsValidScpArgument(this string value)
        {
            return !string.IsNullOrEmpty(value) && value.Length <= 12 && ScpArgument.IsMatch(value);
        }
    }
}
