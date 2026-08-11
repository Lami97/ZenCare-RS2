namespace ZenCare.Model.SearchObjects
{
    public class UserRoleSearchObject : PagedSearchObject
    {
        public int? UserId { get; set; }

        public int? RoleId { get; set; }
    }
}
