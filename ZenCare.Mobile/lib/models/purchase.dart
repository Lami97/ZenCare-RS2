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
  final PurchaseStatus status;
  final PaymentStatus paymentStatus;
  final double totalAmount;
  final String? stripePaymentIntentId;
  final DateTime? paidAt;
  final DateTime createdAt;
  final DateTime? updatedAt;
  final List<PurchaseItem> purchaseItems;

  String get displayNumber =>
      purchaseNumber.isNotEmpty ? purchaseNumber : '#$id';
  String get statusText => status.label;
  String get paymentStatusText => paymentStatus.label;
  bool get canPay =>
      status == PurchaseStatus.pendingPayment &&
      paymentStatus == PaymentStatus.pending;
  bool get canCancel =>
      status == PurchaseStatus.pendingPayment &&
      paymentStatus == PaymentStatus.pending;
  bool get canRefund =>
      status == PurchaseStatus.paid && paymentStatus == PaymentStatus.succeeded;

  factory Purchase.fromJson(Map<String, dynamic> json) {
    return Purchase(
      id: json['id'] as int? ?? json['purchaseId'] as int? ?? 0,
      purchaseId: json['purchaseId'] as int? ?? json['id'] as int? ?? 0,
      userId: json['userId'] as int? ?? 0,
      userName: json['userName'] as String? ?? '',
      purchaseNumber: json['purchaseNumber'] as String? ?? '',
      status: PurchaseStatus.fromValue(json['status'] as int? ?? 0),
      paymentStatus:
          PaymentStatus.fromValue(json['paymentStatus'] as int? ?? 0),
      totalAmount: (json['totalAmount'] as num?)?.toDouble() ?? 0,
      stripePaymentIntentId: json['stripePaymentIntentId'] as String?,
      paidAt: json['paidAt'] == null
          ? null
          : DateTime.tryParse(json['paidAt'].toString()),
      createdAt: DateTime.tryParse(json['createdAt']?.toString() ?? '') ??
          DateTime.fromMillisecondsSinceEpoch(0),
      updatedAt: json['updatedAt'] == null
          ? null
          : DateTime.tryParse(json['updatedAt'].toString()),
      purchaseItems: (json['purchaseItems'] as List<dynamic>? ?? [])
          .map((item) => PurchaseItem.fromJson(item as Map<String, dynamic>))
          .toList(),
    );
  }
}

enum PurchaseStatus {
  unknown(0, 'Unknown'),
  draft(1, 'Draft'),
  pendingPayment(2, 'Pending payment'),
  paid(3, 'Paid'),
  processing(4, 'Processing'),
  readyForPickup(5, 'Ready for pickup'),
  shipped(6, 'Shipped'),
  completed(7, 'Completed'),
  cancelled(8, 'Cancelled'),
  refunded(9, 'Refunded'),
  failed(10, 'Failed');

  const PurchaseStatus(this.value, this.label);

  final int value;
  final String label;

  static PurchaseStatus fromValue(int value) {
    return PurchaseStatus.values.firstWhere(
      (status) => status.value == value,
      orElse: () => PurchaseStatus.unknown,
    );
  }
}

enum PaymentStatus {
  unknown(0, 'Unknown'),
  pending(1, 'Pending'),
  succeeded(2, 'Succeeded'),
  failed(3, 'Failed'),
  cancelled(4, 'Cancelled'),
  refunded(5, 'Refunded');

  const PaymentStatus(this.value, this.label);

  final int value;
  final String label;

  static PaymentStatus fromValue(int value) {
    return PaymentStatus.values.firstWhere(
      (status) => status.value == value,
      orElse: () => PaymentStatus.unknown,
    );
  }
}
