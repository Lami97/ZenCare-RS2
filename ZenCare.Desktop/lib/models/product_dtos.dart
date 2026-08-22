import 'admin_models.dart';

class ProductDto implements AdminEntity {
  const ProductDto({
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
    required this.supplierId,
    required this.supplierName,
    required this.isActive,
    required this.createdAt,
    this.updatedAt,
  });

  @override
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
  final int supplierId;
  final String supplierName;
  final bool isActive;
  final DateTime createdAt;
  final DateTime? updatedAt;

  factory ProductDto.fromJson(JsonMap json) => ProductDto(
    id: jsonInt(json['id']),
    name: json['name']?.toString() ?? '',
    description: json['description']?.toString(),
    price: jsonDouble(json['price']),
    stockQuantity: jsonInt(json['stockQuantity']),
    productCategoryId: jsonInt(json['productCategoryId']),
    productCategoryName: json['productCategoryName']?.toString() ?? '',
    productTypeId: jsonInt(json['productTypeId']),
    productTypeName: json['productTypeName']?.toString() ?? '',
    unitOfMeasureId: jsonInt(json['unitOfMeasureId']),
    unitOfMeasureName: json['unitOfMeasureName']?.toString() ?? '',
    supplierId: jsonInt(json['supplierId']),
    supplierName: json['supplierName']?.toString() ?? '',
    isActive: jsonBool(json['isActive']),
    createdAt:
        jsonDateTime(json['createdAt']) ??
        DateTime.fromMillisecondsSinceEpoch(0, isUtc: true),
    updatedAt: jsonDateTime(json['updatedAt']),
  );

  @override
  Object? formValue(String key) => switch (key) {
    'name' => name,
    'description' => description,
    'price' => price,
    'stockQuantity' => stockQuantity,
    'productCategoryId' => productCategoryId,
    'productTypeId' => productTypeId,
    'unitOfMeasureId' => unitOfMeasureId,
    'supplierId' => supplierId,
    'isActive' => isActive,
    _ => null,
  };
}

class ProductInsertDto implements AdminWriteDto {
  const ProductInsertDto({
    required this.name,
    this.description,
    required this.price,
    required this.stockQuantity,
    required this.productCategoryId,
    required this.productTypeId,
    required this.unitOfMeasureId,
    required this.supplierId,
    required this.isActive,
  });
  final String name;
  final String? description;
  final double price;
  final int stockQuantity;
  final int productCategoryId;
  final int productTypeId;
  final int unitOfMeasureId;
  final int supplierId;
  final bool isActive;

  @override
  JsonMap toJson() => {
    'name': name,
    'description': description,
    'price': price,
    'stockQuantity': stockQuantity,
    'productCategoryId': productCategoryId,
    'productTypeId': productTypeId,
    'unitOfMeasureId': unitOfMeasureId,
    'supplierId': supplierId,
    'isActive': isActive,
  };
}

class ProductUpdateDto extends ProductInsertDto {
  const ProductUpdateDto({
    required this.id,
    required super.name,
    super.description,
    required super.price,
    required super.stockQuantity,
    required super.productCategoryId,
    required super.productTypeId,
    required super.unitOfMeasureId,
    required super.supplierId,
    required super.isActive,
  });
  final int id;

  @override
  JsonMap toJson() => {...super.toJson(), 'id': id};
}
