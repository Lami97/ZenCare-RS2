class UserNotification {
  UserNotification({
    required this.id,
    required this.userId,
    required this.title,
    required this.message,
    this.notificationType,
    required this.status,
    required this.isRead,
    this.sentAt,
    required this.createdAt,
    this.updatedAt,
  });

  final int id;
  final int userId;
  final String title;
  final String message;
  final String? notificationType;
  final int status;
  final bool isRead;
  final DateTime? sentAt;
  final DateTime createdAt;
  final DateTime? updatedAt;

  factory UserNotification.fromJson(Map<String, dynamic> json) {
    return UserNotification(
      id: json['id'] as int? ?? 0,
      userId: json['userId'] as int? ?? 0,
      title: json['title'] as String? ?? '',
      message: json['message'] as String? ?? '',
      notificationType: json['notificationType'] as String?,
      status: json['status'] as int? ?? 0,
      isRead: json['isRead'] as bool? ?? false,
      sentAt: _parseDate(json['sentAt']),
      createdAt: _parseDate(json['createdAt']) ?? DateTime.fromMillisecondsSinceEpoch(0),
      updatedAt: _parseDate(json['updatedAt']),
    );
  }
}

DateTime? _parseDate(dynamic value) {
  if (value == null) {
    return null;
  }

  final text = value.toString();
  final parsed = DateTime.tryParse(text);

  if (parsed == null || parsed.isUtc || _hasExplicitUtcOffset(text)) {
    return parsed;
  }

  return DateTime.utc(
    parsed.year,
    parsed.month,
    parsed.day,
    parsed.hour,
    parsed.minute,
    parsed.second,
    parsed.millisecond,
    parsed.microsecond,
  );
}

bool _hasExplicitUtcOffset(String value) {
  return RegExp(r'(Z|[+-]\d{2}:?\d{2})$', caseSensitive: false).hasMatch(value);
}
