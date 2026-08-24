class AppConfig {
  static const String apiBaseUrl = String.fromEnvironment(
    'API_BASE_URL',
    defaultValue: 'http://localhost:5281',
  );

  static String get resolvedApiBaseUrl {
    return apiBaseUrl.trim().replaceAll(RegExp(r'/+$'), '');
  }
}
