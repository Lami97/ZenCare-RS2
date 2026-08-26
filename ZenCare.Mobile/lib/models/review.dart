class Review {
  Review({
    required this.id,
    required this.userId,
    required this.userName,
    this.appointmentId,
    this.productId,
    this.productName,
    required this.rating,
    this.comment,
    required this.status,
    required this.createdAt,
    this.updatedAt,
  });

  final int id;
  final int userId;
  final String userName;
  final int? appointmentId;
  final int? productId;
  final String? productName;
  final int rating;
  final String? comment;
  final ReviewStatus status;
  final DateTime createdAt;
  final DateTime? updatedAt;

  String get statusText => status.label;

  String get targetName {
    if (productName != null && productName!.trim().isNotEmpty) {
      return productName!.trim();
    }

    if (appointmentId != null) {
      return 'Appointment #$appointmentId';
    }

    return 'Review #$id';
  }

  factory Review.fromJson(Map<String, dynamic> json) {
    return Review(
      id: json['id'] as int? ?? 0,
      userId: json['userId'] as int? ?? 0,
      userName: json['userName'] as String? ?? '',
      appointmentId: json['appointmentId'] as int?,
      productId: json['productId'] as int?,
      productName: json['productName'] as String?,
      rating: json['rating'] as int? ?? 0,
      comment: json['comment'] as String?,
      status: ReviewStatus.fromValue(
        json['status'] as int? ?? ReviewStatus.pendingApproval.value,
      ),
      createdAt: DateTime.tryParse(json['createdAt']?.toString() ?? '') ?? DateTime.fromMillisecondsSinceEpoch(0),
      updatedAt: json['updatedAt'] == null ? null : DateTime.tryParse(json['updatedAt'].toString()),
    );
  }
}

enum ReviewStatus {
  unknown(0, 'Unknown'),
  pendingApproval(1, 'Pending approval'),
  approved(2, 'Approved'),
  rejected(3, 'Rejected');

  const ReviewStatus(this.value, this.label);

  final int value;
  final String label;

  static ReviewStatus fromValue(int value) {
    return ReviewStatus.values.firstWhere(
      (status) => status.value == value,
      orElse: () => ReviewStatus.unknown,
    );
  }
}
