import '../models/paged_result.dart';
import '../models/user_notification.dart';
import 'api_service.dart';

class NotificationService {
  NotificationService(this._apiService);

  final ApiService _apiService;

  Future<PagedResult<UserNotification>> getMyNotifications({
    int page = 1,
    int pageSize = 100,
  }) {
    return _apiService.get<PagedResult<UserNotification>>(
      '/Notification/My',
      queryParameters: {
        'Page': page,
        'PageSize': pageSize,
        'IncludeTotalCount': true,
      },
      fromJson: (data) => PagedResult<UserNotification>.fromJson(
        data as Map<String, dynamic>,
        UserNotification.fromJson,
      ),
    );
  }

  Future<UserNotification> markAsRead(int id) {
    return _apiService.put<UserNotification>(
      '/Notification/My/$id/read',
      fromJson: (data) => UserNotification.fromJson(data as Map<String, dynamic>),
    );
  }
}
