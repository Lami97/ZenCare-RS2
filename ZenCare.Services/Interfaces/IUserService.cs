using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;

namespace ZenCare.Services.Interfaces
{
    public interface IUserService : ICRUDService<UserResponse, UserInsertRequest, UserUpdateRequest, UserSearchObject>
    {
    }
}
