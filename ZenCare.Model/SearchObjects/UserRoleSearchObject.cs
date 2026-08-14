namespace ZenCare.Model.SearchObjects
{
    public class UserRoleSearchObject : PagedSearchObject
    {
        public string? Username { get; set; }

        public int? UserId { get; set; }

        public int? RoleId { get; set; }
    }
}
