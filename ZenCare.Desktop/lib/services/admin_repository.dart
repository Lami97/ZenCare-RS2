import '../models/admin_models.dart';
import '../models/paged_result.dart';
import 'api_service.dart';

class AdminRepository {
  AdminRepository(this._apiService);

  final ApiService _apiService;

  Future<PagedResult> list(
    AdminModule module, {
    required int page,
    required int pageSize,
    Map<String, dynamic>? filters,
  }) {
    final query = <String, dynamic>{
      'Page': page,
      'PageSize': pageSize,
      'IncludeTotalCount': true,
      ...?filters,
    };
    return _apiService.getPaged(module.endpoint, queryParameters: query);
  }

  Future<Map<String, dynamic>> getById(AdminModule module, int id) {
    return _apiService.getMap('${module.endpoint}/$id');
  }

  Future<void> create(AdminModule module, Map<String, dynamic> data) async {
    await _apiService.postMap(module.endpoint, data: data);
  }

  Future<void> update(
    AdminModule module,
    int id,
    Map<String, dynamic> data,
  ) async {
    await _apiService.putMap(
      '${module.endpoint}/$id',
      data: {...data, 'id': id},
    );
  }

  Future<void> delete(AdminModule module, int id) async {
    await _apiService.delete('${module.endpoint}/$id');
  }

  Future<List<LookupOption>> lookup(LookupConfig config) async {
    final items = <Map<String, dynamic>>[];
    var page = 1;
    const pageSize = 100;
    while (true) {
      final result = await _apiService.getPaged(
        config.endpoint,
        queryParameters: {
          'Page': page,
          'PageSize': pageSize,
          'IncludeTotalCount': true,
          ...config.queryParameters,
        },
      );
      items.addAll(result.items);
      final total = result.totalCount;
      if (result.items.length < pageSize ||
          (total != null && items.length >= total)) {
        break;
      }
      page++;
    }
    final optionsByValue = <int, LookupOption>{};
    for (final item in items) {
      final option = _toLookupOption(config, item);
      if (option.value > 0) {
        optionsByValue.putIfAbsent(option.value, () => option);
      }
    }
    return optionsByValue.values.toList();
  }

  Future<LookupOption> lookupById(LookupConfig config, int id) async {
    final item = await _apiService.getMap('${config.endpoint}/$id');
    return _toLookupOption(config, item);
  }

  LookupOption _toLookupOption(LookupConfig config, Map<String, dynamic> item) {
    final rawValue = item[config.valueKey];
    final value = rawValue is int
        ? rawValue
        : int.tryParse(rawValue?.toString() ?? '') ?? 0;
    return LookupOption(
      value: value,
      label: config.labelBuilder(item),
      raw: item,
    );
  }
}
