import 'dart:async';

import 'package:flutter/foundation.dart';

import '../models/category.dart' as models;
import '../models/product.dart';
import '../services/product_service.dart';
import '../utils/api_exception.dart';

class ProductProvider extends ChangeNotifier {
  ProductProvider(this._productService);

  final ProductService _productService;

  static const int pageSize = 20;

  final List<Product> _products = [];
  final List<models.Category> _categories = [];
  int? _selectedCategoryId;
  String _searchText = '';
  int _visibleCount = pageSize;
  int? _totalCount;
  bool _isLoading = false;
  bool _isLoadingMore = false;
  String? _error;
  Timer? _searchDebounce;
  int _requestVersion = 0;

  List<Product> get products => _products.take(_visibleCount).toList();
  List<models.Category> get categories => List.unmodifiable(_categories);
  int? get selectedCategoryId => _selectedCategoryId;
  String get searchText => _searchText;
  int get currentPage => (_visibleCount / pageSize).ceil();
  int get totalCount => _totalCount ?? _products.length;
  bool get isLoading => _isLoading;
  bool get isLoadingMore => _isLoadingMore;
  String? get error => _error;
  bool get hasMore => _visibleCount < _products.length;
  bool get isEmpty => !_isLoading && _error == null && _products.isEmpty;

  Future<void> loadInitial() async {
    _isLoading = true;
    _error = null;
    notifyListeners();

    await Future.wait([
      _loadCategories(),
      _loadProducts(resetVisibleCount: true),
    ]);

    _isLoading = false;
    notifyListeners();
  }

  Future<void> refresh() async {
    _searchDebounce?.cancel();
    await _loadCategories();
    await _loadProducts(resetVisibleCount: true);
  }

  Future<void> retry() async {
    await loadInitial();
  }

  void setSearchText(String value) {
    _searchText = value;
    _searchDebounce?.cancel();
    _searchDebounce = Timer(const Duration(milliseconds: 450), () {
      _loadProducts(resetVisibleCount: true);
    });
  }

  Future<void> setSelectedCategoryId(int? categoryId) async {
    if (_selectedCategoryId == categoryId) {
      return;
    }

    _selectedCategoryId = categoryId;
    notifyListeners();
    await _loadProducts(resetVisibleCount: true);
  }

  Future<void> loadMore() async {
    if (!hasMore || _isLoadingMore) {
      return;
    }

    _isLoadingMore = true;
    notifyListeners();

    _visibleCount = (_visibleCount + pageSize).clamp(0, _products.length).toInt();

    _isLoadingMore = false;
    notifyListeners();
  }

  Future<void> _loadCategories() async {
    try {
      final result = await _productService.getCategories(isActive: true);
      _categories
        ..clear()
        ..addAll(result.items.where((category) => category.isActive));
    } on ApiException catch (error) {
      _error = error.message;
    } catch (_) {
      _error = 'Unable to load product categories.';
    }
  }

  Future<void> _loadProducts({required bool resetVisibleCount}) async {
    final requestVersion = ++_requestVersion;

    if (resetVisibleCount) {
      _visibleCount = pageSize;
    }

    _error = null;
    notifyListeners();

    try {
      final result = await _productService.getProducts(
        name: _searchText,
        productCategoryId: _selectedCategoryId,
        isActive: true,
      );

      if (requestVersion != _requestVersion) {
        return;
      }

      _products
        ..clear()
        ..addAll(result.items.where((product) => product.isActive));
      _totalCount = result.totalCount ?? _products.length;
      _visibleCount = _visibleCount.clamp(0, _products.length).toInt();
      _error = null;
    } on ApiException catch (error) {
      if (requestVersion == _requestVersion) {
        _error = error.message;
      }
    } catch (_) {
      if (requestVersion == _requestVersion) {
        _error = 'Unable to load products.';
      }
    } finally {
      if (requestVersion == _requestVersion) {
        notifyListeners();
      }
    }
  }

  @override
  void dispose() {
    _searchDebounce?.cancel();
    super.dispose();
  }
}


