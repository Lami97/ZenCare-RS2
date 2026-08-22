import 'package:dio/dio.dart';

import '../config/app_config.dart';
import '../models/admin_models.dart';
import '../models/paged_result.dart';
import '../utils/api_exception.dart';

class ApiService {
  ApiService() {
    _dio.interceptors.add(
      InterceptorsWrapper(
        onRequest: (options, handler) {
          final token = _token;
          if (token != null && token.isNotEmpty) {
            options.headers['Authorization'] = 'Bearer $token';
          }
          handler.next(options);
        },
        onError: (error, handler) {
          if (error.response?.statusCode == 401) {
            _onUnauthorized?.call();
          }
          handler.next(error);
        },
      ),
    );
  }

  final Dio _dio = Dio(
    BaseOptions(
      baseUrl: AppConfig.resolvedApiBaseUrl,
      connectTimeout: const Duration(seconds: 15),
      receiveTimeout: const Duration(seconds: 25),
      headers: const {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
      },
    ),
  );

  String? _token;
  void Function()? _onUnauthorized;

  void setToken(String? token) => _token = token;
  void setUnauthorizedHandler(void Function()? handler) =>
      _onUnauthorized = handler;

  Future<T> getObject<T>(
    String path, {
    Map<String, Object?>? queryParameters,
    required T Function(JsonMap json) decode,
  }) async {
    try {
      final response = await _dio.get<Object?>(
        _normalizePath(path),
        queryParameters: _cleanQuery(queryParameters),
      );
      return decode(_asJsonMap(response.data));
    } on DioException catch (error) {
      throw _toApiException(error);
    }
  }

  Future<PagedResult<T>> getPaged<T>(
    String path, {
    Map<String, Object?>? queryParameters,
    required T Function(JsonMap json) decodeItem,
  }) async {
    try {
      final response = await _dio.get<Object?>(
        _normalizePath(path),
        queryParameters: _cleanQuery(queryParameters),
      );
      return PagedResult.fromJson(response.data, decodeItem);
    } on DioException catch (error) {
      throw _toApiException(error);
    }
  }

  Future<T> postObject<T>(
    String path, {
    Object? data,
    required T Function(JsonMap json) decode,
  }) async {
    try {
      final response = await _dio.post<Object?>(
        _normalizePath(path),
        data: data,
      );
      return decode(_asJsonMap(response.data));
    } on DioException catch (error) {
      throw _toApiException(error);
    }
  }

  Future<T> putObject<T>(
    String path, {
    Object? data,
    required T Function(JsonMap json) decode,
  }) async {
    try {
      final response = await _dio.put<Object?>(
        _normalizePath(path),
        data: data,
      );
      return decode(_asJsonMap(response.data));
    } on DioException catch (error) {
      throw _toApiException(error);
    }
  }

  Future<void> post(String path, {Object? data}) async {
    try {
      await _dio.post<Object?>(_normalizePath(path), data: data);
    } on DioException catch (error) {
      throw _toApiException(error);
    }
  }

  Future<void> delete(String path) async {
    try {
      await _dio.delete<Object?>(_normalizePath(path));
    } on DioException catch (error) {
      throw _toApiException(error);
    }
  }

  String _normalizePath(String path) {
    final trimmed = path.trim();
    if (trimmed.startsWith(RegExp(r'https?://'))) {
      return trimmed;
    }
    return trimmed.startsWith('/') ? trimmed : '/$trimmed';
  }

  Map<String, Object?>? _cleanQuery(Map<String, Object?>? query) {
    if (query == null) return null;
    final cleaned = <String, Object?>{};
    for (final entry in query.entries) {
      final value = entry.value;
      if (value == null) continue;
      if (value is String && value.trim().isEmpty) continue;
      cleaned[entry.key] = value;
    }
    return cleaned;
  }

  JsonMap _asJsonMap(Object? data) {
    if (data is Map) return JsonMap.from(data);
    throw const FormatException('The API response was not a JSON object.');
  }

  ApiException _toApiException(DioException error) {
    final statusCode = error.response?.statusCode;
    final data = error.response?.data;
    if (data is String && data.trim().isNotEmpty) {
      return ApiException(data, statusCode: statusCode);
    }
    if (data is Map) {
      final map = Map<String, dynamic>.from(data);
      final message = map['message'] ?? map['title'] ?? map['error'];
      if (message != null && message.toString().trim().isNotEmpty) {
        return ApiException(message.toString(), statusCode: statusCode);
      }
    }
    if (statusCode == 401) {
      return ApiException(
        'Your session expired. Please sign in again.',
        statusCode: statusCode,
      );
    }
    if (statusCode == 403) {
      return ApiException(
        'You do not have permission to use this admin function.',
        statusCode: statusCode,
      );
    }
    return ApiException(
      'Network error. Please try again.',
      statusCode: statusCode,
    );
  }
}
