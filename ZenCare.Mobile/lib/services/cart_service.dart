import '../models/cart.dart';
import '../models/cart_item.dart';
import '../models/paged_result.dart';
import '../models/product.dart';
import '../models/purchase.dart';
import 'api_service.dart';

class CartService {
  CartService(this._apiService);

  final ApiService _apiService;

  Future<Cart> getMyCart() {
    return _apiService.get<Cart>(
      '/Cart/My',
      fromJson: (data) => Cart.fromJson(data as Map<String, dynamic>),
    );
  }

  Future<Cart> createMyCart() {
    return _apiService.post<Cart>(
      '/Cart/My',
      data: {'userId': 0},
      fromJson: (data) => Cart.fromJson(data as Map<String, dynamic>),
    );
  }

  Future<PagedResult<CartItem>> getMyCartItems({bool includeTotalCount = true}) {
    return _apiService.get<PagedResult<CartItem>>(
      '/CartItem/My',
      queryParameters: {
        'Page': 1,
        'PageSize': 100,
        'IncludeTotalCount': includeTotalCount,
      },
      fromJson: (data) => PagedResult<CartItem>.fromJson(
        data as Map<String, dynamic>,
        CartItem.fromJson,
      ),
    );
  }

  Future<CartItem> addItem({
    required int cartId,
    required Product product,
    required int quantity,
  }) {
    return _apiService.post<CartItem>(
      '/CartItem/My',
      data: {
        'cartId': cartId,
        'productId': product.id,
        'quantity': quantity,
        'unitPrice': product.price,
      },
      fromJson: (data) => CartItem.fromJson(data as Map<String, dynamic>),
    );
  }

  Future<CartItem> updateItem({
    required CartItem item,
    required int quantity,
  }) {
    return _apiService.put<CartItem>(
      '/CartItem/My/${item.id}',
      data: {
        'id': item.id,
        'cartId': item.cartId,
        'productId': item.productId,
        'quantity': quantity,
        'unitPrice': item.unitPrice,
      },
      fromJson: (data) => CartItem.fromJson(data as Map<String, dynamic>),
    );
  }

  Future<void> removeItem(int id) {
    return _apiService.delete<void>(
      '/CartItem/My/$id',
      fromJson: (_) {},
    );
  }

  Future<Purchase> checkout(List<CartItem> items) {
    return _apiService.post<Purchase>(
      '/Purchase/Checkout',
      data: {
        'items': items
            .map((item) => {
                  'productId': item.productId,
                  'quantity': item.quantity,
                })
            .toList(),
      },
      fromJson: (data) => Purchase.fromJson(data as Map<String, dynamic>),
    );
  }
}
