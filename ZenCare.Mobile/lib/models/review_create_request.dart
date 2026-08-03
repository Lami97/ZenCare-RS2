class ReviewCreateRequest {
  ReviewCreateRequest({
    this.appointmentId,
    this.productId,
    required this.rating,
    this.comment,
  });

  final int? appointmentId;
  final int? productId;
  final int rating;
  final String? comment;

  Map<String, dynamic> toJson() {
    return {
      'userId': 0,
      'appointmentId': appointmentId,
      'productId': productId,
      'rating': rating,
      'comment': comment,
      'status': 1,
    };
  }
}