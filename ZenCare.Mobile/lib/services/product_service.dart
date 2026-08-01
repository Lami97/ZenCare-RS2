import '../models/category.dart';
import '../models/paged_result.dart';
import '../models/product.dart';
import 'api_service.dart';

class ProductService {
  ProductService(this._apiService);

  final ApiService _apiService;

  Future<PagedResult<Product>> getProducts({
    String? name,
    int? productCategoryId,
    int? productTypeId,
    bool? isActive,
  }) {
    return _apiService.get<PagedResult<Product>>(
      '/Product',
      queryParameters: {
        if (name != null && name.trim().isNotEmpty) 'Name': name.trim(),
        if (productCategoryId != null) 'ProductCategoryId': productCategoryId,
        if (productTypeId != null) 'ProductTypeId': productTypeId,
        if (isActive != null) 'IsActive': isActive,
        'IncludeTotalCount': true,
      },
      fromJson: (data) => PagedResult<Product>.fromJson(
        data as Map<String, dynamic>,
        Product.fromJson,
      ),
    );
  }

  Future<Product> getProductById(int id) {
    return _apiService.get<Product>(
      '/Product/$id',
      fromJson: (data) => Product.fromJson(data as Map<String, dynamic>),
    );
  }

  Future<PagedResult<Category>> getCategories({bool? isActive}) {
    return _apiService.get<PagedResult<Category>>(
      '/ProductCategory',
      queryParameters: {
        if (isActive != null) 'IsActive': isActive,
        'IncludeTotalCount': true,
      },
      fromJson: (data) => PagedResult<Category>.fromJson(
        data as Map<String, dynamic>,
        Category.fromJson,
      ),
    );
  }
}
