class Category {
  Category({
    required this.id,
    required this.name,
    this.description,
    required this.isActive,
  });

  final int id;
  final String name;
  final String? description;
  final bool isActive;

  factory Category.fromJson(Map<String, dynamic> json) {
    return Category(
      id: json['id'] as int,
      name: json['name'] as String? ?? '',
      description: json['description'] as String?,
      isActive: json['isActive'] as bool? ?? false,
    );
  }
}
