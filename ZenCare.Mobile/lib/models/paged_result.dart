class PagedResult<T> {
  PagedResult({
    required this.items,
    this.totalCount,
  });

  final List<T> items;
  final int? totalCount;

  factory PagedResult.fromJson(
    Map<String, dynamic> json,
    T Function(Map<String, dynamic>) fromJson,
  ) {
    return PagedResult<T>(
      items: (json['items'] as List<dynamic>? ?? [])
          .map((item) => fromJson(item as Map<String, dynamic>))
          .toList(),
      totalCount: json['totalCount'] as int?,
    );
  }
}
