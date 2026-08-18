import 'dart:async';

import 'package:flutter/foundation.dart';

import '../models/category.dart' as models;
import '../models/wellness_service.dart';
import '../services/wellness_service_service.dart';
import '../utils/api_exception.dart';

class WellnessServiceProvider extends ChangeNotifier {
  WellnessServiceProvider(this._wellnessServiceService);

  final WellnessServiceService _wellnessServiceService;

  static const int pageSize = 20;

  final List<WellnessService> _services = [];
  final List<models.Category> _categories = [];
  int? _selectedCategoryId;
  String _searchText = '';
  int _currentPage = 0;
  int? _totalCount;
  bool _hasMore = false;
  bool _isLoading = false;
  bool _isLoadingMore = false;
  String? _error;
  Timer? _searchDebounce;
  int _requestVersion = 0;

  List<WellnessService> get services => List.unmodifiable(_services);
  List<models.Category> get categories => List.unmodifiable(_categories);
  int? get selectedCategoryId => _selectedCategoryId;
  String get searchText => _searchText;
  int get currentPage => _currentPage;
  int get totalCount => _totalCount ?? _services.length;
  bool get hasMore => _hasMore;
  bool get isLoading => _isLoading;
  bool get isLoadingMore => _isLoadingMore;
  String? get error => _error;
  bool get isEmpty => !_isLoading && _error == null && _services.isEmpty;

  Future<void> loadInitial() async {
    _searchDebounce?.cancel();
    final requestVersion = ++_requestVersion;

    _isLoading = true;
    _isLoadingMore = false;
    _error = null;
    _services.clear();
    _currentPage = 0;
    _totalCount = null;
    _hasMore = false;
    notifyListeners();

    try {
      final categoriesResult =
          await _wellnessServiceService.getCategories(isActive: true);
      if (requestVersion != _requestVersion) {
        return;
      }

      final servicesResult = await _wellnessServiceService.getServices(
        name: _searchText,
        serviceCategoryId: _selectedCategoryId,
        isActive: true,
        page: 1,
        pageSize: pageSize,
      );

      if (requestVersion != _requestVersion) {
        return;
      }

      _categories
        ..clear()
        ..addAll(
          categoriesResult.items.where(
            (models.Category category) => category.isActive,
          ),
        );
      _replaceServices(
        servicesResult.items.where((service) => service.isActive).toList(),
        servicesResult.totalCount,
      );
      _error = null;
    } on ApiException catch (error) {
      if (requestVersion == _requestVersion) {
        _error = error.message;
      }
    } catch (_) {
      if (requestVersion == _requestVersion) {
        _error = 'Unable to load services.';
      }
    } finally {
      if (requestVersion == _requestVersion) {
        _isLoading = false;
        notifyListeners();
      }
    }
  }

  Future<void> refresh() => loadInitial();

  Future<void> retry() => loadInitial();

  void setSearchText(String value) {
    _searchText = value;
    _searchDebounce?.cancel();
    _requestVersion++;
    _searchDebounce = Timer(const Duration(milliseconds: 450), () {
      _loadFirstPage();
    });
  }

  Future<void> setSelectedCategoryId(int? categoryId) async {
    if (_selectedCategoryId == categoryId) {
      return;
    }

    _selectedCategoryId = categoryId;
    await _loadFirstPage();
  }

  Future<void> loadMore() async {
    if (!_hasMore || _isLoading || _isLoadingMore) {
      return;
    }

    final requestVersion = ++_requestVersion;
    final nextPage = _currentPage + 1;
    _isLoadingMore = true;
    notifyListeners();

    try {
      final result = await _wellnessServiceService.getServices(
        name: _searchText,
        serviceCategoryId: _selectedCategoryId,
        isActive: true,
        page: nextPage,
        pageSize: pageSize,
      );

      if (requestVersion != _requestVersion) {
        return;
      }

      final activeItems =
          result.items.where((service) => service.isActive).toList();
      final existingIds = _services.map((service) => service.id).toSet();
      _services.addAll(
        activeItems.where((service) => existingIds.add(service.id)),
      );
      _currentPage = nextPage;
      _totalCount = result.totalCount ?? _totalCount;
      _hasMore = activeItems.isNotEmpty &&
          (_totalCount == null || _services.length < _totalCount!);
      _error = null;
    } on ApiException catch (error) {
      if (requestVersion == _requestVersion) {
        _error = error.message;
      }
    } catch (_) {
      if (requestVersion == _requestVersion) {
        _error = 'Unable to load more services.';
      }
    } finally {
      if (requestVersion == _requestVersion) {
        _isLoadingMore = false;
        notifyListeners();
      }
    }
  }

  Future<void> _loadFirstPage() async {
    final requestVersion = ++_requestVersion;

    _isLoading = true;
    _isLoadingMore = false;
    _error = null;
    _services.clear();
    _currentPage = 0;
    _totalCount = null;
    _hasMore = false;
    notifyListeners();

    try {
      final result = await _wellnessServiceService.getServices(
        name: _searchText,
        serviceCategoryId: _selectedCategoryId,
        isActive: true,
        page: 1,
        pageSize: pageSize,
      );

      if (requestVersion != _requestVersion) {
        return;
      }

      _replaceServices(
        result.items.where((service) => service.isActive).toList(),
        result.totalCount,
      );
      _error = null;
    } on ApiException catch (error) {
      if (requestVersion == _requestVersion) {
        _error = error.message;
      }
    } catch (_) {
      if (requestVersion == _requestVersion) {
        _error = 'Unable to load services.';
      }
    } finally {
      if (requestVersion == _requestVersion) {
        _isLoading = false;
        notifyListeners();
      }
    }
  }

  void _replaceServices(List<WellnessService> items, int? totalCount) {
    _services
      ..clear()
      ..addAll(items);
    _currentPage = 1;
    _totalCount = totalCount;
    _hasMore = items.isNotEmpty &&
        (totalCount == null
            ? items.length == pageSize
            : items.length < totalCount);
  }

  @override
  void dispose() {
    _searchDebounce?.cancel();
    _requestVersion++;
    super.dispose();
  }
}
