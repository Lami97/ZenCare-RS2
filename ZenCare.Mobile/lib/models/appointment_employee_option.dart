class AppointmentEmployeeOption {
  AppointmentEmployeeOption({
    required this.employeeId,
    required this.fullName,
    this.specialization,
    required this.isAvailable,
  });

  final int employeeId;
  final String fullName;
  final String? specialization;
  final bool isAvailable;

  factory AppointmentEmployeeOption.fromJson(Map<String, dynamic> json) {
    return AppointmentEmployeeOption(
      employeeId: json['employeeId'] as int? ?? 0,
      fullName: json['fullName'] as String? ?? '',
      specialization: json['specialization'] as String?,
      isAvailable: json['isAvailable'] as bool? ?? false,
    );
  }

  String get displayName {
    if ((specialization ?? '').trim().isEmpty) {
      return fullName;
    }

    return '$fullName - $specialization';
  }
}
