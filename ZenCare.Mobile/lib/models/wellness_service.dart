class WellnessService {
  WellnessService({
    required this.id,
    required this.name,
    this.description,
    required this.durationMinutes,
    required this.price,
    required this.serviceCategoryId,
    required this.serviceCategoryName,
    required this.isActive,
  });

  final int id;
  final String name;
  final String? description;
  final int durationMinutes;
  final double price;
  final int serviceCategoryId;
  final String serviceCategoryName;
  final bool isActive;

  factory WellnessService.fromJson(Map<String, dynamic> json) {
    return WellnessService(
      id: json['id'] as int? ?? 0,
      name: json['name'] as String? ?? '',
      description: json['description'] as String?,
      durationMinutes: json['durationMinutes'] as int? ?? 0,
      price: (json['price'] as num?)?.toDouble() ?? 0,
      serviceCategoryId: json['serviceCategoryId'] as int? ?? 0,
      serviceCategoryName: json['serviceCategoryName'] as String? ?? '',
      isActive: json['isActive'] as bool? ?? false,
    );
  }
}
