import 'purchase_item.dart';

class Purchase {
  Purchase({
    required this.id,
    required this.purchaseId,
    required this.userId,
    required this.userName,
    required this.purchaseNumber,
    required this.status,
    required this.paymentStatus,
    required this.totalAmount,
    this.stripePaymentIntentId,
    this.paidAt,
    required this.createdAt,
    this.updatedAt,
    required this.purchaseItems,
  });

  final int id;
  final int purchaseId;
  final int userId;
  final String userName;
  final String purchaseNumber;
  final int status;
  final int paymentStatus;
  final double totalAmount;
  final String? stripePaymentIntentId;
  final DateTime? paidAt;
  final DateTime createdAt;
  final DateTime? updatedAt;
  final List<PurchaseItem> purchaseItems;

  String get displayNumber => purchaseNumber.isNotEmpty ? purchaseNumber : '#$id';
  String get statusText => _purchaseStatusName(status);
  String get paymentStatusText => _paymentStatusName(paymentStatus);
  bool get canPay => status == 2 && paymentStatus == 1;
  bool get canRefund => status == 3 && paymentStatus == 2;

  factory Purchase.fromJson(Map<String, dynamic> json) {
    return Purchase(
      id: json['id'] as int? ?? json['purchaseId'] as int? ?? 0,
      purchaseId: json['purchaseId'] as int? ?? json['id'] as int? ?? 0,
      userId: json['userId'] as int? ?? 0,
      userName: json['userName'] as String? ?? '',
      purchaseNumber: json['purchaseNumber'] as String? ?? '',
      status: json['status'] as int? ?? 0,
      paymentStatus: json['paymentStatus'] as int? ?? 0,
      totalAmount: (json['totalAmount'] as num?)?.toDouble() ?? 0,
      stripePaymentIntentId: json['stripePaymentIntentId'] as String?,
      paidAt: json['paidAt'] == null ? null : DateTime.tryParse(json['paidAt'].toString()),
      createdAt: DateTime.tryParse(json['createdAt']?.toString() ?? '') ?? DateTime.fromMillisecondsSinceEpoch(0),
      updatedAt: json['updatedAt'] == null ? null : DateTime.tryParse(json['updatedAt'].toString()),
      purchaseItems: (json['purchaseItems'] as List<dynamic>? ?? [])
          .map((item) => PurchaseItem.fromJson(item as Map<String, dynamic>))
          .toList(),
    );
  }
}

String _purchaseStatusName(int value) {
  return switch (value) {
    1 => 'Draft',
    2 => 'Pending payment',
    3 => 'Paid',
    4 => 'Processing',
    5 => 'Ready for pickup',
    6 => 'Shipped',
    7 => 'Completed',
    8 => 'Cancelled',
    9 => 'Refunded',
    10 => 'Failed',
    _ => 'Unknown',
  };
}

String _paymentStatusName(int value) {
  return switch (value) {
    1 => 'Pending',
    2 => 'Succeeded',
    3 => 'Failed',
    4 => 'Cancelled',
    5 => 'Refunded',
    _ => 'Unknown',
  };
}
