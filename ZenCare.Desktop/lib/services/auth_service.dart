import '../models/auth_dtos.dart';
import 'api_service.dart';

class AuthService {
  AuthService(this._apiService);

  final ApiService _apiService;

  Future<LoginResponseDto> login(String username, String password) {
    final request = LoginRequestDto(username: username, password: password);
    return _apiService.postObject(
      'Auth/Login',
      data: request.toJson(),
      decode: LoginResponseDto.fromJson,
    );
  }

  Future<void> logout() async {
    await _apiService.post('Auth/Logout');
  }
}
