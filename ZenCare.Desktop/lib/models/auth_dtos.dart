import 'admin_models.dart';

class LoginRequestDto implements AdminWriteDto {
  const LoginRequestDto({required this.username, required this.password});
  final String username;
  final String password;
  @override
  JsonMap toJson() => {'username': username, 'password': password};
}

class AuthenticatedUser {
  const AuthenticatedUser({
    required this.userId,
    required this.firstName,
    required this.lastName,
    required this.username,
    required this.email,
    required this.fullName,
    this.phoneNumber,
    required this.isActive,
    required this.roles,
  });
  final int userId;
  final String firstName;
  final String lastName;
  final String username;
  final String email;
  final String fullName;
  final String? phoneNumber;
  final bool isActive;
  final List<String> roles;
  factory AuthenticatedUser.fromJson(JsonMap json) => AuthenticatedUser(
    userId: jsonInt(json['userId']),
    firstName: json['firstName']?.toString() ?? '',
    lastName: json['lastName']?.toString() ?? '',
    username: json['username']?.toString() ?? '',
    email: json['email']?.toString() ?? '',
    fullName: json['fullName']?.toString() ?? '',
    phoneNumber: json['phoneNumber']?.toString(),
    isActive: jsonBool(json['isActive']),
    roles: (json['roles'] is List ? json['roles'] as List : const <Object?>[])
        .map((role) => role.toString())
        .toList(),
  );
  JsonMap toJson() => {
    'userId': userId,
    'firstName': firstName,
    'lastName': lastName,
    'username': username,
    'email': email,
    'fullName': fullName,
    'phoneNumber': phoneNumber,
    'isActive': isActive,
    'roles': roles,
  };
}

class LoginResponseDto {
  const LoginResponseDto({
    required this.user,
    required this.token,
    required this.expiresAt,
  });
  final AuthenticatedUser user;
  final String token;
  final DateTime expiresAt;
  factory LoginResponseDto.fromJson(JsonMap json) {
    final expiresAt = jsonDateTime(json['expiresAt']);
    if (expiresAt == null) {
      throw const FormatException(
        'Login response did not contain a valid expiry.',
      );
    }
    return LoginResponseDto(
      user: AuthenticatedUser.fromJson(json),
      token: json['token']?.toString() ?? '',
      expiresAt: expiresAt,
    );
  }
}
