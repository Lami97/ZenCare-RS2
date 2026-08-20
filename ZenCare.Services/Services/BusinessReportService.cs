using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ZenCare.Model.Enums;
using ZenCare.Model.Exceptions;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

namespace ZenCare.Services.Services
{
    public class BusinessReportService : BaseCRUDService<BusinessReportResponse, Database.BusinessReport, BusinessReportInsertRequest, BusinessReportUpdateRequest, BusinessReportSearchObject>, IBusinessReportService
    {
        private readonly TimeZoneInfo _businessTimeZone;

        public BusinessReportService(
            ZenCareDbContext dbContext,
            IMapper mapper,
            TimeZoneInfo businessTimeZone) : base(dbContext, mapper)
        {
            _businessTimeZone = businessTimeZone;
        }

        public override async Task<BusinessReportResponse> GetByIdAsync(int id)
        {
            var entity = await DbContext.BusinessReports
                .Include(br => br.GeneratedByUser)
                .FirstOrDefaultAsync(br => br.Id == id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Database.BusinessReport), id);
            }

            return Mapper.Map<BusinessReportResponse>(entity);
        }

        public async Task<BusinessAnalyticsResponse> GetAnalyticsAsync(BusinessReportSearchObject? search = null)
        {
            var dateFrom = search?.DateFrom.HasValue == true
                ? DateTime.SpecifyKind(search.DateFrom.Value.Date, DateTimeKind.Unspecified)
                : (DateTime?)null;
            var dateTo = search?.DateTo.HasValue == true
                ? DateTime.SpecifyKind(search.DateTo.Value.Date, DateTimeKind.Unspecified)
                : (DateTime?)null;

            if (dateFrom.HasValue && dateTo.HasValue && dateFrom > dateTo)
            {
                throw new BusinessException("From date must be before or equal to To date.");
            }

            var dateToExclusive = dateTo?.AddDays(1);
            var timestampFromUtc = dateFrom.HasValue
                ? TimeZoneInfo.ConvertTimeToUtc(dateFrom.Value, _businessTimeZone)
                : (DateTime?)null;
            var timestampToExclusiveUtc = dateToExclusive.HasValue
                ? TimeZoneInfo.ConvertTimeToUtc(dateToExclusive.Value, _businessTimeZone)
                : (DateTime?)null;
            var appointments = DbContext.Appointments.AsNoTracking();
            if (dateFrom.HasValue)
            {
                appointments = appointments.Where(appointment => appointment.AppointmentDate >= dateFrom.Value);
            }
            if (dateToExclusive.HasValue)
            {
                appointments = appointments.Where(appointment => appointment.AppointmentDate < dateToExclusive.Value);
            }

            var purchases = DbContext.Purchases.AsNoTracking();
            if (timestampFromUtc.HasValue)
            {
                purchases = purchases.Where(purchase => purchase.CreatedAt >= timestampFromUtc.Value);
            }
            if (timestampToExclusiveUtc.HasValue)
            {
                purchases = purchases.Where(purchase => purchase.CreatedAt < timestampToExclusiveUtc.Value);
            }

            var successfulPurchases = DbContext.Purchases
                .AsNoTracking()
                .Where(purchase =>
                    purchase.Status == PurchaseStatus.Completed
                    && purchase.PaymentStatus == PaymentStatus.Succeeded
                    && purchase.PaidAt.HasValue);
            if (timestampFromUtc.HasValue)
            {
                successfulPurchases = successfulPurchases.Where(purchase => purchase.PaidAt >= timestampFromUtc.Value);
            }
            if (timestampToExclusiveUtc.HasValue)
            {
                successfulPurchases = successfulPurchases.Where(purchase => purchase.PaidAt < timestampToExclusiveUtc.Value);
            }

            var completedAppointments = appointments.Where(appointment => appointment.Status == AppointmentStatus.Completed);

            var appointmentStatusGroups = await appointments
                .GroupBy(appointment => appointment.Status)
                .Select(group => new { Status = group.Key, Count = group.Count() })
                .OrderBy(group => group.Status)
                .ToListAsync();

            var bestSellingProducts = await DbContext.PurchaseItems
                .AsNoTracking()
                .Where(item => successfulPurchases.Any(purchase => purchase.Id == item.PurchaseId))
                .GroupBy(item => new { item.ProductId, item.Product.Name })
                .Select(group => new BestSellingProductAnalyticsResponse
                {
                    ProductId = group.Key.ProductId,
                    ProductName = group.Key.Name,
                    QuantitySold = group.Sum(item => item.Quantity),
                    Revenue = group.Sum(item => item.TotalPrice)
                })
                .OrderByDescending(item => item.QuantitySold)
                .ThenByDescending(item => item.Revenue)
                .ThenBy(item => item.ProductName)
                .Take(10)
                .ToListAsync();

            var serviceUsage = await completedAppointments
                .GroupBy(appointment => new { appointment.WellnessServiceId, appointment.WellnessService.Name })
                .Select(group => new ServiceUsageAnalyticsResponse
                {
                    ServiceId = group.Key.WellnessServiceId,
                    ServiceName = group.Key.Name,
                    AppointmentCount = group.Count()
                })
                .OrderByDescending(item => item.AppointmentCount)
                .ThenBy(item => item.ServiceName)
                .Take(10)
                .ToListAsync();

            var weekAnchor = new DateTime(2000, 1, 3, 0, 0, 0, DateTimeKind.Utc);
            var weeklyGroups = await completedAppointments
                .GroupBy(appointment => EF.Functions.DateDiffDay(weekAnchor, appointment.AppointmentDate) / 7)
                .Select(group => new { WeekOffset = group.Key, AttendanceCount = group.Count() })
                .OrderBy(group => group.WeekOffset)
                .ToListAsync();

            var employeeWorkload = await completedAppointments
                .GroupBy(appointment => new
                {
                    appointment.EmployeeId,
                    appointment.Employee.User.FirstName,
                    appointment.Employee.User.LastName
                })
                .Select(group => new
                {
                    Name = group.Key.FirstName + " " + group.Key.LastName,
                    Count = group.Count()
                })
                .OrderByDescending(item => item.Count)
                .ThenBy(item => item.Name)
                .Take(10)
                .ToListAsync();

            var clientActivity = await completedAppointments
                .GroupBy(appointment => new { appointment.UserId, appointment.User.Username })
                .Select(group => new NamedCountAnalyticsResponse
                {
                    Name = group.Key.Username,
                    Count = group.Count()
                })
                .OrderByDescending(item => item.Count)
                .ThenBy(item => item.Name)
                .Take(10)
                .ToListAsync();

            var successfulClientIds = successfulPurchases.Select(purchase => purchase.UserId);
            var attendedClientIds = completedAppointments.Select(appointment => appointment.UserId);

            return new BusinessAnalyticsResponse
            {
                DateFrom = dateFrom,
                DateTo = dateTo,
                TotalRevenue = await successfulPurchases.SumAsync(purchase => (decimal?)purchase.TotalAmount) ?? 0,
                CompletedPurchases = await successfulPurchases.CountAsync(),
                CompletedAppointments = await completedAppointments.CountAsync(),
                UniqueClients = await successfulClientIds.Union(attendedClientIds).Distinct().CountAsync(),
                TotalUsers = await DbContext.Users.AsNoTracking().CountAsync(),
                TotalEmployees = await DbContext.Employees.AsNoTracking().CountAsync(),
                TotalAppointments = await appointments.CountAsync(),
                TotalServices = await DbContext.WellnessServices.AsNoTracking().CountAsync(),
                TotalProducts = await DbContext.Products.AsNoTracking().CountAsync(),
                TotalPurchases = await purchases.CountAsync(),
                BestSellingProducts = bestSellingProducts,
                ServiceUsage = serviceUsage,
                WeeklyAttendance = weeklyGroups.Select(group => new WeeklyAttendanceAnalyticsResponse
                {
                    WeekStart = weekAnchor.AddDays(group.WeekOffset * 7),
                    AttendanceCount = group.AttendanceCount
                }).ToList(),
                AppointmentStatuses = appointmentStatusGroups.Select(group => new StatusCountAnalyticsResponse
                {
                    Status = group.Status.ToString(),
                    Count = group.Count
                }).ToList(),
                EmployeeWorkload = employeeWorkload.Select(item => new NamedCountAnalyticsResponse
                {
                    Name = item.Name.Trim(),
                    Count = item.Count
                }).ToList(),
                ClientActivity = clientActivity
            };
        }

        protected override IQueryable<Database.BusinessReport> ApplyFilters(IQueryable<Database.BusinessReport> query, BusinessReportSearchObject? search)
        {
            if (search != null)
            {
                if (search.GeneratedByUserId.HasValue)
                {
                    query = query.Where(br => br.GeneratedByUserId == search.GeneratedByUserId.Value);
                }

                if (!string.IsNullOrWhiteSpace(search.ReportType))
                {
                    query = query.Where(br => br.ReportType.Contains(search.ReportType));
                }

                if (search.DateFrom.HasValue)
                {
                    query = query.Where(br => br.DateFrom.Date >= search.DateFrom.Value.Date);
                }

                if (search.DateTo.HasValue)
                {
                    query = query.Where(br => br.DateTo.Date <= search.DateTo.Value.Date);
                }
            }

            return query;
        }

        protected override Task<IQueryable<Database.BusinessReport>> IncludeRelatedEntitiesAsync(IQueryable<Database.BusinessReport> query, BusinessReportSearchObject? search)
        {
            query = query.Include(br => br.GeneratedByUser);

            return Task.FromResult(query);
        }
    }
}
