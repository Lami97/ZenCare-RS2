class PaymentStatusResponse {
  PaymentStatusResponse({
    required this.purchaseId,
    required this.paymentId,
    required this.purchaseStatus,
    required this.paymentStatus,
    this.paidAt,
    this.refundedAt,
  });

  final int purchaseId;
  final int paymentId;
  final int purchaseStatus;
  final int paymentStatus;
  final DateTime? paidAt;
  final DateTime? refundedAt;

  factory PaymentStatusResponse.fromJson(Map<String, dynamic> json) {
    return PaymentStatusResponse(
      purchaseId: json['purchaseId'] as int? ?? 0,
      paymentId: json['paymentId'] as int? ?? 0,
      purchaseStatus: json['purchaseStatus'] as int? ?? 0,
      paymentStatus: json['paymentStatus'] as int? ?? 0,
      paidAt: json['paidAt'] == null ? null : DateTime.tryParse(json['paidAt'].toString()),
      refundedAt: json['refundedAt'] == null ? null : DateTime.tryParse(json['refundedAt'].toString()),
    );
  }
}
