class PagedResult {
  PagedResult({required this.items, this.totalCount});

  final List<Map<String, dynamic>> items;
  final int? totalCount;

  factory PagedResult.fromJson(dynamic data) {
    if (data is List) {
      return PagedResult(
        items: data
            .whereType<Map>()
            .map((e) => Map<String, dynamic>.from(e))
            .toList(),
      );
    }

    if (data is Map<String, dynamic>) {
      final rawItems = data['items'];
      return PagedResult(
        items: rawItems is List
            ? rawItems
                  .whereType<Map>()
                  .map((e) => Map<String, dynamic>.from(e))
                  .toList()
            : [],
        totalCount: data['totalCount'] is int
            ? data['totalCount'] as int
            : int.tryParse(data['totalCount']?.toString() ?? ''),
      );
    }

    return PagedResult(items: []);
  }
}
