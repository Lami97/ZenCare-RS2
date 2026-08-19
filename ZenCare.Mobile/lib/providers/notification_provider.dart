import 'dart:async';

import 'package:flutter/foundation.dart';

import '../models/user_notification.dart';
import '../services/notification_service.dart';
import '../utils/api_exception.dart';

class NotificationProvider extends ChangeNotifier {
  NotificationProvider(this._notificationService);

  static const Duration pollingInterval = Duration(seconds: 30);

  final NotificationService _notificationService;
  final List<UserNotification> _notifications = [];
  final Set<int> _markingIds = {};

  Timer? _pollingTimer;
  bool _isLoading = false;
  bool _requestInProgress = false;
  bool _isDisposed = false;
  String? _error;

  List<UserNotification> get notifications => List.unmodifiable(_notifications);
  bool get isLoading => _isLoading;
  String? get error => _error;
  bool get isEmpty => !_isLoading && _error == null && _notifications.isEmpty;
  bool isMarkingAsRead(int id) => _markingIds.contains(id);

  void start() {
    if (_pollingTimer != null) {
      return;
    }

    unawaited(loadNotifications(showLoading: true));
    _pollingTimer = Timer.periodic(
      pollingInterval,
      (_) => unawaited(loadNotifications()),
    );
  }

  Future<void> loadNotifications({bool showLoading = false}) async {
    if (_requestInProgress) {
      return;
    }

    _requestInProgress = true;

    if (showLoading && _notifications.isEmpty) {
      _isLoading = true;
      _error = null;
      _notifyListeners();
    }

    try {
      final result = await _notificationService.getMyNotifications();
      _notifications
        ..clear()
        ..addAll(result.items)
        ..sort((a, b) => b.createdAt.compareTo(a.createdAt));
      _error = null;
    } on ApiException catch (error) {
      if (_notifications.isEmpty) {
        _error = error.message;
      }
    } catch (_) {
      if (_notifications.isEmpty) {
        _error = 'Unable to load notifications.';
      }
    } finally {
      _requestInProgress = false;
      _isLoading = false;
      _notifyListeners();
    }
  }

  Future<void> markAsRead(int id) async {
    if (_markingIds.contains(id)) {
      return;
    }

    _markingIds.add(id);
    _notifyListeners();

    try {
      final updated = await _notificationService.markAsRead(id);
      final index = _notifications.indexWhere((item) => item.id == id);

      if (index >= 0) {
        _notifications[index] = updated;
      }
    } finally {
      _markingIds.remove(id);
      _notifyListeners();
    }
  }

  Future<void> refresh() => loadNotifications();
  Future<void> retry() => loadNotifications(showLoading: true);

  void _notifyListeners() {
    if (!_isDisposed) {
      notifyListeners();
    }
  }

  @override
  void dispose() {
    _isDisposed = true;
    _pollingTimer?.cancel();
    _pollingTimer = null;
    super.dispose();
  }
}
