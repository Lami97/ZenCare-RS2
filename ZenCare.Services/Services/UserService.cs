using AutoMapper;
using ZenCare.Model.Exceptions;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;
using ZenCare.Services.Security;

namespace ZenCare.Services.Services
{
    public class UserService : BaseCRUDService<UserResponse, Database.User, UserInsertRequest, UserUpdateRequest, UserSearchObject>, IUserService
    {
        public UserService(ZenCareDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public override async Task<UserResponse> InsertAsync(UserInsertRequest request)
        {
            var entity = Mapper.Map<Database.User>(request);
            var salt = PasswordHasher.GenerateSalt();

            entity.PasswordSalt = salt;
            entity.PasswordHash = PasswordHasher.GenerateHash(request.Password, salt);

            SetCreatedAt(entity);

            DbContext.Users.Add(entity);
            await DbContext.SaveChangesAsync();

            return Mapper.Map<UserResponse>(entity);
        }

        public override async Task<UserResponse> UpdateAsync(int id, UserUpdateRequest request)
        {
            var entity = await DbContext.Users.FindAsync(id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Database.User), id);
            }

            var passwordHash = entity.PasswordHash;
            var passwordSalt = entity.PasswordSalt;

            Mapper.Map(request, entity);

            entity.PasswordHash = passwordHash;
            entity.PasswordSalt = passwordSalt;

            SetUpdatedAt(entity);

            await DbContext.SaveChangesAsync();

            return Mapper.Map<UserResponse>(entity);
        }

        protected override IQueryable<Database.User> ApplyFilters(IQueryable<Database.User> query, UserSearchObject? search)
        {
            if (search != null)
            {
                if (!string.IsNullOrWhiteSpace(search.FirstName))
                {
                    query = query.Where(u => u.FirstName.Contains(search.FirstName));
                }

                if (!string.IsNullOrWhiteSpace(search.LastName))
                {
                    query = query.Where(u => u.LastName.Contains(search.LastName));
                }

                if (!string.IsNullOrWhiteSpace(search.Email))
                {
                    query = query.Where(u => u.Email.Contains(search.Email));
                }

                if (!string.IsNullOrWhiteSpace(search.Username))
                {
                    query = query.Where(u => u.Username.Contains(search.Username));
                }

                if (search.IsActive.HasValue)
                {
                    query = query.Where(u => u.IsActive == search.IsActive.Value);
                }
            }

            return query;
        }

    }
}


