import 'package:flutter/foundation.dart';

import '../models/cart.dart';
import '../models/cart_item.dart';
import '../models/product.dart';
import '../models/purchase.dart';
import '../services/cart_service.dart';
import '../services/product_service.dart';
import '../utils/api_exception.dart';

class CartProvider extends ChangeNotifier {
  CartProvider(this._cartService, this._productService);

  final CartService _cartService;
  final ProductService _productService;

  Cart? _cart;
  final List<CartItem> _items = [];
  bool _isLoading = false;
  bool _isMutating = false;
  String? _error;

  Cart? get cart => _cart;
  List<CartItem> get items => List.unmodifiable(_items);
  bool get isLoading => _isLoading;
  bool get isMutating => _isMutating;
  String? get error => _error;
  bool get isEmpty => !_isLoading && _error == null && _items.isEmpty;
  int get totalItemCount => _items.fold(0, (total, item) => total + item.quantity);
  double get totalPrice => _items.fold(0, (total, item) => total + item.subtotal);

  Future<void> loadCart() async {
    _isLoading = true;
    _error = null;
    notifyListeners();

    try {
      await _ensureCart();
      final result = await _cartService.getMyCartItems();
      _items
        ..clear()
        ..addAll(result.items);
      _error = null;
    } on ApiException catch (error) {
      _error = error.message;
    } catch (_) {
      _error = 'Unable to load cart.';
    } finally {
      _isLoading = false;
      notifyListeners();
    }
  }

  Future<void> refresh() => loadCart();
  Future<void> retry() => loadCart();

  Future<void> addProduct(Product product, {int quantity = 1}) async {
    if (product.stockQuantity <= 0) {
      throw ApiException('This product is out of stock.');
    }

    await _runMutation(() async {
      final cart = await _ensureCart();
      final existing = _findItemByProductId(product.id);

      if (existing == null) {
        await _cartService.addItem(cartId: cart.id, product: product, quantity: quantity);
      } else {
        final newQuantity = existing.quantity + quantity;
        if (newQuantity > product.stockQuantity) {
          throw ApiException('Maximum available stock reached.');
        }

        await _cartService.updateItem(item: existing, quantity: newQuantity);
      }

      await _reloadItems();
    });
  }

  Future<void> increaseQuantity(CartItem item) async {
    await _runMutation(() async {
      final product = await _productService.getProductById(item.productId);
      if (item.quantity >= product.stockQuantity) {
        throw ApiException('Maximum available stock reached.');
      }

      await _cartService.updateItem(item: item, quantity: item.quantity + 1);
      await _reloadItems();
    });
  }

  Future<void> decreaseQuantity(CartItem item) async {
    await _runMutation(() async {
      if (item.quantity <= 1) {
        await _cartService.removeItem(item.id);
      } else {
        await _cartService.updateItem(item: item, quantity: item.quantity - 1);
      }

      await _reloadItems();
    });
  }

  Future<void> removeItem(CartItem item) async {
    await _runMutation(() async {
      await _cartService.removeItem(item.id);
      await _reloadItems();
    });
  }

  Future<Purchase> checkout() async {
    if (_items.isEmpty) {
      throw ApiException('Your cart is empty.');
    }

    _isMutating = true;
    notifyListeners();

    try {
      final purchase = await _cartService.checkout(_items);

      for (final item in List<CartItem>.from(_items)) {
        try {
          await _cartService.removeItem(item.id);
        } catch (_) {
          // Checkout already succeeded; stale cart cleanup can be retried on refresh.
        }
      }

      await _reloadItems();
      return purchase;
    } on ApiException {
      rethrow;
    } catch (_) {
      throw ApiException('Checkout could not be completed.');
    } finally {
      _isMutating = false;
      notifyListeners();
    }
  }

  CartItem? _findItemByProductId(int productId) {
    for (final item in _items) {
      if (item.productId == productId) {
        return item;
      }
    }

    return null;
  }

  Future<Cart> _ensureCart() async {
    if (_cart != null) {
      return _cart!;
    }

    try {
      _cart = await _cartService.getMyCart();
    } on ApiException {
      _cart = await _cartService.createMyCart();
    }

    return _cart!;
  }

  Future<void> _reloadItems() async {
    final result = await _cartService.getMyCartItems();
    _items
      ..clear()
      ..addAll(result.items);
    _error = null;
  }

  Future<void> _runMutation(Future<void> Function() action) async {
    _isMutating = true;
    _error = null;
    notifyListeners();

    try {
      await action();
    } on ApiException catch (error) {
      _error = error.message;
      rethrow;
    } catch (_) {
      _error = 'Cart action could not be completed.';
      throw ApiException(_error!);
    } finally {
      _isMutating = false;
      notifyListeners();
    }
  }
}