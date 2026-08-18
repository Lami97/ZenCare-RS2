import '../models/category.dart';
import '../models/paged_result.dart';
import '../models/wellness_service.dart';
import 'api_service.dart';

class WellnessServiceService {
  WellnessServiceService(this._apiService);

  final ApiService _apiService;

  Future<PagedResult<WellnessService>> getServices({
    String? name,
    int? serviceCategoryId,
    bool? isActive,
    String? sortBy,
    int page = 1,
    int pageSize = 20,
  }) {
    return _apiService.get<PagedResult<WellnessService>>(
      '/Service',
      queryParameters: {
        if (name != null && name.trim().isNotEmpty) 'Name': name.trim(),
        if (serviceCategoryId != null) 'ServiceCategoryId': serviceCategoryId,
        if (isActive != null) 'IsActive': isActive,
        if (sortBy != null && sortBy.trim().isNotEmpty) 'SortBy': sortBy.trim(),
        'Page': page,
        'PageSize': pageSize,
        'IncludeTotalCount': true,
      },
      fromJson: (data) => PagedResult<WellnessService>.fromJson(
        data as Map<String, dynamic>,
        WellnessService.fromJson,
      ),
    );
  }

  Future<PagedResult<Category>> getCategories({bool? isActive}) {
    return _apiService.get<PagedResult<Category>>(
      '/ServiceCategory',
      queryParameters: {
        if (isActive != null) 'IsActive': isActive,
        'Page': 1,
        'PageSize': 100,
        'IncludeTotalCount': true,
      },
      fromJson: (data) => PagedResult<Category>.fromJson(
        data as Map<String, dynamic>,
        Category.fromJson,
      ),
    );
  }

  Future<WellnessService> getServiceById(int id) {
    return _apiService.get<WellnessService>(
      '/Service/$id',
      fromJson: (data) =>
          WellnessService.fromJson(data as Map<String, dynamic>),
    );
  }
}
