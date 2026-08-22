import 'admin_models.dart';

class SupplierDto implements AdminEntity {
  const SupplierDto({
    required this.id,
    required this.name,
    this.contactEmail,
    this.phoneNumber,
    this.address,
    required this.isActive,
    required this.createdAt,
    this.updatedAt,
  });
  @override
  final int id;
  final String name;
  final String? contactEmail;
  final String? phoneNumber;
  final String? address;
  final bool isActive;
  final DateTime createdAt;
  final DateTime? updatedAt;
  factory SupplierDto.fromJson(JsonMap json) => SupplierDto(
    id: jsonInt(json['id']),
    name: json['name']?.toString() ?? '',
    contactEmail: json['contactEmail']?.toString(),
    phoneNumber: json['phoneNumber']?.toString(),
    address: json['address']?.toString(),
    isActive: jsonBool(json['isActive']),
    createdAt:
        jsonDateTime(json['createdAt']) ??
        DateTime.fromMillisecondsSinceEpoch(0, isUtc: true),
    updatedAt: jsonDateTime(json['updatedAt']),
  );
  @override
  Object? formValue(String key) => switch (key) {
    'name' => name,
    'contactEmail' => contactEmail,
    'phoneNumber' => phoneNumber,
    'address' => address,
    'isActive' => isActive,
    _ => null,
  };
}

class SupplierInsertDto implements AdminWriteDto {
  const SupplierInsertDto({
    required this.name,
    this.contactEmail,
    this.phoneNumber,
    this.address,
    required this.isActive,
  });
  final String name;
  final String? contactEmail;
  final String? phoneNumber;
  final String? address;
  final bool isActive;
  @override
  JsonMap toJson() => {
    'name': name,
    'contactEmail': contactEmail,
    'phoneNumber': phoneNumber,
    'address': address,
    'isActive': isActive,
  };
}

class SupplierUpdateDto extends SupplierInsertDto {
  const SupplierUpdateDto({
    required this.id,
    required super.name,
    super.contactEmail,
    super.phoneNumber,
    super.address,
    required super.isActive,
  });
  final int id;
  @override
  JsonMap toJson() => {...super.toJson(), 'id': id};
}
