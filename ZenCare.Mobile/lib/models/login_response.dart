class LoginResponse {
  LoginResponse({
    required this.userId,
    required this.firstName,
    required this.lastName,
    required this.username,
    required this.email,
    required this.fullName,
    this.phoneNumber,
    required this.isActive,
    required this.token,
    required this.expiresAt,
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
  final String token;
  final DateTime expiresAt;
  final List<String> roles;

  factory LoginResponse.fromJson(Map<String, dynamic> json) {
    return LoginResponse(
      userId: json['userId'] as int,
      firstName: json['firstName'] as String? ?? '',
      lastName: json['lastName'] as String? ?? '',
      username: json['username'] as String? ?? '',
      email: json['email'] as String? ?? '',
      fullName: json['fullName'] as String? ?? '',
      phoneNumber: json['phoneNumber'] as String?,
      isActive: json['isActive'] as bool? ?? true,
      token: json['token'] as String? ?? '',
      expiresAt: DateTime.parse(json['expiresAt'] as String),
      roles: (json['roles'] as List<dynamic>? ?? [])
          .map((role) => role.toString())
          .toList(),
    );
  }
}
