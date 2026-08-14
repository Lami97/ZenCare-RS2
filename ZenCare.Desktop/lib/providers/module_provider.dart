import 'package:flutter/foundation.dart';

import '../models/admin_models.dart';
import '../services/admin_repository.dart';
import '../utils/api_exception.dart';

class ModuleProvider extends ChangeNotifier {
  ModuleProvider(this._repository, this.module);

  final AdminRepository _repository;
  final AdminModule module;

  final int pageSize = 20;
  var _page = 1;
  var _isLoading = false;
  var _isMutating = false;
  String? _error;
  List<Map<String, dynamic>> _items = [];
  int? _totalCount;
  Map<String, dynamic> _filters = {};

  int get page => _page;
  bool get isLoading => _isLoading;
  bool get isMutating => _isMutating;
  String? get error => _error;
  List<Map<String, dynamic>> get items => List.unmodifiable(_items);
  int? get totalCount => _totalCount;
  int get totalPages => _totalCount == null
      ? _page
      : ((_totalCount! + pageSize - 1) ~/ pageSize).clamp(1, 999999);
  bool get canPrevious => _page > 1;
  bool get canNext =>
      _totalCount == null ? _items.length == pageSize : _page < totalPages;

  Future<void> load({Map<String, dynamic>? filters, int? page}) async {
    _isLoading = true;
    _error = null;
    if (filters != null) _filters = filters;
    if (page != null) _page = page;
    notifyListeners();
    try {
      final result = await _repository.list(
        module,
        page: _page,
        pageSize: pageSize,
        filters: _filters,
      );
      _items = result.items;
      _totalCount = result.totalCount;
    } on ApiException catch (error) {
      _error = error.message;
    } catch (_) {
      _error = 'Data could not be loaded.';
    } finally {
      _isLoading = false;
      notifyListeners();
    }
  }

  Future<void> create(Map<String, dynamic> data) async {
    await _mutate(() => _repository.create(module, data));
  }

  Future<void> update(int id, Map<String, dynamic> data) async {
    await _mutate(() => _repository.update(module, id, data));
  }

  Future<void> delete(int id) async {
    await _mutate(() => _repository.delete(module, id));
  }

  Future<void> nextPage() async {
    if (!canNext) return;
    await load(page: _page + 1);
  }

  Future<void> previousPage() async {
    if (!canPrevious) return;
    await load(page: _page - 1);
  }

  Future<void> refresh() => load(page: _page);

  Future<void> _mutate(Future<void> Function() action) async {
    _isMutating = true;
    notifyListeners();
    try {
      await action();
      await load(page: _page);
    } finally {
      _isMutating = false;
      notifyListeners();
    }
  }
}
