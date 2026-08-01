using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ZenCare.Model.Enums;
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

        public async Task<AdminCreateClientResponse> CreateClientAsync(AdminCreateClientRequest request)
        {
            if (request.Password != request.PasswordConfirm)
            {
                throw new BusinessException("Password and confirmation password do not match.");
            }

            var usernameExists = await DbContext.Users.AnyAsync(u => u.Username == request.Username);
            if (usernameExists)
            {
                throw new BusinessException("Username is already in use.");
            }

            var emailExists = await DbContext.Users.AnyAsync(u => u.Email == request.Email);
            if (emailExists)
            {
                throw new BusinessException("Email is already in use.");
            }

            var clientRole = await DbContext.Roles
                .FirstOrDefaultAsync(role => role.Name == "Client" || role.RoleType == UserRoleType.Client);

            if (clientRole == null)
            {
                throw new BusinessException("Client role was not found.");
            }

            await using var transaction = await DbContext.Database.BeginTransactionAsync();

            var salt = PasswordHasher.GenerateSalt();
            var createdAt = DateTime.UtcNow;
            var user = new Database.User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Username = request.Username,
                PhoneNumber = request.PhoneNumber,
                PasswordSalt = salt,
                PasswordHash = PasswordHasher.GenerateHash(request.Password, salt),
                IsActive = request.IsActive,
                CreatedAt = createdAt
            };

            DbContext.Users.Add(user);
            await DbContext.SaveChangesAsync();

            DbContext.UserRoles.Add(new Database.UserRole
            {
                UserId = user.Id,
                RoleId = clientRole.Id,
                CreatedAt = DateTime.UtcNow
            });

            var clientProfile = new Database.ClientProfile
            {
                UserId = user.Id,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                HealthNotes = request.HealthNotes,
                Preferences = request.Preferences,
                CreatedAt = DateTime.UtcNow
            };

            DbContext.ClientProfiles.Add(clientProfile);
            await DbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return new AdminCreateClientResponse
            {
                UserId = user.Id,
                ClientProfileId = clientProfile.Id,
                Username = user.Username,
                Email = user.Email,
                Role = clientRole.Name,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
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
