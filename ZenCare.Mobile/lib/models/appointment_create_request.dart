import 'appointment_status.dart';

class AppointmentCreateRequest {
  AppointmentCreateRequest({
    required this.userId,
    required this.employeeId,
    required this.wellnessServiceId,
    required this.appointmentDate,
    required this.startTime,
    required this.endTime,
    this.status = AppointmentStatus.pending,
    this.notes,
    this.cancellationReason,
  });

  final int userId;
  final int employeeId;
  final int wellnessServiceId;
  final DateTime appointmentDate;
  final Duration startTime;
  final Duration endTime;
  final AppointmentStatus status;
  final String? notes;
  final String? cancellationReason;

  Map<String, dynamic> toJson() {
    return {
      'userId': userId,
      'employeeId': employeeId,
      'wellnessServiceId': wellnessServiceId,
      'appointmentDate': DateTime.utc(appointmentDate.year, appointmentDate.month, appointmentDate.day).toIso8601String(),
      'startTime': _formatTime(startTime),
      'endTime': _formatTime(endTime),
      'status': status.value,
      'notes': notes,
      'cancellationReason': cancellationReason,
    };
  }

  static String _formatTime(Duration value) {
    final hours = value.inHours.toString().padLeft(2, '0');
    final minutes = value.inMinutes.remainder(60).toString().padLeft(2, '0');
    final seconds = value.inSeconds.remainder(60).toString().padLeft(2, '0');

    return '$hours:$minutes:$seconds';
  }
}
