import '../models/payment_intent_response.dart';
import '../models/payment_status_response.dart';
import 'api_service.dart';

class PaymentService {
  PaymentService(this._apiService);

  final ApiService _apiService;

  Future<PaymentIntentResponse> createPaymentIntent(int purchaseId) {
    return _apiService.post<PaymentIntentResponse>(
      '/Payment/My/create-intent/$purchaseId',
      fromJson: (data) => PaymentIntentResponse.fromJson(data as Map<String, dynamic>),
    );
  }

  Future<PaymentStatusResponse> confirmPayment(int purchaseId) {
    return _apiService.post<PaymentStatusResponse>(
      '/Payment/My/confirm/$purchaseId',
      fromJson: (data) => PaymentStatusResponse.fromJson(data as Map<String, dynamic>),
    );
  }

  Future<PaymentStatusResponse> refundPayment(int purchaseId) {
    return _apiService.post<PaymentStatusResponse>(
      '/Payment/My/refund/$purchaseId',
      fromJson: (data) => PaymentStatusResponse.fromJson(data as Map<String, dynamic>),
    );
  }
}
