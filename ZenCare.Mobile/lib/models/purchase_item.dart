class PurchaseItem {
  PurchaseItem({
    required this.id,
    required this.purchaseId,
    required this.purchaseNumber,
    required this.productId,
    required this.productName,
    required this.quantity,
    required this.unitPrice,
    required this.totalPrice,
  });

  final int id;
  final int purchaseId;
  final String purchaseNumber;
  final int productId;
  final String productName;
  final int quantity;
  final double unitPrice;
  final double totalPrice;

  factory PurchaseItem.fromJson(Map<String, dynamic> json) {
    return PurchaseItem(
      id: json['id'] as int? ?? 0,
      purchaseId: json['purchaseId'] as int? ?? 0,
      purchaseNumber: json['purchaseNumber'] as String? ?? '',
      productId: json['productId'] as int? ?? 0,
      productName: json['productName'] as String? ?? '',
      quantity: json['quantity'] as int? ?? 0,
      unitPrice: (json['unitPrice'] as num?)?.toDouble() ?? 0,
      totalPrice: (json['totalPrice'] as num?)?.toDouble() ?? 0,
    );
  }
}