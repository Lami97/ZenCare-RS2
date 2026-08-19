import 'dart:convert';

import 'package:flutter/foundation.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '../models/forgot_password_request.dart';
import '../models/login_request.dart';
import '../models/login_response.dart';
import '../models/change_password_request.dart';
import '../models/register_request.dart';
import '../models/register_response.dart';
import '../models/reset_password_request.dart';
import '../models/update_profile_request.dart';
import '../models/user.dart';
import '../models/user_profile_response.dart';
import '../services/api_service.dart';
import '../services/auth_service.dart';
import '../utils/api_exception.dart';

class AuthProvider extends ChangeNotifier {
  static const _tokenKey = 'auth_token';
  static const _userKey = 'auth_user';
  static const _expiresAtKey = 'auth_expires_at';

  AuthService? _authService;
  ApiService? _apiService;
  bool _configured = false;
  bool _isInitializing = true;
  bool _isLoading = false;
  bool _isRegistering = false;
  bool _isResettingPassword = false;
  String? _token;
  User? _user;

  bool get isInitializing => _isInitializing;
  bool get isLoading => _isLoading;
  bool get isRegistering => _isRegistering;
  bool get isResettingPassword => _isResettingPassword;
  bool get isAuthenticated => _token != null && _user != null;
  User? get user => _user;
  String? get token => _token;

  void configure(AuthService authService, ApiService apiService) {
    _authService = authService;
    _apiService = apiService;
    _apiService!.setUnauthorizedHandler(() => logout(notifyServer: false));

    if (!_configured) {
      _configured = true;
      _loadSession();
    }
  }

  Future<void> login(String username, String password) async {
    _setLoading(true);

    try {
      final response = await _authService!.login(
        LoginRequest(username: username, password: password),
      );
      await _saveSession(response);
    } finally {
      _setLoading(false);
    }
  }

  Future<RegisterResponse> register(RegisterRequest request) async {
    _setRegistering(true);

    try {
      return await _authService!.register(request);
    } finally {
      _setRegistering(false);
    }
  }

  Future<String> forgotPassword(String email) async {
    _setResettingPassword(true);

    try {
      return await _authService!.forgotPassword(
        ForgotPasswordRequest(email: email),
      );
    } finally {
      _setResettingPassword(false);
    }
  }

  Future<String> resetPassword(ResetPasswordRequest request) async {
    _setResettingPassword(true);

    try {
      return await _authService!.resetPassword(request);
    } finally {
      _setResettingPassword(false);
    }
  }

  Future<void> updateProfile(UpdateProfileRequest request) async {
    final currentUser = _user;
    if (currentUser == null) {
      throw ApiException('Profile could not be updated. Please sign in again.');
    }

    _setLoading(true);

    try {
      final response = await _authService!.updateProfile(request);
      await _saveUser(_mapProfileResponse(response, currentUser.roles));
    } finally {
      _setLoading(false);
    }
  }

  Future<void> refreshProfile() async {
    final currentUser = _user;
    if (currentUser == null) {
      throw ApiException('Profile could not be loaded. Please sign in again.');
    }

    final response = await _authService!.getProfile();
    await _saveUser(_mapProfileResponse(response, currentUser.roles));
  }

  Future<void> changePassword(ChangePasswordRequest request) async {
    if (_user == null) {
      throw ApiException('Password could not be changed. Please sign in again.');
    }

    _setLoading(true);

    try {
      await _authService!.changePassword(request);
    } finally {
      _setLoading(false);
    }
  }

  Future<void> logout({bool notifyServer = true}) async {
    if (notifyServer && _token != null) {
      try {
        await _authService!.logout();
      } on ApiException catch (error) {
        if (error.statusCode != 401) {
          rethrow;
        }
      }
    }

    await _clearSession();
  }

  Future<void> _clearSession() async {
    final preferences = await SharedPreferences.getInstance();
    await preferences.remove(_tokenKey);
    await preferences.remove(_userKey);
    await preferences.remove(_expiresAtKey);

    _token = null;
    _user = null;
    _apiService?.setToken(null);
    notifyListeners();
  }

  Future<void> _loadSession() async {
    final preferences = await SharedPreferences.getInstance();
    final token = preferences.getString(_tokenKey);
    final userJson = preferences.getString(_userKey);
    final expiresAtValue = preferences.getString(_expiresAtKey);

    if (token != null && userJson != null && expiresAtValue != null) {
      final expiresAt = DateTime.tryParse(expiresAtValue);

      if (expiresAt != null && expiresAt.isAfter(DateTime.now().toUtc())) {
        _token = token;
        _user = User.fromJson(jsonDecode(userJson) as Map<String, dynamic>);
        _apiService?.setToken(token);
      } else {
        await logout();
      }
    }

    _isInitializing = false;
    notifyListeners();
  }

  Future<void> _saveSession(LoginResponse response) async {
    final user = User(
      id: response.userId,
      firstName: response.firstName,
      lastName: response.lastName,
      username: response.username,
      email: response.email,
      fullName: response.fullName,
      phoneNumber: response.phoneNumber,
      isActive: response.isActive,
      roles: response.roles,
    );

    await _saveUser(user);

    final preferences = await SharedPreferences.getInstance();
    await preferences.setString(_tokenKey, response.token);
    await preferences.setString(_expiresAtKey, response.expiresAt.toUtc().toIso8601String());

    _token = response.token;
    _apiService?.setToken(response.token);
    notifyListeners();
  }

  Future<void> _saveUser(User user) async {
    final preferences = await SharedPreferences.getInstance();
    await preferences.setString(_userKey, jsonEncode(user.toJson()));

    _user = user;
    notifyListeners();
  }

  User _mapProfileResponse(UserProfileResponse response, List<String> roles) {
    return User(
      id: response.id,
      firstName: response.firstName,
      lastName: response.lastName,
      username: response.username,
      email: response.email,
      fullName: response.fullName,
      phoneNumber: response.phoneNumber,
      isActive: response.isActive,
      roles: roles,
    );
  }

  void _setLoading(bool value) {
    _isLoading = value;
    notifyListeners();
  }

  void _setRegistering(bool value) {
    _isRegistering = value;
    notifyListeners();
  }

  void _setResettingPassword(bool value) {
    _isResettingPassword = value;
    notifyListeners();
  }
}
