namespace ZenCare.Model.SearchObjects
{
    public class UserSearchObject : PagedSearchObject
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string? Username { get; set; }

        public bool? IsActive { get; set; }

        public bool? IsClient { get; set; }
    }
}
