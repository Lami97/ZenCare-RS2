import 'admin_models.dart';

class UserDto implements AdminEntity {
  const UserDto({
    required this.id,
    required this.firstName,
    required this.lastName,
    required this.email,
    required this.username,
    this.phoneNumber,
    required this.isActive,
    required this.createdAt,
    this.updatedAt,
    this.lastLoginAt,
  });
  @override
  final int id;
  final String firstName;
  final String lastName;
  final String email;
  final String username;
  final String? phoneNumber;
  final bool isActive;
  final DateTime createdAt;
  final DateTime? updatedAt;
  final DateTime? lastLoginAt;
  String get fullName => '$firstName $lastName'.trim();

  factory UserDto.fromJson(JsonMap json) => UserDto(
    id: jsonInt(json['id']),
    firstName: json['firstName']?.toString() ?? '',
    lastName: json['lastName']?.toString() ?? '',
    email: json['email']?.toString() ?? '',
    username: json['username']?.toString() ?? '',
    phoneNumber: json['phoneNumber']?.toString(),
    isActive: jsonBool(json['isActive']),
    createdAt:
        jsonDateTime(json['createdAt']) ??
        DateTime.fromMillisecondsSinceEpoch(0, isUtc: true),
    updatedAt: jsonDateTime(json['updatedAt']),
    lastLoginAt: jsonDateTime(json['lastLoginAt']),
  );

  @override
  Object? formValue(String key) => switch (key) {
    'firstName' => firstName,
    'lastName' => lastName,
    'email' => email,
    'username' => username,
    'phoneNumber' => phoneNumber,
    'isActive' => isActive,
    _ => null,
  };
}

class UserInsertDto implements AdminWriteDto {
  const UserInsertDto({
    required this.firstName,
    required this.lastName,
    required this.email,
    required this.username,
    required this.password,
    required this.passwordConfirm,
    this.phoneNumber,
    required this.isActive,
  });
  final String firstName;
  final String lastName;
  final String email;
  final String username;
  final String password;
  final String passwordConfirm;
  final String? phoneNumber;
  final bool isActive;
  @override
  JsonMap toJson() => {
    'firstName': firstName,
    'lastName': lastName,
    'email': email,
    'username': username,
    'password': password,
    'passwordConfirm': passwordConfirm,
    'phoneNumber': phoneNumber,
    'isActive': isActive,
  };
}

class UserUpdateDto implements AdminWriteDto {
  const UserUpdateDto({
    required this.id,
    required this.firstName,
    required this.lastName,
    required this.email,
    required this.username,
    this.phoneNumber,
    required this.isActive,
  });
  final int id;
  final String firstName;
  final String lastName;
  final String email;
  final String username;
  final String? phoneNumber;
  final bool isActive;
  @override
  JsonMap toJson() => {
    'id': id,
    'firstName': firstName,
    'lastName': lastName,
    'email': email,
    'username': username,
    'phoneNumber': phoneNumber,
    'isActive': isActive,
  };
}

class UserRoleDto implements AdminEntity {
  const UserRoleDto({
    required this.id,
    required this.userId,
    required this.userName,
    required this.roleId,
    required this.roleName,
    required this.createdAt,
  });
  @override
  final int id;
  final int userId;
  final String userName;
  final int roleId;
  final String roleName;
  final DateTime createdAt;
  factory UserRoleDto.fromJson(JsonMap json) => UserRoleDto(
    id: jsonInt(json['id']),
    userId: jsonInt(json['userId']),
    userName: json['userName']?.toString() ?? '',
    roleId: jsonInt(json['roleId']),
    roleName: json['roleName']?.toString() ?? '',
    createdAt:
        jsonDateTime(json['createdAt']) ??
        DateTime.fromMillisecondsSinceEpoch(0, isUtc: true),
  );
  @override
  Object? formValue(String key) => switch (key) {
    'userId' => userId,
    'roleId' => roleId,
    _ => null,
  };
}

class UserRoleInsertDto implements AdminWriteDto {
  const UserRoleInsertDto({required this.userId, required this.roleId});
  final int userId;
  final int roleId;
  @override
  JsonMap toJson() => {'userId': userId, 'roleId': roleId};
}

class UserRoleUpdateDto extends UserRoleInsertDto {
  const UserRoleUpdateDto({
    required this.id,
    required super.userId,
    required super.roleId,
  });
  final int id;
  @override
  JsonMap toJson() => {...super.toJson(), 'id': id};
}
