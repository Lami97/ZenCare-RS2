import '../models/appointment.dart';
import '../models/appointment_create_request.dart';
import '../models/appointment_employee_option.dart';
import '../models/paged_result.dart';
import '../models/time_slot.dart';
import 'api_service.dart';

class AppointmentService {
  AppointmentService(this._apiService);

  final ApiService _apiService;

  Future<PagedResult<Appointment>> getMyAppointments({
    int? employeeId,
    int? wellnessServiceId,
    int? status,
    DateTime? appointmentDate,
    int page = 1,
    int pageSize = 20,
  }) {
    return _apiService.get<PagedResult<Appointment>>(
      '/Appointment/My',
      queryParameters: {
        if (employeeId != null) 'EmployeeId': employeeId,
        if (wellnessServiceId != null) 'WellnessServiceId': wellnessServiceId,
        if (status != null) 'Status': status,
        if (appointmentDate != null)
          'AppointmentDate': DateTime.utc(appointmentDate.year,
                  appointmentDate.month, appointmentDate.day)
              .toIso8601String(),
        'Page': page,
        'PageSize': pageSize,
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

  Future<PagedResult<TimeSlot>> getAvailableTimeSlots({
    required int wellnessServiceId,
    int page = 1,
    int pageSize = 100,
  }) {
    return _apiService.get<PagedResult<TimeSlot>>(
      '/TimeSlot/Available',
      queryParameters: {
        'WellnessServiceId': wellnessServiceId,
        'Page': page,
        'PageSize': pageSize,
        'IncludeTotalCount': true,
      },
      fromJson: (data) => PagedResult<TimeSlot>.fromJson(
        data as Map<String, dynamic>,
        TimeSlot.fromJson,
      ),
    );
  }

  Future<Appointment> cancelMyAppointment({
    required int id,
    required String cancellationReason,
  }) {
    return _apiService.post<Appointment>(
      '/Appointment/My/cancel/$id',
      data: {
        'cancellationReason': cancellationReason,
      },
      fromJson: (data) => Appointment.fromJson(data as Map<String, dynamic>),
    );
  }

  Future<List<AppointmentEmployeeOption>> getAvailableEmployees({
    required int wellnessServiceId,
    DateTime? appointmentDate,
    Duration? startTime,
    Duration? endTime,
  }) {
    return _apiService.get<List<AppointmentEmployeeOption>>(
      '/Appointment/My/available-employees',
      queryParameters: {
        'wellnessServiceId': wellnessServiceId,
        if (appointmentDate != null)
          'appointmentDate': DateTime.utc(appointmentDate.year,
                  appointmentDate.month, appointmentDate.day)
              .toIso8601String(),
        if (startTime != null) 'startTime': _formatTime(startTime),
        if (endTime != null) 'endTime': _formatTime(endTime),
        'page': 1,
        'pageSize': 100,
      },
      fromJson: (data) => (data as List<dynamic>)
          .map((item) =>
              AppointmentEmployeeOption.fromJson(item as Map<String, dynamic>))
          .toList(),
    );
  }

  String _formatTime(Duration value) {
    final hours = value.inHours.toString().padLeft(2, '0');
    final minutes = value.inMinutes.remainder(60).toString().padLeft(2, '0');
    final seconds = value.inSeconds.remainder(60).toString().padLeft(2, '0');

    return '$hours:$minutes:$seconds';
  }
}
