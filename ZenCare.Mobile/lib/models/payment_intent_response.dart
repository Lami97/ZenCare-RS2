class PaymentIntentResponse {
  PaymentIntentResponse({
    required this.purchaseId,
    required this.paymentId,
    required this.stripePaymentIntentId,
    required this.clientSecret,
    required this.publishableKey,
    required this.amount,
    required this.currency,
    required this.status,
  });

  final int purchaseId;
  final int paymentId;
  final String stripePaymentIntentId;
  final String clientSecret;
  final String publishableKey;
  final double amount;
  final String currency;
  final int status;

  factory PaymentIntentResponse.fromJson(Map<String, dynamic> json) {
    return PaymentIntentResponse(
      purchaseId: json['purchaseId'] as int? ?? 0,
      paymentId: json['paymentId'] as int? ?? 0,
      stripePaymentIntentId: json['stripePaymentIntentId'] as String? ?? '',
      clientSecret: json['clientSecret'] as String? ?? '',
      publishableKey: json['publishableKey'] as String? ?? '',
      amount: (json['amount'] as num?)?.toDouble() ?? 0,
      currency: json['currency'] as String? ?? '',
      status: json['status'] as int? ?? 0,
    );
  }
}
