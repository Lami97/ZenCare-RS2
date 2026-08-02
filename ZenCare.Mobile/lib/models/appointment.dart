import 'appointment_status.dart';

class Appointment {
  Appointment({
    required this.id,
    required this.userId,
    required this.userName,
    required this.employeeId,
    required this.employeeName,
    required this.wellnessServiceId,
    required this.serviceName,
    required this.appointmentDate,
    required this.startTime,
    required this.endTime,
    required this.status,
    this.notes,
    this.cancellationReason,
    required this.createdAt,
    this.updatedAt,
  });

  final int id;
  final int userId;
  final String userName;
  final int employeeId;
  final String employeeName;
  final int wellnessServiceId;
  final String serviceName;
  final DateTime appointmentDate;
  final Duration startTime;
  final Duration endTime;
  final AppointmentStatus status;
  final String? notes;
  final String? cancellationReason;
  final DateTime createdAt;
  final DateTime? updatedAt;

  factory Appointment.fromJson(Map<String, dynamic> json) {
    return Appointment(
      id: json['id'] as int? ?? 0,
      userId: json['userId'] as int? ?? 0,
      userName: json['userName'] as String? ?? '',
      employeeId: json['employeeId'] as int? ?? 0,
      employeeName: json['employeeName'] as String? ?? '',
      wellnessServiceId: json['wellnessServiceId'] as int? ?? 0,
      serviceName: json['serviceName'] as String? ?? '',
      appointmentDate: DateTime.parse(json['appointmentDate'] as String),
      startTime: _parseTimeSpan(json['startTime'] as String? ?? '00:00:00'),
      endTime: _parseTimeSpan(json['endTime'] as String? ?? '00:00:00'),
      status: AppointmentStatus.fromValue(json['status'] as int? ?? 1),
      notes: json['notes'] as String?,
      cancellationReason: json['cancellationReason'] as String?,
      createdAt: DateTime.parse(json['createdAt'] as String),
      updatedAt: json['updatedAt'] == null ? null : DateTime.parse(json['updatedAt'] as String),
    );
  }

  static Duration _parseTimeSpan(String value) {
    final parts = value.split(':');
    if (parts.length < 2) {
      return Duration.zero;
    }

    final hours = int.tryParse(parts[0]) ?? 0;
    final minutes = int.tryParse(parts[1]) ?? 0;
    final seconds = parts.length > 2 ? int.tryParse(parts[2].split('.').first) ?? 0 : 0;

    return Duration(hours: hours, minutes: minutes, seconds: seconds);
  }
}
