import '../models/recommendation.dart';
import 'api_service.dart';

class RecommendationService {
  RecommendationService(this._apiService);

  final ApiService _apiService;

  Future<List<Recommendation>> getProductRecommendations({int take = 5}) {
    return _apiService.get<List<Recommendation>>(
      '/Recommendation/My/products',
      queryParameters: {'take': take},
      fromJson: (data) => (data as List<dynamic>)
          .map((item) => Recommendation.fromJson(item as Map<String, dynamic>))
          .toList(),
    );
  }

  Future<List<Recommendation>> getServiceRecommendations({int take = 5}) {
    return _apiService.get<List<Recommendation>>(
      '/Recommendation/My/services',
      queryParameters: {'take': take},
      fromJson: (data) => (data as List<dynamic>)
          .map((item) => Recommendation.fromJson(item as Map<String, dynamic>))
          .toList(),
    );
  }
}
