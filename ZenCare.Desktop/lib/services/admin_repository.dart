import '../models/admin_models.dart';
import '../models/business_analytics.dart';
import '../models/paged_result.dart';
import 'api_service.dart';

class AdminRepository {
  AdminRepository(this._apiService);

  final ApiService _apiService;

  Future<PagedResult<AdminEntity>> list(
    AdminModule module, {
    required int page,
    required int pageSize,
    Map<String, Object?>? filters,
  }) {
    final query = <String, Object?>{
      'Page': page,
      'PageSize': pageSize,
      'IncludeTotalCount': true,
      ...?filters,
    };
    return _apiService.getPaged(
      module.endpoint,
      queryParameters: query,
      decodeItem: module.decoder,
    );
  }

  Future<AdminEntity> getById(AdminModule module, int id) {
    return _apiService.getObject(
      '${module.endpoint}/$id',
      decode: module.decoder,
    );
  }

  Future<AdminEntity> create(AdminModule module, AdminWriteDto request) {
    return _apiService.postObject(
      module.endpoint,
      data: request.toJson(),
      decode: module.decoder,
    );
  }

  Future<AdminEntity> update(
    AdminModule module,
    int id,
    AdminWriteDto request,
  ) {
    return _apiService.putObject(
      '${module.endpoint}/$id',
      data: request.toJson(),
      decode: module.decoder,
    );
  }

  Future<void> delete(AdminModule module, int id) async {
    await _apiService.delete('${module.endpoint}/$id');
  }

  Future<BusinessAnalytics> getBusinessAnalytics({
    DateTime? dateFrom,
    DateTime? dateTo,
  }) async {
    return _apiService.getObject(
      'BusinessReport/analytics',
      queryParameters: {
        'DateFrom': _dateQueryValue(dateFrom),
        'DateTo': _dateQueryValue(dateTo),
      },
      decode: BusinessAnalytics.fromJson,
    );
  }

  String? _dateQueryValue(DateTime? value) {
    if (value == null) return null;
    final year = value.year.toString().padLeft(4, '0');
    final month = value.month.toString().padLeft(2, '0');
    final day = value.day.toString().padLeft(2, '0');
    return '$year-$month-$day';
  }

  Future<List<LookupOption>> lookup(LookupConfig config) async {
    final items = <AdminEntity>[];
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
        decodeItem: config.decoder,
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
    final item = await _apiService.getObject(
      '${config.endpoint}/$id',
      decode: config.decoder,
    );
    return _toLookupOption(config, item);
  }

  LookupOption _toLookupOption(LookupConfig config, AdminEntity item) {
    return LookupOption(value: item.id, label: config.labelBuilder(item));
  }
}
