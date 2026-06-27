using ZenCare.Model.Enums;

namespace ZenCare.Model.SearchObjects
{
    public class RoleSearchObject : BaseSearchObject
    {
        public string? Name { get; set; }

        public UserRoleType? RoleType { get; set; }

        public bool? IsActive { get; set; }
    }
}
