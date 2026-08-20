using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using ZenCare.Model.Constants;
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
        private const int NameMaxLength = 50;
        private const int UsernameMaxLength = 100;
        private const int EmailMaxLength = 100;
        private const int PasswordMinLength = 6;
        private const string PhoneNumberMessage = "Phone number must contain 9 or 10 digits (numbers only).";
        private static readonly Regex PhoneNumberRegex = new(@"^\d{9,10}$", RegexOptions.Compiled);

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
            var firstName = NormalizeRequired(request.FirstName, "First name", NameMaxLength);
            var lastName = NormalizeRequired(request.LastName, "Last name", NameMaxLength);
            var username = NormalizeRequired(request.Username, "Username", UsernameMaxLength);
            var email = NormalizeEmail(request.Email);
            var phoneNumber = NormalizePhoneNumber(request.PhoneNumber);

            if (string.IsNullOrEmpty(request.Password))
            {
                throw new BusinessException("Password is required.");
            }

            if (request.Password.Length < PasswordMinLength)
            {
                throw new BusinessException("Password must contain at least 6 characters.");
            }

            if (string.IsNullOrEmpty(request.PasswordConfirm))
            {
                throw new BusinessException("Confirm password is required.");
            }

            if (request.Password != request.PasswordConfirm)
            {
                throw new BusinessException("Passwords do not match.");
            }

            var usernameExists = await DbContext.Users.AnyAsync(u => u.Username == username);
            if (usernameExists)
            {
                throw new BusinessException("This username is already in use. Enter a different username.");
            }

            var emailExists = await DbContext.Users.AnyAsync(u => u.Email == email);
            if (emailExists)
            {
                throw new BusinessException("This email address is already in use. Enter a different email address.");
            }

            var clientRole = await DbContext.Roles
                .FirstOrDefaultAsync(role => role.Name == AppRoles.Client || role.RoleType == UserRoleType.Client);

            if (clientRole == null)
            {
                throw new BusinessException("Client role was not found.");
            }

            await using var transaction = await DbContext.Database.BeginTransactionAsync();

            var salt = PasswordHasher.GenerateSalt();
            var createdAt = DateTime.UtcNow;
            var user = new Database.User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Username = username,
                PhoneNumber = phoneNumber,
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

        public async Task<UserResponse> GetMyProfileAsync(int userId)
        {
            var entity = await DbContext.Users.FindAsync(userId);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Database.User), userId);
            }

            return Mapper.Map<UserResponse>(entity);
        }

        public async Task<UserResponse> UpdateMyProfileAsync(int userId, UpdateMyProfileRequest request)
        {
            var entity = await DbContext.Users.FindAsync(userId);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Database.User), userId);
            }

            var firstName = NormalizeRequired(request.FirstName, "First name", NameMaxLength);
            var lastName = NormalizeRequired(request.LastName, "Last name", NameMaxLength);
            var email = NormalizeEmail(request.Email);
            var phoneNumber = NormalizePhoneNumber(request.PhoneNumber);

            var emailExists = await DbContext.Users.AnyAsync(u => u.Id != userId && u.Email == email);
            if (emailExists)
            {
                throw new BusinessException("This email address is already in use. Enter a different email address.");
            }

            entity.FirstName = firstName;
            entity.LastName = lastName;
            entity.Email = email;
            entity.PhoneNumber = phoneNumber;

            SetUpdatedAt(entity);

            await DbContext.SaveChangesAsync();

            return Mapper.Map<UserResponse>(entity);
        }

        public async Task<ChangePasswordResponse> ChangeMyPasswordAsync(int userId, ChangePasswordRequest request)
        {
            var entity = await DbContext.Users.FindAsync(userId);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Database.User), userId);
            }

            ValidateChangePasswordRequest(request);

            if (!PasswordHasher.Verify(entity.PasswordHash, entity.PasswordSalt, request.CurrentPassword))
            {
                throw new BusinessException("Current password is incorrect.");
            }

            var salt = PasswordHasher.GenerateSalt();
            entity.PasswordSalt = salt;
            entity.PasswordHash = PasswordHasher.GenerateHash(request.NewPassword, salt);

            SetUpdatedAt(entity);

            await DbContext.SaveChangesAsync();

            return new ChangePasswordResponse
            {
                Message = "Password changed successfully."
            };
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

                if (search.IsClient.HasValue)
                {
                    query = search.IsClient.Value
                        ? query.Where(u =>
                            u.ClientProfile != null &&
                            u.UserRoles.Any(ur => ur.Role.Name == AppRoles.Client || ur.Role.RoleType == UserRoleType.Client))
                        : query.Where(u =>
                            u.ClientProfile == null ||
                            !u.UserRoles.Any(ur => ur.Role.Name == AppRoles.Client || ur.Role.RoleType == UserRoleType.Client));
                }
            }

            return query;
        }

        private static string NormalizeRequired(string value, string fieldName, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new BusinessException($"{fieldName} is required.");
            }

            var normalizedValue = value.Trim();

            if (normalizedValue.Length > maxLength)
            {
                throw new BusinessException($"{fieldName} must not exceed {maxLength} characters.");
            }

            return normalizedValue;
        }

        private static string NormalizeEmail(string value)
        {
            var email = NormalizeRequired(value, "Email", EmailMaxLength);

            if (!new EmailAddressAttribute().IsValid(email))
            {
                throw new BusinessException("Email must be in the format: user@example.com.");
            }

            return email;
        }

        private static string? NormalizePhoneNumber(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var phoneNumber = value.Trim();

            if (!PhoneNumberRegex.IsMatch(phoneNumber))
            {
                throw new BusinessException(PhoneNumberMessage);
            }

            return phoneNumber;
        }

        private static void ValidateChangePasswordRequest(ChangePasswordRequest request)
        {
            if (string.IsNullOrEmpty(request.CurrentPassword))
            {
                throw new BusinessException("Current password is required.");
            }

            if (string.IsNullOrEmpty(request.NewPassword))
            {
                throw new BusinessException("New password is required.");
            }

            if (request.NewPassword.Length < PasswordMinLength)
            {
                throw new BusinessException("New password must contain at least 6 characters.");
            }

            if (string.IsNullOrEmpty(request.ConfirmNewPassword))
            {
                throw new BusinessException("Confirm new password is required.");
            }

            if (request.NewPassword != request.ConfirmNewPassword)
            {
                throw new BusinessException("Passwords do not match.");
            }
        }
    }
}
