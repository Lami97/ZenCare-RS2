import 'admin_models.dart';

class PagedResult<T> {
  const PagedResult({required this.items, this.totalCount});

  final List<T> items;
  final int? totalCount;

  factory PagedResult.fromJson(
    Object? data,
    T Function(JsonMap json) decodeItem,
  ) {
    if (data is List) {
      return PagedResult(
        items: data
            .whereType<Map>()
            .map((item) => decodeItem(JsonMap.from(item)))
            .toList(),
      );
    }

    if (data is Map) {
      final json = JsonMap.from(data);
      final rawItems = json['items'];
      return PagedResult(
        items: rawItems is List
            ? rawItems
                  .whereType<Map>()
                  .map((item) => decodeItem(JsonMap.from(item)))
                  .toList()
            : const [],
        totalCount: json['totalCount'] == null
            ? null
            : jsonInt(json['totalCount']),
      );
    }

    return const PagedResult(items: []);
  }
}
