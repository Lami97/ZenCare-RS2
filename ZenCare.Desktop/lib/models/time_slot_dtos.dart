import 'admin_models.dart';

class TimeSlotDto implements AdminEntity {
  const TimeSlotDto({
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
    required this.createdAt,
    this.updatedAt,
  });

  @override
  final int id;
  final int employeeId;
  final String employeeName;
  final int wellnessServiceId;
  final String serviceName;
  final DateTime slotDate;
  final String startTime;
  final String endTime;
  final bool isActive;
  final bool isBooked;
  final bool isAvailable;
  final String status;
  final DateTime createdAt;
  final DateTime? updatedAt;

  factory TimeSlotDto.fromJson(JsonMap json) => TimeSlotDto(
    id: jsonInt(json['id']),
    employeeId: jsonInt(json['employeeId']),
    employeeName: json['employeeName']?.toString() ?? '',
    wellnessServiceId: jsonInt(json['wellnessServiceId']),
    serviceName: json['serviceName']?.toString() ?? '',
    slotDate:
        jsonDateTime(json['slotDate']) ??
        DateTime.fromMillisecondsSinceEpoch(0, isUtc: true),
    startTime: json['startTime']?.toString() ?? '',
    endTime: json['endTime']?.toString() ?? '',
    isActive: jsonBool(json['isActive']),
    isBooked: jsonBool(json['isBooked']),
    isAvailable: jsonBool(json['isAvailable']),
    status: json['status']?.toString() ?? '',
    createdAt:
        jsonDateTime(json['createdAt']) ??
        DateTime.fromMillisecondsSinceEpoch(0, isUtc: true),
    updatedAt: jsonDateTime(json['updatedAt']),
  );

  @override
  Object? formValue(String key) => switch (key) {
    'employeeId' => employeeId,
    'wellnessServiceId' => wellnessServiceId,
    'slotDate' => slotDate,
    'startTime' => startTime,
    'endTime' => endTime,
    'isActive' => isActive,
    _ => null,
  };
}

class TimeSlotInsertDto implements AdminWriteDto {
  const TimeSlotInsertDto({
    required this.employeeId,
    required this.wellnessServiceId,
    required this.slotDate,
    required this.startTime,
    required this.endTime,
    required this.isActive,
  });

  final int employeeId;
  final int wellnessServiceId;
  final DateTime slotDate;
  final String startTime;
  final String endTime;
  final bool isActive;

  @override
  JsonMap toJson() => {
    'employeeId': employeeId,
    'wellnessServiceId': wellnessServiceId,
    'slotDate': slotDate.toIso8601String(),
    'startTime': startTime,
    'endTime': endTime,
    'isActive': isActive,
  };
}

class TimeSlotUpdateDto extends TimeSlotInsertDto {
  const TimeSlotUpdateDto({
    required this.id,
    required super.employeeId,
    required super.wellnessServiceId,
    required super.slotDate,
    required super.startTime,
    required super.endTime,
    required super.isActive,
  });

  final int id;

  @override
  JsonMap toJson() => {...super.toJson(), 'id': id};
}
