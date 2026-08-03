class CartItem {
  CartItem({
    required this.id,
    required this.cartId,
    required this.userName,
    required this.productId,
    required this.productName,
    required this.quantity,
    required this.unitPrice,
    required this.createdAt,
    this.updatedAt,
  });

  final int id;
  final int cartId;
  final String userName;
  final int productId;
  final String productName;
  final int quantity;
  final double unitPrice;
  final DateTime createdAt;
  final DateTime? updatedAt;

  double get subtotal => quantity * unitPrice;

  factory CartItem.fromJson(Map<String, dynamic> json) {
    return CartItem(
      id: json['id'] as int? ?? 0,
      cartId: json['cartId'] as int? ?? 0,
      userName: json['userName'] as String? ?? '',
      productId: json['productId'] as int? ?? 0,
      productName: json['productName'] as String? ?? '',
      quantity: json['quantity'] as int? ?? 0,
      unitPrice: (json['unitPrice'] as num?)?.toDouble() ?? 0,
      createdAt: DateTime.tryParse(json['createdAt']?.toString() ?? '') ?? DateTime.fromMillisecondsSinceEpoch(0),
      updatedAt: json['updatedAt'] == null ? null : DateTime.tryParse(json['updatedAt'].toString()),
    );
  }
}