class Cart {
  Cart({
    required this.id,
    required this.userId,
    required this.userName,
    required this.createdAt,
    this.updatedAt,
  });

  final int id;
  final int userId;
  final String userName;
  final DateTime createdAt;
  final DateTime? updatedAt;

  factory Cart.fromJson(Map<String, dynamic> json) {
    return Cart(
      id: json['id'] as int? ?? 0,
      userId: json['userId'] as int? ?? 0,
      userName: json['userName'] as String? ?? '',
      createdAt: DateTime.tryParse(json['createdAt']?.toString() ?? '') ?? DateTime.fromMillisecondsSinceEpoch(0),
      updatedAt: json['updatedAt'] == null ? null : DateTime.tryParse(json['updatedAt'].toString()),
    );
  }
}