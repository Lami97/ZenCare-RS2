import 'dart:convert';

import 'package:flutter/foundation.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '../models/admin_models.dart';
import '../models/auth_dtos.dart';
import '../services/api_service.dart';
import '../services/auth_service.dart';
import '../utils/api_exception.dart';

class AuthProvider extends ChangeNotifier {
  AuthProvider(this._apiService, this._authService) {
    _apiService.setUnauthorizedHandler(() => logout(notifyServer: false));
  }

  static const _tokenKey = 'desktop_auth_token';
  static const _userKey = 'desktop_auth_user';
  static const _expiresAtKey = 'desktop_auth_expires_at';

  final ApiService _apiService;
  final AuthService _authService;

  bool _isInitializing = true;
  bool _isLoading = false;
  String? _token;
  AuthenticatedUser? _user;
  String? _error;

  bool get isInitializing => _isInitializing;
  bool get isLoading => _isLoading;
  bool get isAuthenticated => _token != null && _user != null;
  AuthenticatedUser? get user => _user;
  String? get error => _error;
  String get displayName => _user?.fullName.isNotEmpty == true
      ? _user!.fullName
      : _user?.username ?? 'Admin';
  List<String> get roles => _user?.roles ?? const [];
  bool get isAdmin => roles.any((role) => role.toLowerCase() == 'admin');

  Future<void> initialize() async {
    final prefs = await SharedPreferences.getInstance();
    final token = prefs.getString(_tokenKey);
    final userJson = prefs.getString(_userKey);
    final expiresAtText = prefs.getString(_expiresAtKey);
    final expiresAt = expiresAtText == null
        ? null
        : DateTime.tryParse(expiresAtText);

    if (token != null &&
        userJson != null &&
        expiresAt != null &&
        expiresAt.isAfter(DateTime.now().toUtc())) {
      _token = token;
      final decoded = jsonDecode(userJson);
      if (decoded is! Map) {
        await _clearSession();
        _isInitializing = false;
        notifyListeners();
        return;
      }
      _user = AuthenticatedUser.fromJson(JsonMap.from(decoded));
      _apiService.setToken(token);
    } else {
      await _clearSession();
    }
    _isInitializing = false;
    notifyListeners();
  }

  Future<void> login(String username, String password) async {
    _error = null;
    _setLoading(true);
    try {
      final response = await _authService.login(username, password);
      final roles = response.user.roles;
      if (!roles.any((role) => role.toLowerCase() == 'admin')) {
        throw ApiException(
          'Only users with the Admin role can use the desktop application.',
        );
      }
      final token = response.token;
      final expiresAt = response.expiresAt;
      if (token.isEmpty) {
        throw ApiException('Login response was incomplete.');
      }
      final user = response.user;
      final prefs = await SharedPreferences.getInstance();
      await prefs.setString(_tokenKey, token);
      await prefs.setString(_expiresAtKey, expiresAt.toUtc().toIso8601String());
      await prefs.setString(_userKey, jsonEncode(user.toJson()));
      _token = token;
      _user = user;
      _apiService.setToken(token);
      notifyListeners();
    } on ApiException catch (error) {
      _error = error.message;
    } catch (_) {
      _error = 'Login failed. Please try again.';
    } finally {
      _setLoading(false);
    }
  }

  Future<void> logout({bool notifyServer = true}) async {
    if (notifyServer && _token != null) {
      try {
        await _authService.logout();
      } on ApiException catch (error) {
        if (error.statusCode != 401) rethrow;
      }
    }
    await _clearSession();
    notifyListeners();
  }

  Future<void> _clearSession() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove(_tokenKey);
    await prefs.remove(_userKey);
    await prefs.remove(_expiresAtKey);
    _token = null;
    _user = null;
    _apiService.setToken(null);
  }

  void _setLoading(bool value) {
    _isLoading = value;
    notifyListeners();
  }
}
