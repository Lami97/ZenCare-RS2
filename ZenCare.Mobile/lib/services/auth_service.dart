import '../models/login_request.dart';
import '../models/login_response.dart';
import 'api_service.dart';

class AuthService {
  AuthService(this._apiService);

  final ApiService _apiService;

  Future<LoginResponse> login(LoginRequest request) {
    return _apiService.post<LoginResponse>(
      '/Auth/Login',
      data: request.toJson(),
      fromJson: (data) => LoginResponse.fromJson(data as Map<String, dynamic>),
    );
  }
}
