class Recommendation {
  Recommendation({
    required this.id,
    required this.name,
    required this.type,
    required this.score,
    required this.reason,
  });

  final int id;
  final String name;
  final String type;
  final double score;
  final String reason;

  factory Recommendation.fromJson(Map<String, dynamic> json) {
    return Recommendation(
      id: json['id'] as int? ?? 0,
      name: json['name'] as String? ?? '',
      type: json['type'] as String? ?? '',
      score: (json['score'] as num?)?.toDouble() ?? 0,
      reason: json['reason'] as String? ?? '',
    );
  }
}
