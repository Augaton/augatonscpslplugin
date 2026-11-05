using PlayerRoles;

namespace AugatonLib.Text
{
    public static class RoleNames
    {
        public static string Full(RoleTypeId role)
        {
            return role.TryGetRoleTemplate(out PlayerRoleBase template) ? template.RoleName : role.ToString();
        }
    }
}
