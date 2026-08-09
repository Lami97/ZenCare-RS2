import '../models/login_request.dart';
import '../models/login_response.dart';
import '../models/register_request.dart';
import '../models/register_response.dart';
import '../models/change_password_request.dart';
import '../models/update_profile_request.dart';
import '../models/user_profile_response.dart';
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

  Future<RegisterResponse> register(RegisterRequest request) {
    return _apiService.post<RegisterResponse>(
      '/Auth/Register',
      data: request.toJson(),
      fromJson: (data) => RegisterResponse.fromJson(data as Map<String, dynamic>),
    );
  }

  Future<UserProfileResponse> updateProfile(UpdateProfileRequest request) {
    return _apiService.put<UserProfileResponse>(
      '/User/My/profile',
      data: request.toJson(),
      fromJson: (data) => UserProfileResponse.fromJson(data as Map<String, dynamic>),
    );
  }

  Future<UserProfileResponse> getProfile() {
    return _apiService.get<UserProfileResponse>(
      '/User/My/profile',
      fromJson: (data) => UserProfileResponse.fromJson(data as Map<String, dynamic>),
    );
  }

  Future<void> changePassword(ChangePasswordRequest request) {
    return _apiService.put<void>(
      '/User/My/password',
      data: request.toJson(),
      fromJson: (_) {},
    );
  }
}
