import '../models/appointment.dart';
import '../models/appointment_create_request.dart';
import '../models/employee.dart';
import '../models/paged_result.dart';
import '../models/wellness_service.dart';
import 'api_service.dart';

class AppointmentService {
  AppointmentService(this._apiService);

  final ApiService _apiService;

  Future<PagedResult<Appointment>> getMyAppointments({
    int? employeeId,
    int? wellnessServiceId,
    int? status,
    DateTime? appointmentDate,
  }) {
    return _apiService.get<PagedResult<Appointment>>(
      '/Appointment/My',
      queryParameters: {
        if (employeeId != null) 'EmployeeId': employeeId,
        if (wellnessServiceId != null) 'WellnessServiceId': wellnessServiceId,
        if (status != null) 'Status': status,
        if (appointmentDate != null) 'AppointmentDate': DateTime.utc(appointmentDate.year, appointmentDate.month, appointmentDate.day).toIso8601String(),
        'IncludeTotalCount': true,
      },
      fromJson: (data) => PagedResult<Appointment>.fromJson(
        data as Map<String, dynamic>,
        Appointment.fromJson,
      ),
    );
  }

  Future<Appointment> getMyAppointmentById(int id) {
    return _apiService.get<Appointment>(
      '/Appointment/My/$id',
      fromJson: (data) => Appointment.fromJson(data as Map<String, dynamic>),
    );
  }

  Future<Appointment> createMyAppointment(AppointmentCreateRequest request) {
    return _apiService.post<Appointment>(
      '/Appointment/My',
      data: request.toJson(),
      fromJson: (data) => Appointment.fromJson(data as Map<String, dynamic>),
    );
  }

  Future<PagedResult<Employee>> getAvailableEmployees() {
    return _apiService.get<PagedResult<Employee>>(
      '/Employee',
      queryParameters: {
        'IsAvailable': true,
        'IncludeTotalCount': true,
      },
      fromJson: (data) => PagedResult<Employee>.fromJson(
        data as Map<String, dynamic>,
        Employee.fromJson,
      ),
    );
  }

  Future<PagedResult<WellnessService>> getActiveServices() {
    return _apiService.get<PagedResult<WellnessService>>(
      '/Service',
      queryParameters: {
        'IsActive': true,
        'IncludeTotalCount': true,
      },
      fromJson: (data) => PagedResult<WellnessService>.fromJson(
        data as Map<String, dynamic>,
        WellnessService.fromJson,
      ),
    );
  }
}
