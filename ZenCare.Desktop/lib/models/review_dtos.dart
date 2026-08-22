import 'admin_models.dart';

class ReviewDto implements AdminEntity {
  const ReviewDto({
    required this.id,
    required this.userId,
    required this.userName,
    this.appointmentId,
    this.productId,
    this.productName,
    this.serviceName,
    required this.rating,
    this.comment,
    required this.status,
    required this.createdAt,
    this.updatedAt,
  });
  @override
  final int id;
  final int userId;
  final String userName;
  final int? appointmentId;
  final int? productId;
  final String? productName;
  final String? serviceName;
  final int rating;
  final String? comment;
  final int status;
  final DateTime createdAt;
  final DateTime? updatedAt;
  String? get targetName => productName ?? serviceName;
  factory ReviewDto.fromJson(JsonMap json) => ReviewDto(
    id: jsonInt(json['id']),
    userId: jsonInt(json['userId']),
    userName: json['userName']?.toString() ?? '',
    appointmentId: json['appointmentId'] == null
        ? null
        : jsonInt(json['appointmentId']),
    productId: json['productId'] == null ? null : jsonInt(json['productId']),
    productName: json['productName']?.toString(),
    serviceName: json['serviceName']?.toString(),
    rating: jsonInt(json['rating']),
    comment: json['comment']?.toString(),
    status: jsonInt(json['status']),
    createdAt:
        jsonDateTime(json['createdAt']) ??
        DateTime.fromMillisecondsSinceEpoch(0, isUtc: true),
    updatedAt: jsonDateTime(json['updatedAt']),
  );
  @override
  Object? formValue(String key) => switch (key) {
    'userId' => userId,
    'appointmentId' => appointmentId,
    'productId' => productId,
    'rating' => rating,
    'comment' => comment,
    'status' => status,
    _ => null,
  };
}

class ReviewInsertDto implements AdminWriteDto {
  const ReviewInsertDto({
    required this.userId,
    this.appointmentId,
    this.productId,
    required this.rating,
    this.comment,
    required this.status,
  });
  final int userId;
  final int? appointmentId;
  final int? productId;
  final int rating;
  final String? comment;
  final int status;
  @override
  JsonMap toJson() => {
    'userId': userId,
    'appointmentId': appointmentId,
    'productId': productId,
    'rating': rating,
    'comment': comment,
    'status': status,
  };
}

class ReviewUpdateDto extends ReviewInsertDto {
  const ReviewUpdateDto({
    required this.id,
    required super.userId,
    super.appointmentId,
    super.productId,
    required super.rating,
    super.comment,
    required super.status,
  });
  final int id;
  @override
  JsonMap toJson() => {...super.toJson(), 'id': id};
}
