class Employee {
  Employee({
    required this.id,
    required this.userId,
    required this.userName,
    this.specialization,
    this.bio,
    this.hireDate,
    required this.isAvailable,
  });

  final int id;
  final int userId;
  final String userName;
  final String? specialization;
  final String? bio;
  final DateTime? hireDate;
  final bool isAvailable;

  factory Employee.fromJson(Map<String, dynamic> json) {
    return Employee(
      id: json['id'] as int? ?? 0,
      userId: json['userId'] as int? ?? 0,
      userName: json['userName'] as String? ?? '',
      specialization: json['specialization'] as String?,
      bio: json['bio'] as String?,
      hireDate: json['hireDate'] == null ? null : DateTime.parse(json['hireDate'] as String),
      isAvailable: json['isAvailable'] as bool? ?? false,
    );
  }

  String get displayName {
    if ((specialization ?? '').trim().isEmpty) {
      return userName;
    }

    return '$userName - $specialization';
  }
}
