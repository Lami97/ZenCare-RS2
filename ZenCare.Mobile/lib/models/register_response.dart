class RegisterResponse {
  RegisterResponse({
    required this.userId,
    required this.clientProfileId,
    required this.username,
    required this.email,
    required this.role,
    required this.isActive,
    required this.createdAt,
    required this.message,
  });

  final int userId;
  final int clientProfileId;
  final String username;
  final String email;
  final String role;
  final bool isActive;
  final DateTime createdAt;
  final String message;

  factory RegisterResponse.fromJson(Map<String, dynamic> json) {
    return RegisterResponse(
      userId: json['userId'] as int,
      clientProfileId: json['clientProfileId'] as int,
      username: json['username'] as String? ?? '',
      email: json['email'] as String? ?? '',
      role: json['role'] as String? ?? '',
      isActive: json['isActive'] as bool? ?? true,
      createdAt: DateTime.parse(json['createdAt'] as String),
      message: json['message'] as String? ?? 'Account created successfully. Please sign in.',
    );
  }
}
