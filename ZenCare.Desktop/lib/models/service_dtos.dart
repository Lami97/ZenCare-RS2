import 'admin_models.dart';

class WellnessServiceDto implements AdminEntity {
  const WellnessServiceDto({
    required this.id,
    required this.name,
    this.description,
    required this.durationMinutes,
    required this.price,
    required this.serviceCategoryId,
    required this.serviceCategoryName,
    required this.isActive,
    required this.createdAt,
    this.updatedAt,
  });
  @override
  final int id;
  final String name;
  final String? description;
  final int durationMinutes;
  final double price;
  final int serviceCategoryId;
  final String serviceCategoryName;
  final bool isActive;
  final DateTime createdAt;
  final DateTime? updatedAt;

  factory WellnessServiceDto.fromJson(JsonMap json) => WellnessServiceDto(
    id: jsonInt(json['id']),
    name: json['name']?.toString() ?? '',
    description: json['description']?.toString(),
    durationMinutes: jsonInt(json['durationMinutes']),
    price: jsonDouble(json['price']),
    serviceCategoryId: jsonInt(json['serviceCategoryId']),
    serviceCategoryName: json['serviceCategoryName']?.toString() ?? '',
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
    'durationMinutes' => durationMinutes,
    'price' => price,
    'serviceCategoryId' => serviceCategoryId,
    'isActive' => isActive,
    _ => null,
  };
}

class WellnessServiceInsertDto implements AdminWriteDto {
  const WellnessServiceInsertDto({
    required this.name,
    this.description,
    required this.durationMinutes,
    required this.price,
    required this.serviceCategoryId,
    required this.isActive,
  });
  final String name;
  final String? description;
  final int durationMinutes;
  final double price;
  final int serviceCategoryId;
  final bool isActive;
  @override
  JsonMap toJson() => {
    'name': name,
    'description': description,
    'durationMinutes': durationMinutes,
    'price': price,
    'serviceCategoryId': serviceCategoryId,
    'isActive': isActive,
  };
}

class WellnessServiceUpdateDto extends WellnessServiceInsertDto {
  const WellnessServiceUpdateDto({
    required this.id,
    required super.name,
    super.description,
    required super.durationMinutes,
    required super.price,
    required super.serviceCategoryId,
    required super.isActive,
  });
  final int id;
  @override
  JsonMap toJson() => {...super.toJson(), 'id': id};
}
