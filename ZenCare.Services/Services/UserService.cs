using AutoMapper;
using System.Security.Cryptography;
using System.Text;
using ZenCare.Model.Exceptions;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

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
            var salt = GenerateSalt();

            entity.PasswordSalt = salt;
            entity.PasswordHash = GenerateHash(request.Password, salt);

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

        private static string GenerateSalt()
        {
            using var rng = RandomNumberGenerator.Create();
            var saltBytes = new byte[16];
            rng.GetBytes(saltBytes);

            return Convert.ToBase64String(saltBytes);
        }

        private static string GenerateHash(string password, string salt)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(salt), 10000, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(20);

            return Convert.ToBase64String(hash);
        }
    }
}
