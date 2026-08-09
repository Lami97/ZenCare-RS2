import 'package:flutter/foundation.dart';

import '../models/user.dart';
import 'auth_provider.dart';

class ProfileProvider extends ChangeNotifier {
  ProfileProvider({required AuthProvider authProvider}) : _authProvider = authProvider;

  final AuthProvider _authProvider;
  User? _user;
  bool _isLoading = false;
  String? _error;
  ProfileViewData? _profile;

  bool get isLoading => _isLoading;
  String? get error => _error;
  ProfileViewData? get profile => _profile;
  bool get isEmpty => !_isLoading && _error == null && _profile == null;

  Future<void> loadProfile() async {
    _isLoading = true;
    _error = null;
    notifyListeners();

    try {
      if (_authProvider.user == null) {
        _profile = null;
        _error = 'Profile could not be loaded. Please sign in again.';
        return;
      }

      await _authProvider.refreshProfile();
      _user = _authProvider.user;

      if (_user == null) {
        _profile = null;
        _error = 'Profile could not be loaded. Please sign in again.';
        return;
      }

      final user = _user!;

      _profile = ProfileViewData(
        userId: user.id,
        firstName: user.firstName,
        lastName: user.lastName,
        fullName: user.fullName,
        username: user.username,
        email: user.email,
        role: user.primaryRole,
        roles: user.roles,
        phoneNumber: user.phoneNumber,
        isActive: user.isActive,
      );
    } catch (_) {
      _profile = null;
      _error = 'Profile could not be loaded. Please try again.';
    } finally {
      _isLoading = false;
      notifyListeners();
    }
  }

  Future<void> refresh() => loadProfile();

  Future<void> retry() => loadProfile();

  Future<void> updateUser(User? user) {
    _user = user;
    _profile = user == null
        ? null
        : ProfileViewData(
            userId: user.id,
            firstName: user.firstName,
            lastName: user.lastName,
            fullName: user.fullName,
            username: user.username,
            email: user.email,
            role: user.primaryRole,
            roles: user.roles,
            phoneNumber: user.phoneNumber,
            isActive: user.isActive,
          );
    notifyListeners();
    return Future.value();
  }
}

class ProfileViewData {
  ProfileViewData({
    required this.userId,
    required this.firstName,
    required this.lastName,
    required this.fullName,
    required this.username,
    required this.email,
    required this.role,
    required this.roles,
    this.phoneNumber,
    this.isActive,
  });

  final int userId;
  final String firstName;
  final String lastName;
  final String fullName;
  final String username;
  final String email;
  final String role;
  final List<String> roles;
  final String? phoneNumber;
  final bool? isActive;
}
