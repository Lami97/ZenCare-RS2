namespace ZenCare.Model.SearchObjects
{
    public class EmployeeServiceSearchObject : BaseSearchObject
    {
        public int? EmployeeId { get; set; }

        public int? WellnessServiceId { get; set; }

        public bool? IsActive { get; set; }
    }
}
