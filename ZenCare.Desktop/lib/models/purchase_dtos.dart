import 'admin_models.dart';

class PurchaseItemDto implements AdminEntity {
  const PurchaseItemDto({
    required this.id,
    required this.purchaseId,
    required this.purchaseNumber,
    required this.productId,
    required this.productName,
    required this.quantity,
    required this.unitPrice,
    required this.totalPrice,
  });
  @override
  final int id;
  final int purchaseId;
  final String purchaseNumber;
  final int productId;
  final String productName;
  final int quantity;
  final double unitPrice;
  final double totalPrice;
  factory PurchaseItemDto.fromJson(JsonMap json) => PurchaseItemDto(
    id: jsonInt(json['id']),
    purchaseId: jsonInt(json['purchaseId']),
    purchaseNumber: json['purchaseNumber']?.toString() ?? '',
    productId: jsonInt(json['productId']),
    productName: json['productName']?.toString() ?? '',
    quantity: jsonInt(json['quantity']),
    unitPrice: jsonDouble(json['unitPrice']),
    totalPrice: jsonDouble(json['totalPrice']),
  );
  @override
  Object? formValue(String key) => switch (key) {
    'purchaseId' => purchaseId,
    'productId' => productId,
    'quantity' => quantity,
    'unitPrice' => unitPrice,
    'totalPrice' => totalPrice,
    _ => null,
  };
}

class PurchaseDto implements AdminEntity {
  const PurchaseDto({
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
  @override
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
  final List<PurchaseItemDto> purchaseItems;
  factory PurchaseDto.fromJson(JsonMap json) => PurchaseDto(
    id: jsonInt(json['id']),
    purchaseId: jsonInt(json['purchaseId']),
    userId: jsonInt(json['userId']),
    userName: json['userName']?.toString() ?? '',
    purchaseNumber: json['purchaseNumber']?.toString() ?? '',
    status: jsonInt(json['status']),
    paymentStatus: jsonInt(json['paymentStatus']),
    totalAmount: jsonDouble(json['totalAmount']),
    stripePaymentIntentId: json['stripePaymentIntentId']?.toString(),
    paidAt: jsonDateTime(json['paidAt']),
    createdAt:
        jsonDateTime(json['createdAt']) ??
        DateTime.fromMillisecondsSinceEpoch(0, isUtc: true),
    updatedAt: jsonDateTime(json['updatedAt']),
    purchaseItems:
        (json['purchaseItems'] is List
                ? json['purchaseItems'] as List
                : const <Object?>[])
            .whereType<Map>()
            .map((item) => PurchaseItemDto.fromJson(JsonMap.from(item)))
            .toList(),
  );
  @override
  Object? formValue(String key) => switch (key) {
    'userId' => userId,
    'purchaseNumber' => purchaseNumber,
    'totalAmount' => totalAmount,
    'status' => status,
    'paymentStatus' => paymentStatus,
    'paidAt' => paidAt,
    _ => null,
  };
}

class PurchaseUpdateDto implements AdminWriteDto {
  const PurchaseUpdateDto({
    required this.id,
    required this.userId,
    required this.purchaseNumber,
    required this.status,
    required this.paymentStatus,
    required this.totalAmount,
    this.stripePaymentIntentId,
    this.paidAt,
  });
  final int id;
  final int userId;
  final String purchaseNumber;
  final int status;
  final int paymentStatus;
  final double totalAmount;
  final String? stripePaymentIntentId;
  final DateTime? paidAt;
  @override
  JsonMap toJson() => {
    'id': id,
    'userId': userId,
    'purchaseNumber': purchaseNumber,
    'status': status,
    'paymentStatus': paymentStatus,
    'totalAmount': totalAmount,
    'stripePaymentIntentId': stripePaymentIntentId,
    'paidAt': paidAt?.toIso8601String(),
  };
}

class PurchaseItemInsertDto implements AdminWriteDto {
  const PurchaseItemInsertDto({
    required this.purchaseId,
    required this.productId,
    required this.quantity,
    required this.unitPrice,
    required this.totalPrice,
  });
  final int purchaseId;
  final int productId;
  final int quantity;
  final double unitPrice;
  final double totalPrice;
  @override
  JsonMap toJson() => {
    'purchaseId': purchaseId,
    'productId': productId,
    'quantity': quantity,
    'unitPrice': unitPrice,
    'totalPrice': totalPrice,
  };
}

class PurchaseItemUpdateDto extends PurchaseItemInsertDto {
  const PurchaseItemUpdateDto({
    required this.id,
    required super.purchaseId,
    required super.productId,
    required super.quantity,
    required super.unitPrice,
    required super.totalPrice,
  });
  final int id;
  @override
  JsonMap toJson() => {...super.toJson(), 'id': id};
}
