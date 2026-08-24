class TimeSlot {
  TimeSlot({
    required this.id,
    required this.employeeId,
    required this.employeeName,
    required this.wellnessServiceId,
    required this.serviceName,
    required this.slotDate,
    required this.startTime,
    required this.endTime,
    required this.isActive,
    required this.isBooked,
    required this.isAvailable,
    required this.status,
  });

  final int id;
  final int employeeId;
  final String employeeName;
  final int wellnessServiceId;
  final String serviceName;
  final DateTime slotDate;
  final Duration startTime;
  final Duration endTime;
  final bool isActive;
  final bool isBooked;
  final bool isAvailable;
  final String status;

  factory TimeSlot.fromJson(Map<String, dynamic> json) => TimeSlot(
        id: json['id'] as int? ?? 0,
        employeeId: json['employeeId'] as int? ?? 0,
        employeeName: json['employeeName'] as String? ?? '',
        wellnessServiceId: json['wellnessServiceId'] as int? ?? 0,
        serviceName: json['serviceName'] as String? ?? '',
        slotDate: DateTime.parse(json['slotDate'] as String),
        startTime: _parseTimeSpan(json['startTime'] as String? ?? '00:00:00'),
        endTime: _parseTimeSpan(json['endTime'] as String? ?? '00:00:00'),
        isActive: json['isActive'] as bool? ?? false,
        isBooked: json['isBooked'] as bool? ?? false,
        isAvailable: json['isAvailable'] as bool? ?? false,
        status: json['status'] as String? ?? '',
      );

  String get displayLabel =>
      '${_formatDate(slotDate)}  ${_formatTime(startTime)}-${_formatTime(endTime)}  $employeeName';
}

Duration _parseTimeSpan(String value) {
  final parts = value.split(':');
  if (parts.length < 2) return Duration.zero;
  return Duration(
    hours: int.tryParse(parts[0]) ?? 0,
    minutes: int.tryParse(parts[1]) ?? 0,
  );
}

String _formatDate(DateTime value) =>
    '${value.year.toString().padLeft(4, '0')}-${value.month.toString().padLeft(2, '0')}-${value.day.toString().padLeft(2, '0')}';

String _formatTime(Duration value) {
  final hours = value.inHours.toString().padLeft(2, '0');
  final minutes = value.inMinutes.remainder(60).toString().padLeft(2, '0');
  return '$hours:$minutes';
}
