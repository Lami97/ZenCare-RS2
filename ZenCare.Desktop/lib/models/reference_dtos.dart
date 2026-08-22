import 'admin_models.dart';

abstract class NamedReferenceDto implements AdminEntity {
  const NamedReferenceDto({
    required this.id,
    required this.name,
    required this.description,
    required this.isActive,
    required this.createdAt,
    required this.updatedAt,
  });

  @override
  final int id;
  final String name;
  final String? description;
  final bool isActive;
  final DateTime createdAt;
  final DateTime? updatedAt;

  @override
  Object? formValue(String key) => switch (key) {
    'name' => name,
    'description' => description,
    'isActive' => isActive,
    _ => null,
  };
}

typedef _ReferenceValues = ({
  int id,
  String name,
  String? description,
  bool isActive,
  DateTime createdAt,
  DateTime? updatedAt,
});

_ReferenceValues _values(JsonMap json) => (
  id: jsonInt(json['id']),
  name: json['name']?.toString() ?? '',
  description: json['description']?.toString(),
  isActive: jsonBool(json['isActive']),
  createdAt:
      jsonDateTime(json['createdAt']) ??
      DateTime.fromMillisecondsSinceEpoch(0, isUtc: true),
  updatedAt: jsonDateTime(json['updatedAt']),
);

class ProductCategoryDto extends NamedReferenceDto {
  ProductCategoryDto._(_ReferenceValues values)
    : super(
        id: values.id,
        name: values.name,
        description: values.description,
        isActive: values.isActive,
        createdAt: values.createdAt,
        updatedAt: values.updatedAt,
      );

  factory ProductCategoryDto.fromJson(JsonMap json) =>
      ProductCategoryDto._(_values(json));
}

class ProductTypeDto extends NamedReferenceDto {
  ProductTypeDto._(_ReferenceValues values)
    : super(
        id: values.id,
        name: values.name,
        description: values.description,
        isActive: values.isActive,
        createdAt: values.createdAt,
        updatedAt: values.updatedAt,
      );

  factory ProductTypeDto.fromJson(JsonMap json) =>
      ProductTypeDto._(_values(json));
}

class UnitOfMeasureDto extends NamedReferenceDto {
  UnitOfMeasureDto._(_ReferenceValues values)
    : super(
        id: values.id,
        name: values.name,
        description: values.description,
        isActive: values.isActive,
        createdAt: values.createdAt,
        updatedAt: values.updatedAt,
      );

  factory UnitOfMeasureDto.fromJson(JsonMap json) =>
      UnitOfMeasureDto._(_values(json));
}

class ServiceCategoryDto extends NamedReferenceDto {
  ServiceCategoryDto._(_ReferenceValues values)
    : super(
        id: values.id,
        name: values.name,
        description: values.description,
        isActive: values.isActive,
        createdAt: values.createdAt,
        updatedAt: values.updatedAt,
      );

  factory ServiceCategoryDto.fromJson(JsonMap json) =>
      ServiceCategoryDto._(_values(json));
}

class FaqCategoryDto extends NamedReferenceDto {
  FaqCategoryDto._(_ReferenceValues values)
    : super(
        id: values.id,
        name: values.name,
        description: values.description,
        isActive: values.isActive,
        createdAt: values.createdAt,
        updatedAt: values.updatedAt,
      );

  factory FaqCategoryDto.fromJson(JsonMap json) =>
      FaqCategoryDto._(_values(json));
}

abstract class NamedReferenceInsertDto implements AdminWriteDto {
  const NamedReferenceInsertDto({
    required this.name,
    this.description,
    required this.isActive,
  });

  final String name;
  final String? description;
  final bool isActive;

  @override
  JsonMap toJson() => {
    'name': name,
    'description': description,
    'isActive': isActive,
  };
}

abstract class NamedReferenceUpdateDto extends NamedReferenceInsertDto {
  const NamedReferenceUpdateDto({
    required this.id,
    required super.name,
    super.description,
    required super.isActive,
  });

  final int id;

  @override
  JsonMap toJson() => {...super.toJson(), 'id': id};
}

class ProductCategoryInsertDto extends NamedReferenceInsertDto {
  const ProductCategoryInsertDto({
    required super.name,
    super.description,
    required super.isActive,
  });
}

class ProductCategoryUpdateDto extends NamedReferenceUpdateDto {
  const ProductCategoryUpdateDto({
    required super.id,
    required super.name,
    super.description,
    required super.isActive,
  });
}

class ProductTypeInsertDto extends NamedReferenceInsertDto {
  const ProductTypeInsertDto({
    required super.name,
    super.description,
    required super.isActive,
  });
}

class ProductTypeUpdateDto extends NamedReferenceUpdateDto {
  const ProductTypeUpdateDto({
    required super.id,
    required super.name,
    super.description,
    required super.isActive,
  });
}

class UnitOfMeasureInsertDto extends NamedReferenceInsertDto {
  const UnitOfMeasureInsertDto({
    required super.name,
    super.description,
    required super.isActive,
  });
}

class UnitOfMeasureUpdateDto extends NamedReferenceUpdateDto {
  const UnitOfMeasureUpdateDto({
    required super.id,
    required super.name,
    super.description,
    required super.isActive,
  });
}

class ServiceCategoryInsertDto extends NamedReferenceInsertDto {
  const ServiceCategoryInsertDto({
    required super.name,
    super.description,
    required super.isActive,
  });
}

class ServiceCategoryUpdateDto extends NamedReferenceUpdateDto {
  const ServiceCategoryUpdateDto({
    required super.id,
    required super.name,
    super.description,
    required super.isActive,
  });
}

class FaqCategoryInsertDto extends NamedReferenceInsertDto {
  const FaqCategoryInsertDto({
    required super.name,
    super.description,
    required super.isActive,
  });
}

class FaqCategoryUpdateDto extends NamedReferenceUpdateDto {
  const FaqCategoryUpdateDto({
    required super.id,
    required super.name,
    super.description,
    required super.isActive,
  });
}

class RoleDto implements AdminEntity {
  const RoleDto({
    required this.id,
    required this.name,
    required this.roleType,
    required this.description,
    required this.isActive,
  });

  @override
  final int id;
  final String name;
  final int roleType;
  final String? description;
  final bool isActive;

  factory RoleDto.fromJson(JsonMap json) => RoleDto(
    id: jsonInt(json['id']),
    name: json['name']?.toString() ?? '',
    roleType: jsonInt(json['roleType']),
    description: json['description']?.toString(),
    isActive: jsonBool(json['isActive']),
  );

  @override
  Object? formValue(String key) => switch (key) {
    'name' => name,
    'roleType' => roleType,
    'description' => description,
    'isActive' => isActive,
    _ => null,
  };
}
