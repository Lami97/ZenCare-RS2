import '../models/category.dart';
import '../models/paged_result.dart';
import '../models/product.dart';
import 'api_service.dart';

class ProductService {
  ProductService(this._apiService);

  final ApiService _apiService;

  Future<PagedResult<Product>> getProducts({String? name}) {
    return _apiService.get<PagedResult<Product>>(
      '/Product',
      queryParameters: {
        if (name != null && name.trim().isNotEmpty) 'Name': name.trim(),
      },
      fromJson: (data) => PagedResult<Product>.fromJson(
        data as Map<String, dynamic>,
        Product.fromJson,
      ),
    );
  }

  Future<PagedResult<Category>> getCategories() {
    return _apiService.get<PagedResult<Category>>(
      '/ProductCategory',
      fromJson: (data) => PagedResult<Category>.fromJson(
        data as Map<String, dynamic>,
        Category.fromJson,
      ),
    );
  }
}
