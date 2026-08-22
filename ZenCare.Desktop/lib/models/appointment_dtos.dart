import 'admin_models.dart';

class AppointmentDto implements AdminEntity {
  const AppointmentDto({
    required this.id,
    required this.userId,
    required this.userName,
    required this.employeeId,
    required this.employeeName,
    required this.wellnessServiceId,
    required this.serviceName,
    required this.serviceCategoryName,
    required this.appointmentDate,
    required this.startTime,
    required this.endTime,
    required this.status,
    this.notes,
    this.cancellationReason,
    required this.createdAt,
    this.updatedAt,
  });
  @override
  final int id;
  final int userId;
  final String userName;
  final int employeeId;
  final String employeeName;
  final int wellnessServiceId;
  final String serviceName;
  final String serviceCategoryName;
  final DateTime appointmentDate;
  final String startTime;
  final String endTime;
  final int status;
  final String? notes;
  final String? cancellationReason;
  final DateTime createdAt;
  final DateTime? updatedAt;
  factory AppointmentDto.fromJson(JsonMap json) => AppointmentDto(
    id: jsonInt(json['id']),
    userId: jsonInt(json['userId']),
    userName: json['userName']?.toString() ?? '',
    employeeId: jsonInt(json['employeeId']),
    employeeName: json['employeeName']?.toString() ?? '',
    wellnessServiceId: jsonInt(json['wellnessServiceId']),
    serviceName: json['serviceName']?.toString() ?? '',
    serviceCategoryName: json['serviceCategoryName']?.toString() ?? '',
    appointmentDate:
        jsonDateTime(json['appointmentDate']) ??
        DateTime.fromMillisecondsSinceEpoch(0, isUtc: true),
    startTime: json['startTime']?.toString() ?? '',
    endTime: json['endTime']?.toString() ?? '',
    status: jsonInt(json['status']),
    notes: json['notes']?.toString(),
    cancellationReason: json['cancellationReason']?.toString(),
    createdAt:
        jsonDateTime(json['createdAt']) ??
        DateTime.fromMillisecondsSinceEpoch(0, isUtc: true),
    updatedAt: jsonDateTime(json['updatedAt']),
  );
  @override
  Object? formValue(String key) => switch (key) {
    'userId' => userId,
    'employeeId' => employeeId,
    'wellnessServiceId' => wellnessServiceId,
    'appointmentDate' => appointmentDate,
    'startTime' => startTime,
    'endTime' => endTime,
    'status' => status,
    'notes' => notes,
    'cancellationReason' => cancellationReason,
    _ => null,
  };
}

class AppointmentInsertDto implements AdminWriteDto {
  const AppointmentInsertDto({
    required this.userId,
    required this.employeeId,
    required this.wellnessServiceId,
    required this.appointmentDate,
    required this.startTime,
    required this.endTime,
    required this.status,
    this.notes,
    this.cancellationReason,
  });
  final int userId;
  final int employeeId;
  final int wellnessServiceId;
  final DateTime appointmentDate;
  final String startTime;
  final String endTime;
  final int status;
  final String? notes;
  final String? cancellationReason;
  @override
  JsonMap toJson() => {
    'userId': userId,
    'employeeId': employeeId,
    'wellnessServiceId': wellnessServiceId,
    'appointmentDate': appointmentDate.toIso8601String(),
    'startTime': startTime,
    'endTime': endTime,
    'status': status,
    'notes': notes,
    'cancellationReason': cancellationReason,
  };
}

class AppointmentUpdateDto extends AppointmentInsertDto {
  const AppointmentUpdateDto({
    required this.id,
    required super.userId,
    required super.employeeId,
    required super.wellnessServiceId,
    required super.appointmentDate,
    required super.startTime,
    required super.endTime,
    required super.status,
    super.notes,
    super.cancellationReason,
  });
  final int id;
  @override
  JsonMap toJson() => {...super.toJson(), 'id': id};
}
