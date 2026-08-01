class Product {
  Product({
    required this.id,
    required this.name,
    this.description,
    required this.price,
    required this.stockQuantity,
    required this.productCategoryId,
    required this.productCategoryName,
    required this.productTypeId,
    required this.productTypeName,
    required this.unitOfMeasureId,
    required this.unitOfMeasureName,
    required this.isActive,
  });

  final int id;
  final String name;
  final String? description;
  final double price;
  final int stockQuantity;
  final int productCategoryId;
  final String productCategoryName;
  final int productTypeId;
  final String productTypeName;
  final int unitOfMeasureId;
  final String unitOfMeasureName;
  final bool isActive;

  factory Product.fromJson(Map<String, dynamic> json) {
    return Product(
      id: json['id'] as int,
      name: json['name'] as String? ?? '',
      description: json['description'] as String?,
      price: (json['price'] as num?)?.toDouble() ?? 0,
      stockQuantity: json['stockQuantity'] as int? ?? 0,
      productCategoryId: json['productCategoryId'] as int? ?? 0,
      productCategoryName: json['productCategoryName'] as String? ?? '',
      productTypeId: json['productTypeId'] as int? ?? 0,
      productTypeName: json['productTypeName'] as String? ?? '',
      unitOfMeasureId: json['unitOfMeasureId'] as int? ?? 0,
      unitOfMeasureName: json['unitOfMeasureName'] as String? ?? '',
      isActive: json['isActive'] as bool? ?? false,
    );
  }
}
