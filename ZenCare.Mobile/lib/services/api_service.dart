import 'package:dio/dio.dart';

import '../core/app_config.dart';
import '../utils/api_exception.dart';

class ApiService {
  ApiService() {
    _dio.interceptors.add(
      InterceptorsWrapper(
        onRequest: (options, handler) {
          if (_token != null && _token!.isNotEmpty) {
            options.headers['Authorization'] = 'Bearer $_token';
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
      baseUrl: AppConfig.apiBaseUrl,
      connectTimeout: const Duration(seconds: 15),
      receiveTimeout: const Duration(seconds: 20),
      headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
      },
    ),
  );

  String? _token;
  void Function()? _onUnauthorized;

  void setToken(String? token) {
    _token = token;
  }

  void setUnauthorizedHandler(void Function()? handler) {
    _onUnauthorized = handler;
  }

  Future<T> get<T>(
    String path, {
    Map<String, dynamic>? queryParameters,
    required T Function(dynamic data) fromJson,
  }) async {
    try {
      final response = await _dio.get<dynamic>(
        path,
        queryParameters: queryParameters,
      );
      return fromJson(response.data);
    } on DioException catch (error) {
      throw _toApiException(error);
    }
  }

  Future<T> post<T>(
    String path, {
    Object? data,
    required T Function(dynamic data) fromJson,
  }) async {
    try {
      final response = await _dio.post<dynamic>(path, data: data);
      return fromJson(response.data);
    } on DioException catch (error) {
      throw _toApiException(error);
    }
  }

  ApiException _toApiException(DioException error) {
    final statusCode = error.response?.statusCode;
    final data = error.response?.data;

    if (data is String && data.trim().isNotEmpty) {
      return ApiException(data, statusCode: statusCode);
    }

    if (data is Map<String, dynamic>) {
      final message = data['message'] ?? data['title'] ?? data['error'];
      if (message != null) {
        return ApiException(message.toString(), statusCode: statusCode);
      }
    }

    if (statusCode == 401) {
      return ApiException('Your session has expired. Please log in again.', statusCode: statusCode);
    }

    return ApiException('Network error. Please try again.', statusCode: statusCode);
  }

  void dispose() {
    _dio.close(force: true);
  }
}
