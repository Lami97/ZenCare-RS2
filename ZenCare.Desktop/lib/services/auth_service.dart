import 'api_service.dart';

class AuthService {
  AuthService(this._apiService);

  final ApiService _apiService;

  Future<Map<String, dynamic>> login(String username, String password) {
    return _apiService.postMap(
      'Auth/Login',
      data: {'username': username, 'password': password},
    );
  }

  Future<void> logout() async {
    await _apiService.postMap('Auth/Logout');
  }
}
