import 'package:flutter/foundation.dart';

import '../models/recommendation.dart';
import '../services/recommendation_service.dart';
import '../utils/api_exception.dart';

class RecommendationProvider extends ChangeNotifier {
  RecommendationProvider(this._recommendationService);

  final RecommendationService _recommendationService;

  final List<Recommendation> _productRecommendations = [];
  final List<Recommendation> _serviceRecommendations = [];
  bool _isLoading = false;
  String? _error;

  List<Recommendation> get productRecommendations => List.unmodifiable(_productRecommendations);
  List<Recommendation> get serviceRecommendations => List.unmodifiable(_serviceRecommendations);
  bool get isLoading => _isLoading;
  String? get error => _error;
  bool get hasNoProductRecommendations => !_isLoading && _error == null && _productRecommendations.isEmpty;
  bool get hasNoServiceRecommendations => !_isLoading && _error == null && _serviceRecommendations.isEmpty;

  Future<void> loadRecommendations() async {
    _isLoading = true;
    _error = null;
    notifyListeners();

    try {
      final results = await Future.wait([
        _recommendationService.getProductRecommendations(),
        _recommendationService.getServiceRecommendations(),
      ]);

      _productRecommendations
        ..clear()
        ..addAll(results[0]);
      _serviceRecommendations
        ..clear()
        ..addAll(results[1]);
      _error = null;
    } on ApiException catch (error) {
      _error = error.message;
    } catch (_) {
      _error = 'Unable to load recommendations.';
    } finally {
      _isLoading = false;
      notifyListeners();
    }
  }

  Future<void> refresh() => loadRecommendations();
  Future<void> retry() => loadRecommendations();
}
