import '../models/recommendation.dart';
import '../models/paged_result.dart';
import 'api_service.dart';

class RecommendationService {
  RecommendationService(this._apiService);

  final ApiService _apiService;

  Future<List<Recommendation>> getProductRecommendations({int take = 5}) {
    return _apiService.get<List<Recommendation>>(
      '/Recommendation/My/products',
      queryParameters: {'page': 1, 'pageSize': take},
      fromJson: (data) => PagedResult<Recommendation>.fromJson(
        data as Map<String, dynamic>,
        Recommendation.fromJson,
      ).items,
    );
  }

  Future<List<Recommendation>> getServiceRecommendations({int take = 5}) {
    return _apiService.get<List<Recommendation>>(
      '/Recommendation/My/services',
      queryParameters: {'page': 1, 'pageSize': take},
      fromJson: (data) => PagedResult<Recommendation>.fromJson(
        data as Map<String, dynamic>,
        Recommendation.fromJson,
      ).items,
    );
  }
}
