import 'package:flutter/foundation.dart';

import '../models/user.dart';

class ProfileProvider extends ChangeNotifier {
  ProfileProvider({required User? user}) : _user = user;

  final User? _user;
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
      if (_user == null) {
        _profile = null;
        _error = 'Profile could not be loaded. Please sign in again.';
        return;
      }

      _profile = ProfileViewData(
        userId: _user.id,
        fullName: _user.fullName,
        username: _user.username,
        email: _user.email,
        role: _user.primaryRole,
        roles: _user.roles,
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
}

class ProfileViewData {
  ProfileViewData({
    required this.userId,
    required this.fullName,
    required this.username,
    required this.email,
    required this.role,
    required this.roles,
    this.phoneNumber,
    this.isActive,
  });

  final int userId;
  final String fullName;
  final String username;
  final String email;
  final String role;
  final List<String> roles;
  final String? phoneNumber;
  final bool? isActive;
}
