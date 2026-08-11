import '../models/paged_result.dart';
import '../models/review.dart';
import '../models/review_create_request.dart';
import 'api_service.dart';

class ReviewService {
  ReviewService(this._apiService);

  final ApiService _apiService;

  Future<PagedResult<Review>> getMyReviews({
    int? appointmentId,
    int? productId,
    int page = 1,
    int pageSize = 20,
  }) {
    return _apiService.get<PagedResult<Review>>(
      '/Review/My',
      queryParameters: {
        if (appointmentId != null) 'AppointmentId': appointmentId,
        if (productId != null) 'ProductId': productId,
        'Page': page,
        'PageSize': pageSize,
        'IncludeTotalCount': true,
      },
      fromJson: (data) => PagedResult<Review>.fromJson(
        data as Map<String, dynamic>,
        Review.fromJson,
      ),
    );
  }

  Future<Review> getMyReviewById(int id) {
    return _apiService.get<Review>(
      '/Review/My/$id',
      fromJson: (data) => Review.fromJson(data as Map<String, dynamic>),
    );
  }

  Future<Review> createMyReview(ReviewCreateRequest request) {
    return _apiService.post<Review>(
      '/Review/My',
      data: request.toJson(),
      fromJson: (data) => Review.fromJson(data as Map<String, dynamic>),
    );
  }
}
