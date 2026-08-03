import 'package:flutter/foundation.dart';

import '../models/purchase.dart';
import '../services/purchase_service.dart';
import '../utils/api_exception.dart';

class PurchaseProvider extends ChangeNotifier {
  PurchaseProvider(this._purchaseService);

  final PurchaseService _purchaseService;

  final List<Purchase> _purchases = [];
  bool _isLoading = false;
  String? _error;

  List<Purchase> get purchases => List.unmodifiable(_purchases);
  bool get isLoading => _isLoading;
  String? get error => _error;
  bool get isEmpty => !_isLoading && _error == null && _purchases.isEmpty;

  Future<void> loadPurchases() async {
    _isLoading = true;
    _error = null;
    notifyListeners();

    try {
      final result = await _purchaseService.getMyPurchases();
      _purchases
        ..clear()
        ..addAll(result.items);
      _purchases.sort((a, b) => b.createdAt.compareTo(a.createdAt));
      _error = null;
    } on ApiException catch (error) {
      _error = error.message;
    } catch (_) {
      _error = 'Unable to load purchase history.';
    } finally {
      _isLoading = false;
      notifyListeners();
    }
  }

  Future<void> refresh() => loadPurchases();
  Future<void> retry() => loadPurchases();
}