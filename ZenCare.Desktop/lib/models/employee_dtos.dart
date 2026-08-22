import 'admin_models.dart';

class EmployeeDto implements AdminEntity {
  const EmployeeDto({
    required this.id,
    required this.userId,
    required this.userName,
    required this.employeeName,
    this.specialization,
    this.bio,
    this.hireDate,
    required this.isAvailable,
    required this.createdAt,
    this.updatedAt,
  });
  @override
  final int id;
  final int userId;
  final String userName;
  final String employeeName;
  final String? specialization;
  final String? bio;
  final DateTime? hireDate;
  final bool isAvailable;
  final DateTime createdAt;
  final DateTime? updatedAt;
  factory EmployeeDto.fromJson(JsonMap json) => EmployeeDto(
    id: jsonInt(json['id']),
    userId: jsonInt(json['userId']),
    userName: json['userName']?.toString() ?? '',
    employeeName: json['employeeName']?.toString() ?? '',
    specialization: json['specialization']?.toString(),
    bio: json['bio']?.toString(),
    hireDate: jsonDateTime(json['hireDate']),
    isAvailable: jsonBool(json['isAvailable']),
    createdAt:
        jsonDateTime(json['createdAt']) ??
        DateTime.fromMillisecondsSinceEpoch(0, isUtc: true),
    updatedAt: jsonDateTime(json['updatedAt']),
  );
  @override
  Object? formValue(String key) => switch (key) {
    'userId' => userId,
    'specialization' => specialization,
    'bio' => bio,
    'hireDate' => hireDate,
    'isAvailable' => isAvailable,
    _ => null,
  };
}

class EmployeeInsertDto implements AdminWriteDto {
  const EmployeeInsertDto({
    required this.userId,
    this.specialization,
    this.bio,
    this.hireDate,
    required this.isAvailable,
  });
  final int userId;
  final String? specialization;
  final String? bio;
  final DateTime? hireDate;
  final bool isAvailable;
  @override
  JsonMap toJson() => {
    'userId': userId,
    'specialization': specialization,
    'bio': bio,
    'hireDate': hireDate?.toIso8601String(),
    'isAvailable': isAvailable,
  };
}

class EmployeeUpdateDto extends EmployeeInsertDto {
  const EmployeeUpdateDto({
    required this.id,
    required super.userId,
    super.specialization,
    super.bio,
    super.hireDate,
    required super.isAvailable,
  });
  final int id;
  @override
  JsonMap toJson() => {...super.toJson(), 'id': id};
}

class EmployeeServiceDto implements AdminEntity {
  const EmployeeServiceDto({
    required this.id,
    required this.employeeId,
    required this.employeeName,
    required this.wellnessServiceId,
    required this.serviceName,
    required this.isActive,
    required this.createdAt,
    this.updatedAt,
  });
  @override
  final int id;
  final int employeeId;
  final String employeeName;
  final int wellnessServiceId;
  final String serviceName;
  final bool isActive;
  final DateTime createdAt;
  final DateTime? updatedAt;
  factory EmployeeServiceDto.fromJson(JsonMap json) => EmployeeServiceDto(
    id: jsonInt(json['id']),
    employeeId: jsonInt(json['employeeId']),
    employeeName: json['employeeName']?.toString() ?? '',
    wellnessServiceId: jsonInt(json['wellnessServiceId']),
    serviceName: json['serviceName']?.toString() ?? '',
    isActive: jsonBool(json['isActive']),
    createdAt:
        jsonDateTime(json['createdAt']) ??
        DateTime.fromMillisecondsSinceEpoch(0, isUtc: true),
    updatedAt: jsonDateTime(json['updatedAt']),
  );
  @override
  Object? formValue(String key) => switch (key) {
    'employeeId' => employeeId,
    'wellnessServiceId' => wellnessServiceId,
    'isActive' => isActive,
    _ => null,
  };
}

class EmployeeServiceInsertDto implements AdminWriteDto {
  const EmployeeServiceInsertDto({
    required this.employeeId,
    required this.wellnessServiceId,
    required this.isActive,
  });
  final int employeeId;
  final int wellnessServiceId;
  final bool isActive;
  @override
  JsonMap toJson() => {
    'employeeId': employeeId,
    'wellnessServiceId': wellnessServiceId,
    'isActive': isActive,
  };
}

class EmployeeServiceUpdateDto extends EmployeeServiceInsertDto {
  const EmployeeServiceUpdateDto({
    required this.id,
    required super.employeeId,
    required super.wellnessServiceId,
    required super.isActive,
  });
  final int id;
  @override
  JsonMap toJson() => {...super.toJson(), 'id': id};
}
