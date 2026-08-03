import 'package:flutter/foundation.dart';

import '../models/review.dart';
import '../services/review_service.dart';
import '../utils/api_exception.dart';

class ReviewProvider extends ChangeNotifier {
  ReviewProvider(this._reviewService);

  final ReviewService _reviewService;

  final List<Review> _reviews = [];
  bool _isLoading = false;
  String? _error;
  int? _totalCount;

  List<Review> get reviews => List.unmodifiable(_reviews);
  bool get isLoading => _isLoading;
  String? get error => _error;
  int get totalCount => _totalCount ?? _reviews.length;
  bool get isEmpty => !_isLoading && _error == null && _reviews.isEmpty;

  Future<void> loadReviews() async {
    _isLoading = true;
    _error = null;
    notifyListeners();

    try {
      final result = await _reviewService.getMyReviews();
      _reviews
        ..clear()
        ..addAll(result.items);
      _reviews.sort((a, b) => b.createdAt.compareTo(a.createdAt));
      _totalCount = result.totalCount ?? _reviews.length;
      _error = null;
    } on ApiException catch (error) {
      _error = error.message;
    } catch (_) {
      _error = 'Unable to load reviews.';
    } finally {
      _isLoading = false;
      notifyListeners();
    }
  }

  Future<void> refresh() => loadReviews();
  Future<void> retry() => loadReviews();
}