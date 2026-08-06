import '../models/paged_result.dart';
import '../models/purchase.dart';
import 'api_service.dart';

class PurchaseService {
  PurchaseService(this._apiService);

  final ApiService _apiService;

  Future<PagedResult<Purchase>> getMyPurchases() {
    return _apiService.get<PagedResult<Purchase>>(
      '/Purchase/My',
      queryParameters: {
        'IncludeTotalCount': true,
      },
      fromJson: (data) => PagedResult<Purchase>.fromJson(
        data as Map<String, dynamic>,
        Purchase.fromJson,
      ),
    );
  }

  Future<Purchase> getMyPurchaseById(int id) {
    return _apiService.get<Purchase>(
      '/Purchase/My/$id',
      fromJson: (data) => Purchase.fromJson(data as Map<String, dynamic>),
    );
  }

  Future<Purchase> cancelMyPurchase(int id) {
    return _apiService.post<Purchase>(
      '/Purchase/My/cancel/$id',
      fromJson: (data) => Purchase.fromJson(data as Map<String, dynamic>),
    );
  }
}
