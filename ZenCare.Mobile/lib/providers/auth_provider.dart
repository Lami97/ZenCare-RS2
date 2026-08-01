import 'dart:convert';

import 'package:flutter/foundation.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '../models/login_request.dart';
import '../models/login_response.dart';
import '../models/user.dart';
import '../services/api_service.dart';
import '../services/auth_service.dart';

class AuthProvider extends ChangeNotifier {
  static const _tokenKey = 'auth_token';
  static const _userKey = 'auth_user';
  static const _expiresAtKey = 'auth_expires_at';

  AuthService? _authService;
  ApiService? _apiService;
  bool _configured = false;
  bool _isInitializing = true;
  bool _isLoading = false;
  String? _token;
  User? _user;

  bool get isInitializing => _isInitializing;
  bool get isLoading => _isLoading;
  bool get isAuthenticated => _token != null && _user != null;
  User? get user => _user;
  String? get token => _token;

  void configure(AuthService authService, ApiService apiService) {
    _authService = authService;
    _apiService = apiService;
    _apiService!.setUnauthorizedHandler(logout);

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

  Future<void> logout() async {
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
      username: response.username,
      email: response.email,
      fullName: response.fullName,
      roles: response.roles,
    );

    final preferences = await SharedPreferences.getInstance();
    await preferences.setString(_tokenKey, response.token);
    await preferences.setString(_userKey, jsonEncode(user.toJson()));
    await preferences.setString(_expiresAtKey, response.expiresAt.toUtc().toIso8601String());

    _token = response.token;
    _user = user;
    _apiService?.setToken(response.token);
    notifyListeners();
  }

  void _setLoading(bool value) {
    _isLoading = value;
    notifyListeners();
  }
}
