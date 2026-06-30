namespace ZenCare.Model.SearchObjects
{
    public class EmployeeSearchObject : BaseSearchObject
    {
        public int? UserId { get; set; }

        public string? Specialization { get; set; }

        public bool? IsAvailable { get; set; }
    }
}
