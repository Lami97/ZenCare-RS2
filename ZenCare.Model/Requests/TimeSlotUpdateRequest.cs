using System.ComponentModel.DataAnnotations;

namespace ZenCare.Model.Requests;

public class TimeSlotUpdateRequest : TimeSlotInsertRequest
{
    [Range(1, int.MaxValue)]
    public int Id { get; set; }
}
