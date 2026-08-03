import '../models/wellness_service.dart';
import 'api_service.dart';

class WellnessServiceService {
  WellnessServiceService(this._apiService);

  final ApiService _apiService;

  Future<WellnessService> getServiceById(int id) {
    return _apiService.get<WellnessService>(
      '/Service/$id',
      fromJson: (data) => WellnessService.fromJson(data as Map<String, dynamic>),
    );
  }
}
