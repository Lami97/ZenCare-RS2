class AppConfig {
  static const String apiBaseUrl = String.fromEnvironment('API_BASE_URL');

  static String get resolvedApiBaseUrl {
    if (apiBaseUrl.trim().isEmpty) {
      throw StateError(
        'API_BASE_URL is not configured. Start with --dart-define=API_BASE_URL=http://localhost:5281',
      );
    }

    return apiBaseUrl.trim().replaceAll(RegExp(r'/+$'), '');
  }
}
